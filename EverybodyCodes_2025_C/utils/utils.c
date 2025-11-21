#include "../headers/utils.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>

// IO Readers
char* read_file(const char* filename) {
    FILE* file = fopen(filename, "r");
    if (!file) {
        perror("Failed to open file");
        return NULL;
    }

    // Move to end to get size
    fseek(file, 0, SEEK_END);
    long size = ftell(file);
    rewind(file);

    // Allocate buffer (+1 for null terminator)
    char* buffer = (char*)malloc(size + 1);
    if (!buffer) {
        perror("Memory allocation failed");
        fclose(file);
        return NULL;
    }

    // Read file into buffer
    size_t read_size = fread(buffer, 1, size, file);
    buffer[read_size] = '\0'; // Null-terminate

    fclose(file);
    return buffer;
}

// String trimming - removes leading and trailing whitespace
char* trim(char* str) {
    if (!str) return NULL;
    
    // Trim leading space
    while(isspace((unsigned char)*str)) str++;
    
    if(*str == 0) return str;
    
    // Trim trailing space
    char* end = str + strlen(str) - 1;
    while(end > str && isspace((unsigned char)*end)) end--;
    
    // Write new null terminator
    end[1] = '\0';
    
    return str;
}

// char utilities
char** split_string(const char* str, char delimiter, int* out_count) {
    int count = 0;
    const char* temp = str;
    while (*temp) {
        if (*temp == delimiter) count++;
        temp++;
    }
    count++; // for the last segment
    char** result = (char**)malloc(count * sizeof(char*));
    if (!result) {
        perror("Memory allocation failed");
        *out_count = 0;
        return NULL;
    }
    int index = 0;
    const char* start = str;
    temp = str;
    while (*temp) {
        if (*temp == delimiter) {
            size_t len = temp - start;
            result[index] = (char*)malloc(len + 1);
            if (!result[index]) {
                perror("Memory allocation failed");
                // Free previously allocated memory
                for (int j = 0; j < index; j++) free(result[j]);
                free(result);
                *out_count = 0;
                return NULL;
            }
            strncpy(result[index], start, len);
            result[index][len] = '\0';
            index++;
            start = temp + 1;
        }
        temp++;
    }
    // Last segment
    size_t len = temp - start;
    result[index] = (char*)malloc(len + 1);
    if (!result[index]) {
        perror("Memory allocation failed");
        for (int j = 0; j < index; j++) free(result[j]);
        free(result);
        *out_count = 0;
        return NULL;
    }
    strncpy(result[index], start, len);
    result[index][len] = '\0';
    *out_count = count;
    return result;
}

// Module that works with negative movements
int mod(int a, int b) {
    int r = a % b;
    return r < 0 ? r + b : r;
}

int max(int a, int b) {
    return (a > b) ? a : b;
}

int min(int a, int b) {
    return (a < b) ? a : b;
}