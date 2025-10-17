package quest06

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
