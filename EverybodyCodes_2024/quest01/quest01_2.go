package quest01

import (
	"everybodycodes_2024/utilities"
)

func GetValue(value string, modifier int) int {
	var result = 0
	switch value {
	case "A":
		result = 0 + modifier
	case "B":
		result = 1 + modifier
	case "C":
		result = 3 + modifier
	case "D":
		result = 5 + modifier
	case "x":
		result = 0
	}

	return result
}

func Executepart2() int {
	// var fileName string = "./quest01/test_input_02.txt"
	var fileName string = "./quest01/quest01_2.txt"
	var result int = 0
	if fileContent, err := utilities.ReadFileAsText(fileName); err == nil {
		chars := []rune(fileContent)
		for i := 0; i < len(chars); i = i + 2 {
			var modifier = 0
			var firstenemy = string(chars[i])
			var secondenemy = string(chars[i+1])
			if firstenemy != "x" && secondenemy != "x" {
				modifier = 1
			}
			var firstpotions = GetValue(firstenemy, modifier)
			var secondpotions = GetValue(secondenemy, modifier)
			// fmt.Printf("Working enemy %s with value %d and enemy %s with value %d\n",
			// 	firstenemy, firstpotions, secondenemy, secondpotions)
			result = result + firstpotions + secondpotions
		}
	}
	return result
}
