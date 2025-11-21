#define _POSIX_C_SOURCE 199309L

#include <stdio.h>
#include <stdlib.h>
#include <time.h>

#include "headers/quests.h"

static char* format_elapsed_time(struct timespec start, struct timespec end) {
    static char buffer[32];
    long seconds = end.tv_sec - start.tv_sec;
    long nanoseconds = end.tv_nsec - start.tv_nsec;
    
    // Handle nanosecond underflow
    if (nanoseconds < 0) {
        seconds--;
        nanoseconds += 1000000000L;
    }
    
    int minutes = (int)(seconds / 60);
    int secs = (int)(seconds % 60);
    int milliseconds = (int)(nanoseconds / 1000000);
    int microseconds = (int)((nanoseconds % 1000000) / 1000);
    
    snprintf(buffer, sizeof(buffer), "%02d:%02d:%03d.%03d", minutes, secs, milliseconds, microseconds);
    return buffer;
}

int main(void) {
    struct timespec start, end;

    // Quest 01
    clock_gettime(CLOCK_MONOTONIC, &start);
    char* result01_1 = execute_quest01_1();
    clock_gettime(CLOCK_MONOTONIC, &end);
    printf("Result of Quest 01 part 1: %s in %s time (mm:ss:ms.μs)\n", result01_1, format_elapsed_time(start, end));

    clock_gettime(CLOCK_MONOTONIC, &start);
    char* result01_2 = execute_quest01_2();
    clock_gettime(CLOCK_MONOTONIC, &end);
    printf("Result of Quest 01 part 2: %s in %s time (mm:ss:ms.μs)\n", result01_2, format_elapsed_time(start, end));

    clock_gettime(CLOCK_MONOTONIC, &start);
    char* result01_3 = execute_quest01_3();
    clock_gettime(CLOCK_MONOTONIC, &end);
    printf("Result of Quest 01 part 3: %s in %s time (mm:ss:ms.μs)\n", result01_3, format_elapsed_time(start, end));

	free(result01_3);
	free(result01_2);
    free(result01_1);

    return 0;
}