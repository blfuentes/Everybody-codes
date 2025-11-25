package quest07

import (
	"sort"
	"strings"
)

func SortByValue(chariots map[string]int) string {
	type kv struct {
		k string
		v int
	}
	arr := make([]kv, 0, len(chariots))
	for k, v := range chariots {
		arr = append(arr, kv{k, v})
	}

	sort.Slice(arr, func(i, j int) bool {
		if arr[i].v == arr[j].v {
			return arr[i].k < arr[j].k // tie-breaker: lexical order
		}
		return arr[i].v > arr[j].v // highest value first
	})

	var sb strings.Builder
	for _, e := range arr {
		sb.WriteString(e.k)
	}
	return sb.String()
}
