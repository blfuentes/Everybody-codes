package quest06

import (
	"everybodycodes_2024_Go/utilities"
	"strings"
)

func FruitPath(fruit Node) string {
	result := fruit.Name

	for fruit.Parent != nil {
		result = fruit.Parent.Name + result
		fruit = *fruit.Parent
	}

	return result
}

func Executepart1() string {
	// var fileName string = "./quest06/test_input_01.txt"
	var fileName string = "./quest06/quest06_1.txt"
	var result string = ""

	// parsing
	tree := make(map[string]*Node)
	fruits := make([]*Node, 0)
	paths := make([]string, 0)
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		// numRows, numCols := len(fileContent), len(strings.Split(fileContent[0], " "))
		for _, line := range fileContent {
			fromNode, toNodes := strings.Split(line, ":")[0], strings.Split(strings.Split(line, ":")[1], ",")
			node, exists := tree[fromNode]
			if !exists {
				node = &Node{nil, fromNode, make([]*Node, 0), false}
				tree[fromNode] = node
			}
			for _, n := range toNodes {
				child, exists := tree[n]
				if !exists {
					child = &Node{node, n, make([]*Node, 0), n == "@"}
					if child.IsFruit {
						fruits = append(fruits, child)
					} else {
						tree[n] = child
					}
				}
				child.Parent = node
				node.Leaves = append(node.Leaves, child)
			}
		}

		for _, fruit := range fruits {
			paths = append(paths, FruitPath(*fruit))
		}
	}

	result = FindPowerfull(paths)

	return result
}
