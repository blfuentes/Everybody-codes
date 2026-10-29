module quest03_3

open System
open EverybodyCodes_2026_FSharp.Modules

//let path = "quest03/test_input_03.txt"
let path = "quest03/quest03_input_03.txt"

let parseContent (lines: string array) =
    let parseLine (line: string) =
        ((line.Split('-')[1]).Split(' ')[0], (line.Split('-')[1]).Split(' ')[1])
    lines |> Seq.map parseLine

let numOf (c: char) =
    System.Char.GetNumericValue(c) |> int

let calculateVote ((v1, v2): string * string) =
    [0..v2.Length-1]
    |> Seq.sumBy(fun i ->
        let index = i % v1.Length
        let v =
            v1.ToCharArray()
            |> Seq.rev
            |> Seq.item(index)
        int64 (float((numOf v2[v2.Length - i - 1])) * Math.Pow((numOf(v), i)))
    )

let smallest ((v1, v2): string * string) =
    let voteValue = calculateVote (v1, v2)
    let rec foundSmallest current =
        let value = calculateVote (v1, current.ToString())
        if value = voteValue then
            0L
        elif current > 0 then
            let prev = calculateVote (v1, (current-1).ToString())
            foundSmallest (current-1)
        else 
            voteValue
    foundSmallest (int(v2) - 1)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let votes = parseContent lines
    votes |> Seq.sumBy smallest
