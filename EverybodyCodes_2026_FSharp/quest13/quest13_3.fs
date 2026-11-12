module quest13_3

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions
open Microsoft.FSharp.Core.Operators.Checked

// let path = "quest13/test_input_03.txt"
let path = "quest13/quest13_input_03.txt"

type Signal = {
    Id: int
    Hands: int64
    Direction: string
    Current: int64
    Move: int64
}

let extractNumbers(value: string) =
    Regex.Matches(value, @"\d+") 
    |> Seq.map(fun m -> int64(m.Value))
    |> Array.ofSeq

let parseContent (content: string array) =
    let parts = content |> Array.chunkBySize 5

    let parsePortal (lines: string array) =
        let lines = lines |> Array.take 4
        let hands= extractNumbers(lines[0])
        let directions = 
            (lines[1].Split(" ")[1]).Split(":")
        let current = extractNumbers(lines[2])
        let moves = extractNumbers(lines[3])

        let numOfSignals = hands.Length
        [
            for i in 0..numOfSignals-1 do
                yield {
                    Id = i;
                    Hands = hands[i];
                    Direction = directions[i];
                    Current = current[i];
                    Move = moves[i]
                }
        ]
    parts |> Array.map parsePortal

let direction (signal: Signal) =
    match signal.Direction with
    | "CW" -> 1L
    | "CCW" -> -1L
    | _ -> invalidArg "signal" $"Unknown direction: {signal.Direction}"

let countCrossings (signals: Signal list) =
    let countPair (first: Signal) (second: Signal) =
        let firstSize = int64 first.Hands
        let secondSize = int64 second.Hands
        let scale = firstSize * secondSize
        let firstVelocity = direction first * secondSize
        let secondVelocity = direction second * firstSize
        let mutable difference = int64 first.Current * secondSize - int64 second.Current * firstSize
        let mutable meetings = 0L
        let sharedTime = min first.Move second.Move

        let countInterval duration velocity =
            let remainder = difference % scale
            let start = if remainder < 0L then remainder + scale else remainder
            let change = velocity * duration
            if change > 0L then
                meetings <- meetings + (start + change) / scale
            elif change < 0L then
                meetings <- meetings + (-change + (scale - start) % scale) / scale
            difference <- difference + change

        countInterval sharedTime (firstVelocity - secondVelocity)
        if first.Move > second.Move then
            countInterval (first.Move - sharedTime) firstVelocity
        elif second.Move > first.Move then
            countInterval (second.Move - sharedTime) (-secondVelocity)
        meetings

    let hands = signals |> List.toArray
    [ for i in 0 .. hands.Length - 2 do
        for j in i + 1 .. hands.Length - 1 do
            yield countPair hands[i] hands[j] ]
    |> List.fold (+) 0L

let execute() =
    let lines = LocalHelper.GetContent path 2026 13 3
    let signals = parseContent lines
    signals
    |> Array.fold (fun total portal -> total + countCrossings portal) 0L
