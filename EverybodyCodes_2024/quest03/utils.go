package quest03

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
