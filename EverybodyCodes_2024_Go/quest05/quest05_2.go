package quest05

import (
	"everybodycodes_2024_Go/utilities"
	"strconv"
	"strings"
)

func Executepart2() int64 {
	// var fileName string = "./quest05/test_input_02.txt"
	var fileName string = "./quest05/quest05_2.txt"
	var result int64 = 0
	var clappers [][]int

	shouts := make(map[int64]int)

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

	for currentRound := 1; !found; currentRound++ {
		var tmpValue = ""
		ClapRound(currentRound, &clappers)
		exp := 0
		for idx := 0; idx < len(clappers); idx++ {
			tmpValue += strconv.Itoa(clappers[idx][0])
			exp++
		}
		calculatedValue, _ := strconv.ParseInt(tmpValue, 10, 64)
		_, exists := shouts[calculatedValue]
		if exists {
			shouts[calculatedValue]++
			if shouts[calculatedValue] == 2024 {
				found = true
				result = int64(currentRound) * calculatedValue
			}
		} else {
			shouts[calculatedValue] = 1
		}
	}

	return result
}
