module quest16_3

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest16/test_input_03.txt"
let path = "quest16/quest16_input_03.txt"

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

let findCharacters (levers: int array) (catfaces: Dictionary<int, char list>) (currentPositions: int array) (shift: int) =
    let newPositions =
        currentPositions
        |> Array.mapi (fun i p ->
            let wheel = catfaces[i]
            let leverValue = levers[i/3]
            (p + leverValue + shift + wheel.Length) % wheel.Length
        )

    let characters =
        newPositions
        |> Array.mapi (fun i p -> catfaces[i][p])
        |> Seq.map string

    (characters, newPositions)

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

type ExtremeType = Max | Min

let calculateRound (levers: int array) (wheels: Dictionary<int, char list>) (reps: int) =
    let cache = dict [ (Max, new Dictionary<string, int64>()); (Min, new Dictionary<string, int64>()) ]

    let rec recur (turns: int) (pos: int array) (opType: ExtremeType) =
        let key = (String.concat "," (pos |> Array.map string)) + "_" + (string turns)
        
        match cache[opType].TryGetValue(key) with
        | true, cachedValue -> cachedValue
        | false, _ ->
            let shifts = [-1; 0; 1]
            let pulls = shifts |> List.map (fun shift -> findCharacters levers wheels pos shift)
            let scores = pulls |> List.map (fun (pull, _) -> getScore pull)

            let finalScores =
                if turns > 1 then
                    scores
                    |> List.mapi (fun i currentScore ->
                        let _, newPos = pulls[i]
                        currentScore + recur (turns - 1) newPos opType
                    )
                else
                    scores

            let result =
                match opType with
                | Max -> finalScores |> List.max
                | Min -> finalScores |> List.min
            
            cache[opType].Add(key, result)
            result

    let initialPos = Array.zeroCreate (levers.Length*3)
    let maxScore = recur reps initialPos Max
    let minScore = recur reps initialPos Min
    (maxScore, minScore)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let levers, catfaces = parseContent lines
    let (max, min) = calculateRound levers catfaces 256
    sprintf "%d %d" max min
