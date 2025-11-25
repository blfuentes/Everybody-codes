package quest03

import "everybodycodes_2024_Go/utilities"

func calculateDigs(miningmap *[][]int) int {
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

func calculateHeight(repeat bool, level int, checkers [][]int, miningmap *[][]int) {
	if repeat {
		maxRows := len(*miningmap)
		maxCols := len((*miningmap)[0])
		doRepeat := false
		for rowIdx := 0; rowIdx < maxRows; rowIdx++ {
			for colIdx := 0; colIdx < maxCols; colIdx++ {
				if (*miningmap)[rowIdx][colIdx] > 0 {
					add := true
					for checkIdx := 0; checkIdx < len(checkers); checkIdx++ {
						checkRow := rowIdx + checkers[checkIdx][0]
						checkCol := colIdx + checkers[checkIdx][1]
						if utilities.IsInBoundaries(checkRow, checkCol, maxRows, maxCols) {
							if (*miningmap)[checkRow][checkCol] < level {
								add = false
								break
							}
						} else {
							add = false
							break
						}
					}

					if add {
						doRepeat = true
						(*miningmap)[rowIdx][colIdx] += 1
					}
				}
			}
		}
		if doRepeat {
			calculateHeight(doRepeat, level+1, checkers, miningmap)
		}
	}
}
