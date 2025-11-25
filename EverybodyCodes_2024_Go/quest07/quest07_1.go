package quest07

import (
	"everybodycodes_2024_Go/utilities"
	"strings"
)

func Executepart1() string {
	// var fileName string = "./quest07/test_input_01.txt"
	var fileName string = "./quest07/quest07_1.txt"
	var result string = ""

	chariots := make(map[string]int)
	rules := make(map[string][]string)
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		for _, line := range fileContent {
			charName, parts := strings.Split(line, ":")[0], strings.Split(strings.Split(line, ":")[1], ",")
			rules[charName] = parts
			startingValue := 10
			sum := 0
			for idx := 0; idx < 10; idx++ {
				sign := rules[charName][idx%len(parts)]
				switch sign {
				case "+":
					startingValue++
				case "-":
					startingValue--
				}
				sum += startingValue
			}
			chariots[charName] = sum
		}

		result = SortByValue(chariots)
	}
	return result
}
