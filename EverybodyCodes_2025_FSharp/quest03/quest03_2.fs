module quest03_2

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest03/test_input_02.txt"
let path = "quest03/quest03_input_02.txt"

let parseContent (lines: string array) =
    lines[0].Split(",") |> Array.map int

let findSmallestCrateSet (crates: int array) =
    crates
    |> Array.distinct
    |> Array.sort
    |> Array.take 20
    |> Array.sum

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let crates = parseContent lines
    findSmallestCrateSet crates
