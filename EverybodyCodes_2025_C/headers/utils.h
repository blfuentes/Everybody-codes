#ifndef UTILS_H
#define UTILS_H

// IO Readers
char* read_file(const char* filename);

// String utilities
char** split_string(const char* str, char delimiter, int* out_count);

char* trim(char* str);

// Math utilities
int mod(int a, int b);

int max(int a, int b);

int min(int a, int b);

#endif // UTILS_H