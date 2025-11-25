package quest05

import (
	"everybodycodes_2024_Go/utilities"
	"math"
	"strconv"
	"strings"
)

func Executepart1() int {
	// var fileName string = "./quest05/test_input_01.txt"
	var fileName string = "./quest05/quest05_1.txt"
	var result int = 0
	var clappers [][]int
	var numOfRound int = 10
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

	for currentRound := 1; currentRound <= numOfRound; currentRound++ {
		ClapRound(currentRound, &clappers)

	}

	exp := 0
	for idx := len(clappers) - 1; idx >= 0; idx-- {
		result += clappers[idx][0] * int(math.Pow10(exp))
		exp++
	}

	return result
}
