package quest05

import (
	"everybodycodes_2024/utilities"
	"strconv"
	"strings"
)

func ClapperRepeated(clappersMemo map[int]utilities.MatrixList[int], round int, toCheck utilities.Matrix[int]) bool {
	found := false

	existingClappers := clappersMemo[round]

	for _, tmpClapper := range existingClappers {
		found = utilities.AreEqual(tmpClapper, toCheck)
		if found {
			break
		}
	}

	return found
}

func Executepart3() int64 {
	// var fileName string = "./quest05/test_input_03.txt"
	var fileName string = "./quest05/quest05_3.txt"
	var result int64 = 0
	var clappers [][]int

	// parsing
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		numRows, numCols := len(fileContent), len(strings.Split(fileContent[0], " "))

		clappers = make([][]int, numCols)
		for colIdx := range clappers {
			clappers[colIdx] = make([]int, numRows)
		}

		for rowIdx, row := range fileContent {
			var parts = strings.Split(row, " ")
			for colIdx, col := range parts {
				value, _ := strconv.Atoi(col)
				clappers[colIdx][rowIdx] = value
			}
		}
	}

	var found bool = false

	memo := make(map[int]utilities.MatrixList[int])

	for currentRound := 1; !found; currentRound++ {
		var tmpValue = ""
		ClapRound(currentRound, &clappers)
		exp := 0
		for idx := 0; idx < len(clappers); idx++ {
			tmpValue += strconv.Itoa(clappers[idx][0])
			exp++
		}
		calculatedValue, _ := strconv.ParseInt(tmpValue, 10, 64)
		if calculatedValue > result {
			result = calculatedValue
		}
		toCheck := utilities.CopyMatrix(clappers)
		roundIdx := currentRound % len(clappers)
		found = ClapperRepeated(memo, roundIdx, toCheck)
		if !found {
			_, exists := memo[roundIdx]
			if !exists {
				memo[roundIdx] = utilities.MatrixList[int]{toCheck}
			} else {
				memo[roundIdx] = append(memo[roundIdx], toCheck)
			}
		}
	}

	return result
}
