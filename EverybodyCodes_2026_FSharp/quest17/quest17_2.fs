module quest17_2

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

//let path = "quest17/test_input_02.txt"
let path = "quest17/quest17_input_02.txt"

let squares = HashSet<(int*int)>()
let mutable(maxRow,maxCol) = (0, 0)
let parseContent(lines: string array) =
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    
    for r in 0..maxRow do
        for c in 0..maxCol do
            if lines[r][c] = '#' then
                squares.Add((r, c)) |> ignore

let dirs = [
    (0, 1); (0, -1); (-1, 0); (1, 0)
]

let countContinuousPath() =
    squares
    |> Seq.sumBy (fun (row, col) ->
        dirs
        |> List.sumBy (fun (dr, dc) ->
            if squares.Contains(row + dr, col + dc) then 0 else 1))

let execute() =
    let lines = LocalHelper.GetContent path 2026 17 2
    parseContent lines
    countContinuousPath()
