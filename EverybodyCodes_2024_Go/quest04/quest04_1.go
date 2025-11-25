package quest04

import (
	"everybodycodes_2024_Go/utilities"
	"math"
	"strconv"
)

func Executepart1() int {
	// var fileName string = "./quest04/test_input_01.txt"
	var fileName string = "./quest04/quest04_1.txt"
	var result int = 0
	var nails []int
	minValue := math.MaxInt
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		for _, val := range fileContent {
			value, _ := strconv.Atoi(val)
			if value < minValue {
				minValue = value
			}
			nails = append(nails, value)
		}
	}

	for _, value := range nails {
		if value != minValue {
			result += value - minValue
		}
	}

	return result
}
