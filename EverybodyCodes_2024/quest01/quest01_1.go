package quest01

import (
	"everybodycodes_2024/utilities"
)

func Executepart1() int {
	var fileName string = "./quest01/test_input_01.txt" //"./quest01/day01.txt"
	var result int = 0
	if fileContent, err := utilities.ReadFileAsText(fileName); err == nil {
		chars := []rune(fileContent)
		for i := 0; i < len(chars); i++ {
			if string(chars[i]) == "(" {
				result = result + 1
			} else {
				result = result - 1
			}
		}
	}
	return result
}
