package quest02

import (
	"everybodycodes_2024_Go/utilities"
	"strings"
)

func Executepart1() int {
	// var fileName string = "./quest02/test_input_01.txt"
	var fileName string = "./quest02/quest02_1.txt"
	var result int = 0
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		// fmt.Printf("%s\n", fileContent[0])
		parts := strings.Split(strings.Split(fileContent[0], ":")[1], ",")
		words := strings.Split(fileContent[2], " ")
		for _, part := range parts {
			found := 0
			for _, word := range words {
				nums := strings.Count(word, part)
				result = result + nums
				found = found + nums
			}
			// fmt.Printf("%s found %d times\n", part, found)
		}
	}
	return result
}
