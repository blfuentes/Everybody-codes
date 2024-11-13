package quest01

import (
	"everybodycodes_2024/utilities"
)

func GetValue3(value string, modifier int) int {
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

func Executepart3() int {
	// var fileName string = "./quest01/test_input_03.txt"
	var fileName string = "./quest01/quest01_3.txt"
	var result int = 0
	if fileContent, err := utilities.ReadFileAsText(fileName); err == nil {
		chars := []rune(fileContent)
		for i := 0; i < len(chars); i = i + 3 {
			var numenemies = 0
			var firstenemy = string(chars[i])
			var secondenemy = string(chars[i+1])
			var thirdenemy = string(chars[i+2])
			if firstenemy != "x" {
				numenemies++
			}
			if secondenemy != "x" {
				numenemies++
			}
			if thirdenemy != "x" {
				numenemies++
			}
			var modifier = 0
			if numenemies > 1 {
				modifier = (numenemies - 1)
			}
			var firstpotions = GetValue3(firstenemy, modifier)
			var secondpotions = GetValue3(secondenemy, modifier)
			var thirdpotions = GetValue3(thirdenemy, modifier)
			result = result + firstpotions + secondpotions + thirdpotions
		}
	}
	return result
}
