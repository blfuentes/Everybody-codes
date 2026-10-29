module quest03_1

open System
open EverybodyCodes_2026_FSharp.Modules

//let path = "quest03/test_input_01.txt"
let path = "quest03/quest03_input_01.txt"

let parseContent (lines: string array) =
    let parseLine (line: string) =
        ((line.Split('-')[1]).Split(' ')[0], (line.Split('-')[1]).Split(' ')[1])
    lines |> Seq.map parseLine

let numOf (c: char) =
    System.Char.GetNumericValue(c) |> int

let calculateVote ((v1, v2): string * string) =
    [0..3]
    |> Seq.sumBy(fun i ->
        (numOf v2[v2.Length - i - 1]) * (int (Math.Pow((numOf(v1[v1.Length - i - 1]), i))))
    )

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let votes = parseContent lines
    votes |> Seq.sumBy calculateVote
