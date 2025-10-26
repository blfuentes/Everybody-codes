module quest16_part02

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

let path = "quest16/test_input_02.txt"
//let path = "quest16/quest16_input_01.txt"

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

let findCharacters (levers: int array) (catfaces: Dictionary<int, char list>) (round: int64) =
    let positions = levers |> Seq.map(fun l -> int64(l) * round) |> List.ofSeq
    let chars = 
        seq {
            for kvp in catfaces do
                let pos = kvp.Key / 3
                let position = int(positions[pos]%int64(kvp.Value.Length))
                let ch = kvp.Value[position]
                yield ch
        }
    chars |> Seq.map string

let calculateRound (max: int64) (levers: int array) (catfaces: Dictionary<int, char list>) =
    let mutable numOfCoints = 0L
    for r in [1L..max] do
        let values = (findCharacters levers catfaces r)
        let value = String.concat "" (findCharacters levers catfaces r)
        let coins =
            value
            |> Seq.countBy id
            |> Seq.filter(fun (_, c) -> c >= 3)
            |> Seq.sumBy(fun (_, c) -> 1L + (int64(c)-3L))
        numOfCoints <- numOfCoints + coins
        printfn "pull %A: %A" r numOfCoints
    //    printfn "pull %d %s" r value
        //printfn "pull %d %s: %d coins" r value coins

    //printfn "%s: %d coins" value coins
    //let value = 
    //    (findCharacters levers catfaces max)
    //    |> Seq.chunkBySize 3
    //    |> Seq.map(fun c -> String.concat "" (c))
    //String.concat " " (value)
    //let text = String.concat "" (value)
    //printfn "%s" text
    //value
    //|> Seq.countBy id
    //|> Seq.filter(fun (_, c) -> c >= 3)
    //|> Seq.sumBy(fun (_, c) -> 1 + (c-3))

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let levers, catfaces = parseContent lines
    calculateRound 10L levers catfaces

    //calculateRound 202420242024L levers catfaces
