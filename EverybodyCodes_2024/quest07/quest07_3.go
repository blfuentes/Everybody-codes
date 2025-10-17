package quest07

import (
	"everybodycodes_2024/utilities"
	"strings"
)

func pop[T any](slice *[]T) T {
	n := len(*slice)
	if n == 0 {
		panic("empty slice")
	}
	back := (*slice)[n-1]
	*slice = (*slice)[:n-1]
	return back
}

type pt [2]int

func neighbors(r, c, R, C int) (n []pt) {
	for _, d := range []pt{{-1, 0}, {1, 0}, {0, -1}, {0, 1}} {
		rr, cc := r+d[0], c+d[1]
		if 0 <= rr && rr < R && 0 <= cc && cc < C {
			n = append(n, pt{rr, cc})
		}
	}
	return
}

func BuildTrack() string {
	// var fileName string = "./quest07/test_track.txt"
	var fileName string = "./quest07/track_3.txt"

	t := ""
	if rows, err := utilities.ReadFileAsLines(fileName); err == nil {
		// rows := strings.Split(fileContent, "\r\n")
		R, C := len(rows), len(rows[0])
		start := pt{0, 1}
		q := []pt{start}
		seen := map[pt]bool{pt{0, 0}: true, start: true}

		for len(q) > 0 {
			curr := pop(&q)
			t += string(rows[curr[0]][curr[1]])

			for _, n := range neighbors(curr[0], curr[1], R, C) {
				if n[1] >= len(rows[n[0]]) || rows[n[0]][n[1]] == ' ' {
					continue
				}
				if v := seen[n]; !v {
					q = append(q, n)
					seen[n] = true
				}
			}
		}
	}

	return t + "S"
}

func calculate(trackRules, rules []string) int {
	knightValue := 0
	numOfLoop := 0
	startingValue := 10
	idx := 0
	for numOfLoop < 2024 {
		sum := 0
		runningLoop := true
		for runningLoop {
			sign := trackRules[idx%len(trackRules)]
			if sign == "S" {
				numOfLoop++
				runningLoop = false
			}
			if sign == "S" || sign == "=" {
				sign = rules[idx%len(rules)]
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
		knightValue += sum
	}

	return knightValue
}

func GenerateActionCombinations() []string {
	const (
		total = 11
	)
	res := make([]string, 0, 9240)
	buf := make([]rune, 0, total)

	var backtrack func(p, m, e int)
	backtrack = func(p, m, e int) {
		if len(buf) == total {
			res = append(res, string(buf))
			return
		}
		if p > 0 {
			buf = append(buf, '+')
			backtrack(p-1, m, e)
			buf = buf[:len(buf)-1]
		}
		if m > 0 {
			buf = append(buf, '-')
			backtrack(p, m-1, e)
			buf = buf[:len(buf)-1]
		}
		if e > 0 {
			buf = append(buf, '=')
			backtrack(p, m, e-1)
			buf = buf[:len(buf)-1]
		}
	}

	backtrack(5, 3, 3)
	return res
}

func Executepart3() int {
	// var fileName string = "./quest07/test_input_03.txt"
	var fileName string = "./quest07/quest07_3.txt"
	var result int = 0

	var knightValue int = 0
	trackRules := make([]string, 0)
	track := BuildTrack()
	for cIdx := 0; cIdx < len(track); cIdx++ {
		trackRules = append(trackRules, utilities.StringAt(track, cIdx))
	}

	// fmt.Printf("%v", track)
	if fileContent, err := utilities.ReadFileAsText(fileName); err == nil {
		_, parts := strings.Split(fileContent, ":")[0], strings.Split(strings.Split(fileContent, ":")[1], ",")
		knightValue = calculate(trackRules, parts)
		possibleplans := GenerateActionCombinations()
		c := make(chan int)
		for _, plan := range possibleplans {
			go func() {
				tmprules := make([]string, 0)
				for cIdx := 0; cIdx < len(plan); cIdx++ {
					tmprules = append(tmprules, utilities.StringAt(plan, cIdx))
				}
				c <- calculate(trackRules, tmprules)
			}()
		}

		for range len(possibleplans) {
			if <-c > knightValue {
				result++
			}
		}
	}

	return result
}
