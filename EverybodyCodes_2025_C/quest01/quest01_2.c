#include "../headers/utils.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

typedef struct Operator
{
	int distance;
	char symbol;
} Operator;

static Operator build_operator(char* op) {
	Operator operator = { 0 };
	char* trimmed = trim(op);
	operator.distance = atoi(&trimmed[1]);
	operator.symbol = trimmed[0];
	return operator;
}

char* execute_quest01_2() {
	//const char* filename = "quest01/test_input_2.txt";
	const char* filename = "quest01/quest01_input_2.txt";

	char* data = read_file(filename);
	if (data == NULL) {
		printf("Failed to read file: %s\n", filename);
		return NULL;
	}

	int line_count = 0;
	char** lines = split_string(data, '\n', &line_count);

	// find names
	int name_count = 0;
	char** names = split_string(lines[0], ',', &name_count);

	// find operators
	int operators_count = 0;
	char** operators = split_string(lines[2], ',', &operators_count);
	Operator* ops = (Operator*)malloc(operators_count * sizeof(Operator));

	for (int i = 0; i < operators_count; i++) {
		ops[i] = build_operator(operators[i]);
	}

	int current_position = 0;
	for (int i = 0; i < operators_count; i++) {
		Operator op = ops[i];
		if (op.symbol == 'L') {
			current_position = mod(current_position - op.distance, name_count);
		}
		else {
			current_position = mod(current_position + op.distance, name_count);
		}
	}

	char* result_name = strdup(names[current_position]);

	// free memory
	for (int i = 0; i < line_count; i++) {
		free(lines[i]);
	}
	for (int i = 0; i < name_count; i++) {
		free(names[i]);
	}
	for (int i = 0; i < operators_count; i++) {
		free(operators[i]);
	}
	// free allocated arrays
	free(operators);
	free(names);
	free(lines);
	free(data);
	free(ops);

	return result_name;
}