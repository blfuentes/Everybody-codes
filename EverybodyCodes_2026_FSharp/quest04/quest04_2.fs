module quest04_2

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

// let path = "quest04/test_input_02.txt"
let path = "quest04/quest04_input_02.txt"

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

let sortByPartsThenTime (warriors: Warrior seq) =
    let partCount = Seq.length warriors
    let totalTime = warriors |> Seq.sumBy _.Time
    (-partCount, totalTime)

let execute() =
    let lines = LocalHelper.GetContent path 2026 4 2
    let warriors = parseContent lines

    warriors
    |> Seq.sortBy (fun (_, entries) -> sortByPartsThenTime entries)
    |> Seq.indexed
    |> Seq.sumBy(fun (idx, (id, w)) ->
        let time = w |> Seq.sumBy _.Time
        (idx + 1) * time
    )
