module quest04_1

open System.Text.RegularExpressions
open EverybodyCodes_2026_FSharp.Modules

//let path = "quest04/test_input_01.txt"
let path = "quest04/quest04_input_01.txt"

type Warrior = {
    Id: int
    Quest: int
    Part: int
    Time: int
}

let parseContent (lines: string array) =
    let regex = Regex(@"\d+")
    let parseLine (line: string) =
        let numbers = regex.Matches line
        { 
            Id = int(numbers[0].Value);
            Quest = int(numbers[1].Value);
            Part = int(numbers[2].Value); 
            Time = int(numbers[3].Value) 
        }
    lines 
    |> Seq.map parseLine
    |> Seq.groupBy _.Id    
    |> Seq.map(fun (i, ws) ->
        (i, ws |> Seq.sumBy _.Time)
    )

let execute() =
    let lines = LocalHelper.GetContent path 2026 4 1
    let warriors = parseContent lines
    warriors 
    |> Seq.sortBy snd
    |> Seq.indexed
    |> Seq.sumBy(fun (idx, (id, w)) ->
        (idx + 1) * w
    )
