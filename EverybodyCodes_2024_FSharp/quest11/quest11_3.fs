module quest11_3

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest11/test_input_03.txt"
let path = "quest11/quest11_input_03.txt"

let transformations = new Dictionary<string, string list>()

let parseContent(lines: string array) =
    lines
    |> Array.iter(fun line ->
        let origin, target = line.Split(":")[0], (line.Split(":")[1]).Split(",")
        transformations.Add(origin, target |> Array.toList)
    )

let updateState (currentState: Dictionary<string, int64>) =
    let newState = Dictionary<string, int64>()
    for kvp in transformations do
        for target in kvp.Value do
            newState[target] <- 0L
            
    for KeyValue(source, count) in currentState do
        if transformations.ContainsKey(source) then
            for newborn in transformations[source] do
                newState[newborn] <- newState[newborn] + count
            
    newState

let transformation (target: int) (initialString: string) =
    let mutable currentState = Dictionary<string, int64>()
    currentState.Add(initialString, 1L)

    for step in 1..target do
        currentState <- updateState currentState

    currentState.Values |> Seq.sum

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent(lines)
    let possibleStarters =
        transformations.Keys |> Seq.toList
    let results = 
        possibleStarters
        |> List.mapi(fun i starter ->
            let length = transformation 20 starter
            (starter, length)
        )
        |> List.sortByDescending(fun (_, targets) -> targets)
    let max = results.Head |> snd
    let min = results |> List.last |> snd
    max - min
