package quest03

import (
	"everybodycodes_2024_Go/utilities"
)

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

		checkers := [][]int{{-1, 0}, {1, 0}, {0, 1}, {0, -1}}
		calculateHeight(true, 1, checkers, &miningmap)
	}

	result = calculateDigs(&miningmap)
	return result
}
