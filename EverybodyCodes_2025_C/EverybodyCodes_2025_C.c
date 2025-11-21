#include <stdio.h>
#include "quest01/quest01.h"

int main(void) {

    // Quest 01
    int result01_1 = execute_quest01_1();
	printf("Result of Quest 01 part 1: %d\n", result01_1);
	int result01_2 = execute_quest01_2();
	printf("Result of Quest 01 part 2: %d\n", result01_2);
	int result01_3 = execute_quest01_3();
	printf("Result of Quest 01 part 3: %d\n", result01_3);

    return 0;
}