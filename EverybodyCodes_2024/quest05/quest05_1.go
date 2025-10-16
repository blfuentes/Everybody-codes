package quest05

import (
	"everybodycodes_2024/utilities"
	"fmt"
	"math"
	"strconv"
	"strings"
)

func FindSpot(numOfClaps int, clapperQueue []int) (side int, pos int) {
	n := len(clapperQueue)
	leftSideInit, leftSideEnd := 0, n-1
	rightSideInit, rightSideEnd := n, (2*n)-1

	walkingSize := n * 2

	fallingPoint := (numOfClaps % walkingSize) - 1

	if fallingPoint >= leftSideInit && fallingPoint <= leftSideEnd {
		return 0, fallingPoint
	} else if fallingPoint >= rightSideInit && fallingPoint <= rightSideEnd {
		fallingPoint = (walkingSize - 1) - fallingPoint
		return 1, fallingPoint
	}
	return -1, -1
}

func ClapRound(round int, clappers *[][]int) {
	clapperOriginIdx := (round - 1) % len(*clappers)
	clapperQueueIdx := round % len(*clappers)
	claps := (*clappers)[clapperOriginIdx][0]

	(*clappers)[clapperOriginIdx] = (*clappers)[clapperOriginIdx][1:]

	side, pos := FindSpot(claps, (*clappers)[clapperQueueIdx])
	if side == 0 {
		// insert upfront of position
		(*clappers)[clapperQueueIdx] = utilities.InsertAt((*clappers)[clapperQueueIdx], pos, claps)
	} else {
		(*clappers)[clapperQueueIdx] = utilities.InsertAt((*clappers)[clapperQueueIdx], pos+1, claps)
	}
}

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
	fmt.Printf("%v\n", result)

	return result
}
