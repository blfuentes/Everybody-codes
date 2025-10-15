package quest05

import (
	"everybodycodes_2024/utilities"
	"strconv"
	"strings"
)

func FindSpot(numOfClaps int, clapperQueue []int) (side int, pos int) {
	n := len(clapperQueue)
	if n == 0 {
		return 0, 0
	}
	if n == 1 {
		return 0, 0
	}

	period := 2 * (n - 1)
	t := numOfClaps % period

	if t < n {
		// moving forward (0 -> n-1)
		return 0, t
	} else {
		// moving backward(n-1 -> 0)
		return 1, period - t
	}
}

func ClapRound(round int, clappers *[][]int) {
	clapperOriginIdx := round % len(*clappers)
	clapperQueueIdx := clapperOriginIdx + 1
	claps := (*clappers)[clapperOriginIdx][0]
	side, pos := FindSpot(claps, (*clappers)[clapperQueueIdx])
	(*clappers)[clapperOriginIdx] = (*clappers)[clapperOriginIdx][1:]
	if side == 0 {
		// insert upfront of posistion
		(*clappers)[clapperQueueIdx] = utilities.InsertAt((*clappers)[clapperQueueIdx], pos-1, claps)
	} else {
		(*clappers)[clapperQueueIdx] = utilities.InsertAt((*clappers)[clapperQueueIdx], pos, claps)
	}
}

func Executepart1() int {
	var fileName string = "./quest05/test_input_01.txt"
	// var fileName string = "./quest05/quest05_1.txt"
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
	for currentRound := 0; currentRound < numOfRound; currentRound++ {
		ClapRound(currentRound, &clappers)
	}

	return result
}
