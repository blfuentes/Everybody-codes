package utilities

import (
	"fmt"
	"os"
	"strings"

	"github.com/fatih/color"
)

func ReadFileAsText(path string) (string, error) {
	var fileName string = path
	file, err := os.ReadFile(fileName)
	if err != nil {
		fmt.Printf("Cannot read file %s", fileName)

		return "", err
	}
	fileContent := string(file)

	return fileContent, err
}

func ReadFileAsLines(path string) ([]string, error) {
	var fileName string = path
	file, err := os.ReadFile(fileName)
	if err != nil {
		fmt.Printf("Cannot read file %s", fileName)

		return nil, err
	}
	fileContent := string(file)

	return strings.Split(fileContent, "\r\n"), err
}

func ReverseString(s string) string {
	runes := []rune(s)
	size := len(runes)
	for i, j := 0, size-1; i < size>>1; i, j = i+1, j-1 {
		runes[i], runes[j] = runes[j], runes[i]
	}
	return string(runes)
}

// Function to check if an array contains an element
func Contains[T comparable](arr []T, target T) bool {
	for _, value := range arr {
		if value == target {
			return true
		}
	}
	return false
}

func PrintMatrix(matrix *[][]int) {
	for rowIdx := 0; rowIdx < len(*matrix); rowIdx++ {
		for colIdx := 0; colIdx < len((*matrix)[rowIdx]); colIdx++ {
			fmt.Printf("%d", (*matrix)[rowIdx][colIdx])
		}
		fmt.Println()
	}
}

func PrintMatrixWithColors(matrix [][]int) {
	colors := []func(a ...interface{}) string{
		color.New(color.BgBlack).SprintFunc(),
		color.New(color.BgBlue).SprintFunc(),
		color.New(color.BgGreen).SprintFunc(),
		color.New(color.BgHiMagenta).SprintFunc(),
		color.New(color.BgHiRed).SprintFunc(),
		color.New(color.BgHiWhite).SprintFunc(),
		color.New(color.FgBlue).SprintFunc(),
		color.New(color.BgHiYellow).SprintFunc(),
		color.New(color.FgGreen).SprintFunc(),
		color.New(color.BgBlack).SprintFunc(),
	}

	for rowIdx, row := range matrix {
		for colIdx, value := range row {
			colorIdx := matrix[rowIdx][colIdx]
			coloredValue := colors[colorIdx](value)
			fmt.Print(coloredValue, " ")
		}
		fmt.Println()
	}
}

func IsInBoundaries(row, col, maxrows, maxcols int) bool {
	return row >= 0 && row < maxrows && col >= 0 && col < maxcols
}

type Int64Array []int64

func (a Int64Array) Len() int           { return len(a) }
func (a Int64Array) Swap(i, j int)      { a[i], a[j] = a[j], a[i] }
func (a Int64Array) Less(i, j int) bool { return a[i] < a[j] }
