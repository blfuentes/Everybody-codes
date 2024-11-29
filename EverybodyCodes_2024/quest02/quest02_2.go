package quest02

import (
	"everybodycodes_2024/utilities"
	"strings"
)

func Executepart2() int {
	// var fileName string = "./quest02/test_input_02.txt"
	var fileName string = "./quest02/quest02_2.txt"
	var result int = 0
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		// fmt.Printf("%s\n", fileContent[0])
		parts := strings.Split(strings.Split(fileContent[0], ":")[1], ",")
		mappings := make(map[int][]int)
		for lineIdx := 2; lineIdx < len(fileContent); lineIdx++ {
			line := fileContent[lineIdx]
			mappings[lineIdx] = []int{}
			for _, part := range parts {
				rpart := utilities.ReverseString(part)
				// forward
				for idx := 0; idx < len(line); {
					toSearch := line[idx:]
					if foundIdx := strings.Index(toSearch, part); foundIdx != -1 {
						for c := 0; c < len(part); c++ {
							if !utilities.Contains(mappings[lineIdx], idx+foundIdx+c) {
								mappings[lineIdx] = append(mappings[lineIdx], idx+foundIdx+c)
							}
						}
						idx = idx + foundIdx + 1
					} else {
						break
					}
				}

				// reverse
				for idx := 0; idx < len(line); {
					toSearch := line[idx:]
					if foundIdx := strings.Index(toSearch, rpart); foundIdx != -1 {
						for c := 0; c < len(rpart); c++ {
							if !utilities.Contains(mappings[lineIdx], idx+foundIdx+c) {
								mappings[lineIdx] = append(mappings[lineIdx], idx+foundIdx+c)
							}
						}
						idx = idx + foundIdx + 1
					} else {
						break
					}
				}
			}
		}

		for _, el := range mappings {
			result = result + len(el)
		}
	}
	return result
}
