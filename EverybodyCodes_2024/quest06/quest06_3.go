package quest06

import (
	"everybodycodes_2024/utilities"
	"fmt"
	"strings"
)

func Executepart3() int {
	// var fileName string = "./quest06/test_input_01.txt"
	var fileName string = "./quest06/quest06_1.txt"
	var result int = 0

	// parsing
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		numRows, numCols := len(fileContent), len(strings.Split(fileContent[0], " "))
		fmt.Printf("%v, %v", numRows, numCols)
	}

	return result
}
