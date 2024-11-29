package quest03

import (
	"everybodycodes_2024/utilities"
	"fmt"
)

func printMap(miningmap *[][]int) {
	for rowIdx := 0; rowIdx < len(*miningmap); rowIdx++ {
		for colIdx := 0; colIdx < len((*miningmap)[rowIdx]); colIdx++ {
			fmt.Printf("%d", (*miningmap)[rowIdx][colIdx])
		}
		fmt.Println()
	}
}

func canCheck(row, col, maxrows, maxcols int) bool {
	return row >= 0 && row < maxrows && col >= 0 && col < maxcols
}

func calculateHeight(repeat bool, level int, miningmap *[][]int) {
	if repeat {
		maxRows := len(*miningmap)
		maxCols := len((*miningmap)[0])
		doRepeat := false
		for rowIdx := 0; rowIdx < maxRows; rowIdx++ {
			for colIdx := 0; colIdx < maxCols; colIdx++ {
				// up
				addUp := false
				if canCheck(rowIdx-1, colIdx, maxRows, maxCols) {
					addUp = (*miningmap)[rowIdx-1][colIdx] >= level
				}
				// right
				addRight := false
				if canCheck(rowIdx, colIdx+1, maxRows, maxCols) {
					addRight = (*miningmap)[rowIdx][colIdx+1] >= level
				}
				// left
				addLeft := false
				if canCheck(rowIdx, colIdx-1, maxRows, maxCols) {
					addLeft = (*miningmap)[rowIdx][colIdx-1] >= level
				}
				// down
				addDown := false
				if canCheck(rowIdx+1, colIdx, maxRows, maxCols) {
					addDown = (*miningmap)[rowIdx+1][colIdx] >= level
				}

				if addUp && addRight && addLeft && addDown {
					doRepeat = true
					(*miningmap)[rowIdx][colIdx] += 1
				}
			}
		}
		if doRepeat {
			calculateHeight(doRepeat, level+1, miningmap)
		}
	}
}

func calculateDigs(miningmap *[][]int) int {
	levels := make([]int, 10)
	for rowIdx := 0; rowIdx < len(*miningmap); rowIdx++ {
		for colIdx := 0; colIdx < len((*miningmap)[rowIdx]); colIdx++ {
			for cc := (*miningmap)[rowIdx][colIdx]; cc > 0; cc-- {
				levels[cc] += 1
			}
		}
	}
	result := 0
	for idx := 0; idx < len(levels); idx++ {
		result += levels[idx]
	}

	return result
}

func Executepart1() int {
	// var fileName string = "./quest03/test_input_01.txt"
	var fileName string = "./quest03/quest03_1.txt"
	var result int = 0
	var miningmap [][]int
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		for rowIdx := 0; rowIdx < len(fileContent); rowIdx++ {
			line := []rune(fileContent[rowIdx])
			row := make([]int, len(line))
			for colIdx := 0; colIdx < len(line); colIdx++ {
				if string(line[colIdx]) == "#" {
					row[colIdx] = 1
				} else {
					row[colIdx] = 0
				}
			}
			miningmap = append(miningmap, row)
		}

		// fmt.Println("End state")
		// printMap(&miningmap)

		calculateHeight(true, 1, &miningmap)

	}

	// printMap(&miningmap)
	result = calculateDigs(&miningmap)
	return result
}
