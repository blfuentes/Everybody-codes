package quest06

import "everybodycodes_2024/utilities"

type Node struct {
	Parent  *Node
	Name    string
	Leaves  []*Node
	IsFruit bool
}

func FindPowerfull(paths []string) string {
	lengthsPaths := make(map[int][]string)
	for _, p := range paths {
		l := len(p)
		_, exists := lengthsPaths[l]
		if !exists {
			lengthsPaths[l] = []string{p}
		} else {
			lengthsPaths[l] = append(lengthsPaths[l], p)
		}
	}
	for _, v := range lengthsPaths {
		tmp := len(v)
		if tmp == 1 {
			return v[0]
		}
	}
	return ""
}

func ShortcutFruitPath(fruit Node) string {
	result := fruit.Name

	isCycle := false
	parentReached := false
	foundParents := make(map[string]bool)
	for fruit.Parent != nil && !isCycle {
		_, isCycle = foundParents[fruit.Parent.Name]
		if !isCycle {
			result = utilities.StringAt(fruit.Parent.Name, 0) + result
			foundParents[fruit.Parent.Name] = true
			fruit = *fruit.Parent
		}
	}
	parentReached = fruit.Name == "RR"
	if parentReached {
		return result
	} else {
		return ""
	}
}
