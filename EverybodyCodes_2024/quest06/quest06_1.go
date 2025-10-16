package quest06

import (
	"everybodycodes_2024/utilities"
	"strings"
)

type Node struct {
	Name   string
	Leaves []Node
}

func Executepart1() int {
	var fileName string = "./quest06/test_input_01.txt"
	// var fileName string = "./quest06/quest06_1.txt"
	var result int = 0

	// parsing
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		numRows, numCols := len(fileContent), len(strings.Split(fileContent[0], " "))
		nodes := make([]Node, 0)
		for _, line := range fileContent {

		}
	}

	return result
}
