package quest02

import (
	"everybodycodes_2024/utilities"
	"fmt"
	"strings"
)

func Executepart3() int {
	var fileName string = "./quest02/quest02_3.txt"
	// var fileName string = "./quest02/test_input_03.txt"
	var result int = 0
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		fmt.Printf("%s\n", fileContent[0])
		parts := strings.Split(strings.Split(fileContent[0], ":")[1], ",")

		initiallength := len(fileContent[2])
		expandedlength := 0
		for _, part := range parts {
			if len(part) > expandedlength {
				expandedlength = len((part))
			}
		}

		var matrix [][]int
		var wordmap [][]string

		for lineIdx := 2; lineIdx < len(fileContent); lineIdx++ {
			row := make([]string, len(fileContent[lineIdx])+expandedlength-1)
			runes := []rune(fileContent[lineIdx])
			for colIdx := 0; colIdx < len(runes); colIdx++ {
				row[colIdx] = string(runes[colIdx])
			}
			for colIdx := 0; colIdx < expandedlength-1; colIdx++ {
				row[initiallength+colIdx] = row[colIdx]
			}
			matrix = append(matrix, make([]int, len(fileContent[lineIdx])))
			wordmap = append(wordmap, row)
		}

		for _, part := range parts {
			rpart := utilities.ReverseString(part)
			// right
			for rowIdx := 0; rowIdx < len(wordmap); rowIdx++ {
				for colIdx := 0; colIdx < len(wordmap[rowIdx]); {
					toSearch := strings.Join(wordmap[rowIdx][colIdx:], "")
					if foundIdx := strings.Index(toSearch, part); foundIdx != -1 {
						for c := 0; c < len(part); c++ {
							if matrix[rowIdx][(colIdx+foundIdx+c)%initiallength] != 1 {
								matrix[rowIdx][(colIdx+foundIdx+c)%initiallength] = 1
							}
						}
						colIdx = colIdx + foundIdx + 1
					} else {
						break
					}
				}

			}
			// left
			for rowIdx := 0; rowIdx < len(wordmap); rowIdx++ {
				for colIdx := 0; colIdx < len(wordmap[rowIdx]); {
					toSearch := strings.Join(wordmap[rowIdx][colIdx:], "")

					if foundIdx := strings.Index(toSearch, rpart); foundIdx != -1 {
						for c := 0; c < len(rpart); c++ {
							if matrix[rowIdx][(colIdx+foundIdx+c)%initiallength] != 1 {
								matrix[rowIdx][(colIdx+foundIdx+c)%initiallength] = 1
							}
						}
						colIdx = colIdx + foundIdx + 1
					} else {
						break
					}
				}

			}
		}

		for _, part := range parts {
			rpart := utilities.ReverseString(part)
			// down
			for coldIdx := 0; coldIdx < len(matrix[0]); coldIdx++ {
				for rowIdx := 0; rowIdx < len(matrix); {
					var columnElements []string
					for i := rowIdx; i < len(matrix); i++ {
						if coldIdx < len(matrix[i]) {
							columnElements = append(columnElements, wordmap[i][coldIdx])
						}
					}
					toSearch := strings.Join(columnElements, "")

					if foundIdx := strings.Index(toSearch, part); foundIdx != -1 {
						for c := 0; c < len(part); c++ {
							if matrix[rowIdx+foundIdx+c][coldIdx] != 1 {
								matrix[rowIdx+foundIdx+c][coldIdx] = 1
							}
						}
						rowIdx = rowIdx + foundIdx + 1
					} else {
						break
					}
				}
			}
			// up
			for coldIdx := 0; coldIdx < len(matrix[0]); coldIdx++ {
				for rowIdx := 0; rowIdx < len(matrix); {
					var columnElements []string
					for i := rowIdx; i < len(matrix); i++ {
						if coldIdx < len(matrix[i]) {
							columnElements = append(columnElements, wordmap[i][coldIdx])
						}
					}
					toSearch := strings.Join(columnElements, "")

					if foundIdx := strings.Index(toSearch, rpart); foundIdx != -1 {
						for c := 0; c < len(rpart); c++ {
							if matrix[rowIdx+foundIdx+c][coldIdx] != 1 {
								matrix[rowIdx+foundIdx+c][coldIdx] = 1
							}
						}
						rowIdx = rowIdx + foundIdx + 1
					} else {
						break
					}
				}
			}
		}
		for rowIdx := 0; rowIdx < len(matrix); rowIdx++ {
			for colIdx := 0; colIdx < len(matrix[rowIdx]); colIdx++ {
				result = result + matrix[rowIdx][colIdx]
			}
		}
	}
	return result
}
