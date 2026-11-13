module quest14_1

open EverybodyCodes_2026_FSharp.Modules

//let path = "quest14/test_input_01.txt"
let path = "quest14/quest14_input_01.txt"

let countBindings (grid: string array) =
    if grid.Length = 0 || grid[0].Length = 0 then
        invalidArg "grid" "The window must not be empty."

    let height = grid.Length
    let width = grid[0].Length

    let mutable bindings = 0

    for row in 0 .. height - 1 do
        for column in 0 .. width - 1 do
            if row = 0 || grid[row][column] <> grid[row - 1][column] then
                bindings <- bindings + 1

            if column = 0 || grid[row][column] <> grid[row][column - 1] then
                bindings <- bindings + 1

            if row = height - 1 then
                bindings <- bindings + 1

            if column = width - 1 then
                bindings <- bindings + 1

    bindings

let execute() =
    let lines = LocalHelper.GetContent path 2026 14 1
    countBindings lines
