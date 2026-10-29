module quest03_3

open System
open EverybodyCodes_2026_FSharp.Modules

//let path = "quest03/test_input_03.txt"
let path = "quest03/quest03_input_03.txt"

let parseContent (lines: string array) =
    let parseLine (line: string) =
        ((line.Split('-')[1]).Split(' ')[0], (line.Split('-')[1]).Split(' ')[1])
    lines |> Seq.map parseLine

let numOf (c: char) =
    System.Char.GetNumericValue(c) |> int

let calculateVote ((v1, v2): string * string) =
    let weights =
        Array.init v2.Length (fun i ->
            let factor = numOf v1[v1.Length - 1 - (i % v1.Length)]
            pown (int64 factor) i)

    v2
    |> Seq.rev
    |> Seq.mapi (fun i digit -> int64 (numOf digit) * weights[i])
    |> Seq.sum

let smallest ((v1, v2): string * string) =
    let digits = v2 |> Seq.rev |> Seq.map numOf |> Seq.toArray
    let weights =
        Array.init digits.Length (fun i ->
            let factor = numOf v1[v1.Length - 1 - (i % v1.Length)]
            pown (int64 factor) i)

    let target =
        digits
        |> Array.mapi (fun i digit -> int64 digit * weights[i])
        |> Array.sum

    let mutable current = target
    let mutable index = 0
    let mutable found = false

    while index < digits.Length && not found do
        while index < digits.Length && digits[index] = 0 do
            digits[index] <- 9
            current <- current + 9L * weights[index]
            index <- index + 1

        if index < digits.Length then
            digits[index] <- digits[index] - 1
            current <- current - weights[index]
            index <- 0

            if current = target then
                found <- true

    if found then 0L else target

let execute() =
    let lines = LocalHelper.GetContent path 2026 3 3
    let votes = parseContent lines
    votes
    |> Seq.toArray
    |> Array.Parallel.map smallest
    |> Array.sum
