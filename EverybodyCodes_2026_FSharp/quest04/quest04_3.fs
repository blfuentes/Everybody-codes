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
    let combinedTimes = query {
        for (_, entries) in warriors do
        for entry in entries do
        let totalTime = query {
            for previous in entries do
            where (previous.Quest = entry.Quest && previous.Part <= entry.Part)
            sumBy previous.Time
        }
        select { entry with Time = totalTime }
    }

    let points = query {
        for entry in combinedTimes do
        groupBy (entry.Quest, entry.Part) into parts
        for (position, entry) in rankBy _.Time parts do
        select (entry.Id, max 0 (50 * entry.Part - position + 1))
    }

    let totalScores = query {
        for (id, score) in points do
        groupValBy score id into scores
        let total = query {
            for score in scores do
            sumBy score
        }
        select (scores.Key, total)
    }

    totalScores
    |> rankBy (fun (_, score) -> -score)
    |> Seq.sumBy (fun (position, (_, score)) -> int64 position * int64 score)

let calculateChecksumPipeline (warriors: (int * Warrior seq) seq) =
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
    let useQuery = true
    let lines = LocalHelper.GetContent path 2026 4 3
    let warriors = parseContent lines

    // let (value, time) = duration(fun () -> calculateChecksumPipeline warriors)
    // printfn "Pipeline: %s" (LocalHelper.ms time)
    // let (value, time) = duration(fun () -> calculateChecksum warriors)
    // printfn "Query: %s" (LocalHelper.ms time)

    if useQuery then
        calculateChecksum warriors
    else
        calculateChecksumPipeline warriors
