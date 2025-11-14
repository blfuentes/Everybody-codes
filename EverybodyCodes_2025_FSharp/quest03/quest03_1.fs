module quest03_1

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest03/test_input_01.txt"
let path = "quest03/quest03_input_01.txt"

let parseContent (lines: string array) =
    lines[0].Split(",") |> Array.map int

let findLargestCrateSet (crates: int array) =
    crates
    |> Array.distinct
    |> Array.sum

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let crates = parseContent lines
    findLargestCrateSet crates
