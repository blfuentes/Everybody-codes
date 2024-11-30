package quest04

import (
	"everybodycodes_2024/utilities"
	"math"
	"strconv"
)

func Executepart3() int {
	// var fileName string = "./quest04/test_input_01.txt"
	var fileName string = "./quest04/quest04_3.txt"
	var result int = 0
	var nails []int
	sumnails := 0
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		for _, val := range fileContent {
			value, _ := strconv.Atoi(val)
			sumnails += value
			nails = append(nails, value)
		}
	}

	minValue := math.MaxInt64
	for nIdx := 0; nIdx < len(nails); nIdx++ {
		checker := nails[nIdx]
		finished := true
		result = 0
		for _, value := range nails {
			result += int(math.Abs(float64(checker) - float64(value)))
			if result > minValue {
				finished = false
				break
			}
		}
		if finished && result < minValue {
			minValue = result
		}
	}

	return minValue
}
