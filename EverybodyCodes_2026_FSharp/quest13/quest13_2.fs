module quest13_2

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

// let path = "quest13/test_input_02.txt"
let path = "quest13/quest13_input_02.txt"

type Signal = {
    Id: int
    Hands: int
    Direction: string
    Current: int
    Move: int
}

let extractNumbers(value: string) =
    Regex.Matches(value, @"\d+") 
    |> Seq.map(fun m -> int(m.Value))
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

let calculateRotation(signal: Signal) =
    let step =
        if signal.Move = 0 then 0
        elif signal.Direction = "CW" then 1
        elif signal.Direction = "CCW" then -1
        else invalidArg "signal" $"Unknown direction: {signal.Direction}"

    (int64 signal.Current, int64 signal.Current + int64 step)

let countCrossings (signals: Signal list) =
    let movingSignals = signals |> Array.ofList
    let mutable crossings = 0L

    while movingSignals |> Array.exists (fun signal -> signal.Move > 0) do
        for i in 0 .. movingSignals.Length - 2 do
            for j in i + 1 .. movingSignals.Length - 1 do
                let first = movingSignals[i]
                let second = movingSignals[j]
                let firstStart, firstEnd = calculateRotation first
                let secondStart, secondEnd = calculateRotation second
                let firstSize = int64 first.Hands
                let secondSize = int64 second.Hands

                let scale = firstSize * secondSize
                let difference = firstStart * secondSize - secondStart * firstSize
                let start = (difference + scale) % scale
                let change =
                    (firstEnd - firstStart) * secondSize
                    - (secondEnd - secondStart) * firstSize

                if change > 0L then
                    crossings <- crossings + (start + change) / scale
                elif change < 0L then
                    crossings <- crossings + (-change + (scale - start) % scale) / scale

        for i in 0 .. movingSignals.Length - 1 do
            let signal = movingSignals[i]
            if signal.Move > 0 then
                let _, ending = calculateRotation signal
                let size = int64 signal.Hands
                movingSignals[i] <-
                    { signal with
                        Current = int ((ending + size) % size)
                        Move = signal.Move - 1 }

    crossings

let execute() =
    let lines = LocalHelper.GetContent path 2026 13 2
    let signals = parseContent lines
    signals
    |> Array.sumBy countCrossings