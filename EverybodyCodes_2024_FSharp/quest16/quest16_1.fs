module quest16_1

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest16/test_input_01.txt"
let path = "quest16/quest16_input_01.txt"

let parseContent (lines: string array) =
    let levers = lines[0].Split(",") |> Array.map int
    let catfaces = new Dictionary<int, char list>()
    [0..lines[2].Length-1] |> Seq.iter(fun i -> catfaces.Add(i, []))

    for _, line in Array.indexed lines[2..] do
        for colIdx, ch in Array.indexed (line.ToCharArray()) do
            if ch <> ' ' then
                catfaces[colIdx] <- catfaces[colIdx] @ [ch]
    let filtered = new Dictionary<int, char list>()
    let mutable removed = 0
    for kvp in catfaces do
        if kvp.Value.Length > 0 then
            filtered.Add(kvp.Key-removed, kvp.Value)
        else
            removed <- removed + 1

    (levers, filtered)              

let findCharacters (levers: int array) (catfaces: Dictionary<int, char list>) (round: int) =
    let positions = levers |> Seq.map(fun l -> l * round) |> List.ofSeq
    let chars = 
        seq {
            for kvp in catfaces do
                let pos = kvp.Key / 3
                let ch = kvp.Value[positions[pos]%kvp.Value.Length]
                yield ch
        }
    chars |> Seq.map string

let calculateRound (max: int) (levers: int array) (catfaces: Dictionary<int, char list>) =
    let value = 
        (findCharacters levers catfaces max)
        |> Seq.chunkBySize 3
        |> Seq.map(fun c -> String.concat "" (c))
    String.concat " " (value)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let levers, catfaces = parseContent lines
    calculateRound 100 levers catfaces
