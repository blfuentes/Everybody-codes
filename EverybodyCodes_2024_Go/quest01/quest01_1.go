package quest01

import (
	"everybodycodes_2024_Go/utilities"
)

func Executepart1() int {
	// var fileName string = "./quest01/test_input_01.txt"
	var fileName string = "./quest01/quest01_1.txt"
	var result int = 0
	if fileContent, err := utilities.ReadFileAsText(fileName); err == nil {
		chars := []rune(fileContent)
		for i := 0; i < len(chars); i++ {
			switch string(chars[i]) {
			case "A":
				result = result
			case "B":
				result = result + 1
			case "C":
				result = result + 3
			}
		}
	}
	return result
}
