package quest05

import "everybodycodes_2024/utilities"

func FindSpot(numOfClaps int, clapperQueue []int) (side int, pos int) {
	n := len(clapperQueue)
	leftSideInit, leftSideEnd := 0, n-1
	rightSideInit, rightSideEnd := n, (2*n)-1

	walkingSize := n * 2

	fallingPoint := (numOfClaps % walkingSize) - 1

	if fallingPoint >= leftSideInit && fallingPoint <= leftSideEnd {
		return 0, fallingPoint
	} else if fallingPoint < 0 || (fallingPoint >= rightSideInit && fallingPoint <= rightSideEnd) {
		if fallingPoint < 0 {
			fallingPoint = 0
		} else {
			fallingPoint = (walkingSize - 1) - fallingPoint
		}
		return 1, fallingPoint
	}
	return -1, -1
}

func ClapRound(round int, clappers *[][]int) {
	clapperOriginIdx := (round - 1) % len(*clappers)
	clapperQueueIdx := round % len(*clappers)
	claps := (*clappers)[clapperOriginIdx][0]

	(*clappers)[clapperOriginIdx] = (*clappers)[clapperOriginIdx][1:]

	side, pos := FindSpot(claps, (*clappers)[clapperQueueIdx])
	if side == 0 {
		// insert upfront of position
		(*clappers)[clapperQueueIdx] = utilities.InsertAt((*clappers)[clapperQueueIdx], pos, claps)
	} else {
		(*clappers)[clapperQueueIdx] = utilities.InsertAt((*clappers)[clapperQueueIdx], pos+1, claps)
	}
}
