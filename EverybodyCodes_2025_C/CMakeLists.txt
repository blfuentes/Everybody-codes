cmake_minimum_required(VERSION 3.10)

project(EverybodyCodes_2025_C)

set(CMAKE_C_STANDARD 23)
set(CMAKE_C_STANDARD_REQUIRED ON)
set(CMAKE_C_EXTENSIONS OFF)

add_executable(EverybodyCodes_2025_C
  EverybodyCodes_2025_C.c
  quest01/quest01_1.c
  quest01/quest01_2.c
  quest01/quest01_3.c
)

# Add C23 compiler flags for GCC/Clang
if(CMAKE_C_COMPILER_ID STREQUAL "GNU" OR CMAKE_C_COMPILER_ID STREQUAL "Clang")
    target_compile_options(EverybodyCodes_2025_C PRIVATE -std=c2x)
endif()

# Portable pthread handling:
find_package(Threads REQUIRED)
target_link_libraries(EverybodyCodes_2025_C PRIVATE Threads::Threads)