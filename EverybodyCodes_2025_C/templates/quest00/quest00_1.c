#include "../headers/utils.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int execute_quest00_1() {
	/*const char* filename = "quest00/test_input_1.txt";*/
	const char* filename = "quest00/quest00_input_1.txt";
	
	char* data = read_file(filename);
	if (data == NULL) {
		printf("Failed to read file: %s\n", filename);
		return NULL;
	}

	return 0;
}