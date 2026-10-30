module quest04_3

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

// let path = "quest04/test_input_03.txt"
// let path = "quest04/test_input_03b.txt"
// let path = "quest04/test_input_03c.txt"
let path = "quest04/quest04_input_03.txt"

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

let rankBy key entries =
    entries
    |> Seq.sortBy key
    |> Seq.indexed
    |> Seq.groupBy (snd >> key)
    |> Seq.collect (fun (_, tied) ->
        let tied = Seq.toArray tied
        let position = fst tied[0] + 1
        tied |> Seq.map (fun (_, entry) -> position, entry))

let calculateChecksum (warriors: (int * Warrior seq) seq) =
    warriors
    |> Seq.collect (fun (_, entries) ->
        entries
        |> Seq.groupBy _.Quest
        |> Seq.collect (fun (_, questParts) ->
            questParts
            |> Seq.sortBy _.Part
            |> Seq.mapFold (fun totalTime entry ->
                let combinedTime = totalTime + entry.Time
                { entry with Time = combinedTime }, combinedTime) 0
            |> fst))
    |> Seq.groupBy (fun entry -> entry.Quest, entry.Part)
    |> Seq.collect (fun ((_, part), entries) ->
        entries
        |> rankBy _.Time
        |> Seq.map (fun (position, entry) ->
            entry.Id, max 0 (50 * part - position + 1)))
    |> Seq.groupBy fst
    |> Seq.map (fun (id, scores) -> id, scores |> Seq.sumBy snd)
    |> rankBy (fun (_, score) -> -score)
    |> Seq.sumBy (fun (position, (_, score)) -> int64 position * int64 score)

let execute() =
    let lines = LocalHelper.GetContent path 2026 4 3
    let warriors = parseContent lines
    calculateChecksum warriors
