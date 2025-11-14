module quest16_2

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest16/test_input_02.txt"
let path = "quest16/quest16_input_02.txt"

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

let rec gcdBig (a: int64) (b: int64) =
    if b = 0L then a
    else gcdBig b (a % b)

let rec lcmBig (a: int64) (b: int64) =
    if a = 0L || b = 0L then 0L
    else (a * b) / (gcdBig a b)

let rec listLcmBig numbers =
    match numbers with
    | [] -> 1L
    | hd :: tl -> lcmBig hd (listLcmBig tl)

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

let getScore (value: string seq) =
    let groups =
        value
        |> Seq.chunkBySize 3
        |> Seq.map(fun parts -> 
            parts
            |> Array.mapi(fun i c ->
                if i <> 1 then 
                    c 
                else 
                    ""
            ) |> Array.filter(fun c -> c <> "")
        )
        |> Seq.filter(fun v -> v.Length <> 0)
    let value = groups |> Seq.collect id |> List.ofSeq
    let coins =
        value
        |> Seq.countBy id
        |> Seq.filter(fun (_, c) -> c >= 3)
        |> Seq.sumBy(fun (_, c) -> 1L + (int64(c)-3L))
    coins

let calculateRound (max: int64) (levers: int array) (catfaces: Dictionary<int, char list>) =
    let wheels = catfaces |> Seq.chunkBySize 3
    let lengths =
        wheels
        |> Seq.map(fun a -> int64(a[0].Value.Length))
        |> List.ofSeq
    
    let uniqueValues = int(listLcmBig lengths)
    let repeats = int64((float(max)/float((uniqueValues))))
    let mutable numOfCoins = 0L
    
    for idx in [1..uniqueValues] do
        numOfCoins <- numOfCoins + getScore (findCharacters levers catfaces idx)

    numOfCoins <- numOfCoins * repeats

    for idx in [(uniqueValues*(int(repeats))+1)..(int(max))] do
        numOfCoins <- numOfCoins + getScore (findCharacters levers catfaces idx)
    numOfCoins    

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let levers, catfaces = parseContent lines
    calculateRound 202420242024L levers catfaces