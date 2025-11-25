package quest07

import (
	"everybodycodes_2024_Go/utilities"
	"strings"
)

type Instruction struct {
	Action      string
	Clockwise   *Instruction
	Counterwise *Instruction
}

func ReadTrack() string {
	// var fileName string = "./quest07/test_track.txt"
	var fileName string = "./quest07/track_2.txt"

	var result string = ""
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		numRows, numCols := len(fileContent), len(fileContent[0])
		top := fileContent[0]
		bottom := utilities.ReverseString(fileContent[len(fileContent)-1])
		left := ""
		right := ""
		for rowIdx := 1; rowIdx < numRows-1; rowIdx++ {
			left = utilities.StringAt(fileContent[rowIdx], 0) + left
			right = right + utilities.StringAt(fileContent[rowIdx], numCols-1)
		}
		result = top + right + bottom + left
	}

	return result
}

func Executepart2() string {
	// var fileName string = "./quest07/test_input_02.txt"
	var fileName string = "./quest07/quest07_2.txt"
	var result string = ""

	chariots := make(map[string]int)
	rules := make(map[string][]string)
	trackRules := make([]string, 0)
	track := ReadTrack()
	for cIdx := 1; cIdx < len(track); cIdx++ {
		trackRules = append(trackRules, utilities.StringAt(track, cIdx))
	}
	trackRules = append(trackRules, utilities.StringAt(track, 0))

	// fmt.Printf("%v", track)
	if fileContent, err := utilities.ReadFileAsLines(fileName); err == nil {
		for _, line := range fileContent {
			charName, parts := strings.Split(line, ":")[0], strings.Split(strings.Split(line, ":")[1], ",")
			rules[charName] = parts
			numOfLoop := 0
			startingValue := 10
			idx := 0
			for numOfLoop < 10 {
				sum := 0
				runningLoop := true
				for runningLoop {
					sign := trackRules[idx%len(trackRules)]
					if sign == "S" {
						numOfLoop++
						runningLoop = false
					}
					if sign == "S" || sign == "=" {
						sign = rules[charName][idx%len(parts)]
					}
					switch sign {
					case "+":
						startingValue++
					case "-":
						startingValue--
					}
					sum += startingValue
					idx++
				}
				chariots[charName] += sum
			}
		}

		result = SortByValue(chariots)
	}
	return result
}
