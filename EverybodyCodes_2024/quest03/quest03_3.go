package quest03

import (
	"everybodycodes_2024/utilities"
	"fmt"

	"github.com/fatih/color"
)

func printMatrixWithColors(matrix [][]int) {
	colors := []func(a ...interface{}) string{
		color.New(color.BgBlack).SprintFunc(),
		color.New(color.BgBlue).SprintFunc(),
		color.New(color.BgGreen).SprintFunc(),
		color.New(color.BgHiMagenta).SprintFunc(),
		color.New(color.BgHiRed).SprintFunc(),
		color.New(color.BgHiWhite).SprintFunc(),
		color.New(color.FgBlue).SprintFunc(),
		color.New(color.BgHiYellow).SprintFunc(),
		color.New(color.FgGreen).SprintFunc(),
		color.New(color.BgBlack).SprintFunc(),
	}

	for rowIdx, row := range matrix {
		for colIdx, value := range row {
			colorIdx := matrix[rowIdx][colIdx]
			coloredValue := colors[colorIdx](value)
			fmt.Print(coloredValue, " ")
		}
		fmt.Println()
	}
}

func printMap3(miningmap *[][]int) {
	for rowIdx := 0; rowIdx < len(*miningmap); rowIdx++ {
		for colIdx := 0; colIdx < len((*miningmap)[rowIdx]); colIdx++ {
			fmt.Printf("%d", (*miningmap)[rowIdx][colIdx])
		}
		fmt.Println()
	}
}

func canCheck3(row, col, maxrows, maxcols int) bool {
	return row >= 0 && row < maxrows && col >= 0 && col < maxcols
}

func calculateHeight3(repeat bool, level int, miningmap *[][]int) {
	if repeat {
		maxRows := len(*miningmap)
		maxCols := len((*miningmap)[0])
		doRepeat := false
		for rowIdx := 0; rowIdx < maxRows; rowIdx++ {
			for colIdx := 0; colIdx < maxCols; colIdx++ {
				if (*miningmap)[rowIdx][colIdx] > 0 {
					// up
					addUp := false
					if canCheck3(rowIdx-1, colIdx, maxRows, maxCols) {
						addUp = (*miningmap)[rowIdx-1][colIdx] >= level
					}
					// right
					addRight := false
					if canCheck3(rowIdx, colIdx+1, maxRows, maxCols) {
						addRight = (*miningmap)[rowIdx][colIdx+1] >= level
					}
					// left
					addLeft := false
					if canCheck3(rowIdx, colIdx-1, maxRows, maxCols) {
						addLeft = (*miningmap)[rowIdx][colIdx-1] >= level
					}
					// down
					addDown := false
					if canCheck3(rowIdx+1, colIdx, maxRows, maxCols) {
						addDown = (*miningmap)[rowIdx+1][colIdx] >= level
					}
					// top-left
					addTopLeft := false
					if canCheck3(rowIdx-1, colIdx-1, maxRows, maxCols) {
						addTopLeft = (*miningmap)[rowIdx-1][colIdx-1] >= level
					}
					// top-right
					addTopRight := false
					if canCheck3(rowIdx-1, colIdx+1, maxRows, maxCols) {
						addTopRight = (*miningmap)[rowIdx-1][colIdx+1] >= level
					}
					// down-left
					addDownLeft := false
					if canCheck3(rowIdx+1, colIdx-1, maxRows, maxCols) {
						addDownLeft = (*miningmap)[rowIdx+1][colIdx-1] >= level
					}
					// down-right
					addDownRight := false
					if canCheck3(rowIdx+1, colIdx+1, maxRows, maxCols) {
						addDownRight = (*miningmap)[rowIdx+1][colIdx+1] >= level
					}

					if addUp && addRight && addLeft && addDown &&
						addTopLeft && addTopRight && addDownLeft && addDownRight {
						doRepeat = true
						(*miningmap)[rowIdx][colIdx] += 1
					}
				}
			}
		}
		if doRepeat {
			calculateHeight3(doRepeat, level+1, miningmap)
		}
	}
}

func calculateDigs3(miningmap *[][]int) int {
	levels := make([]int, 15)
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

func Executepart3() int {
	// var fileName string = "./quest03/test_input_01.txt"
	var fileName string = "./quest03/quest03_3.txt"
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

		// printMap3(&miningmap)

		calculateHeight3(true, 1, &miningmap)

	}

	// printMap3(&miningmap)
	// printMatrixWithColors(miningmap)
	result = calculateDigs3(&miningmap)
	return result
}
