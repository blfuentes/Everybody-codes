module quest13_3

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest13/test_input_03.txt"
let path = "quest13/quest13_input_03.txt"

let parseContent (lines: string array) =
    lines |> Array.map(fun line -> (bigint.Parse(line.Split("-")[0]), bigint.Parse(line.Split("-")[1])))


// Brute force...~2min
let constructWheel (numbers: (bigint*bigint) array) =
    let numberOfElements = 
        numbers
        |> Array.sumBy(fun (from', to') -> to' - from' + 1I)

    let wheel = new Dictionary<bigint, bigint>()
    wheel.Add(0I, 1I)

    let mutable clockwise = 1
    let mutable counterwise = numberOfElements
    for idx in 0..numbers.Length - 1 do
        printfn "Processing range %d/%d" (idx + 1) numbers.Length
        let from', to' = numbers[idx]
        if idx % 2 = 0 then
            for i in from'..to' do
                wheel[clockwise] <- i
                clockwise <- clockwise + 1
        else
            for i in from'..to' do
                wheel[counterwise] <- i
                counterwise <- counterwise - 1I
    wheel

let turnWheel(wheel: Dictionary<bigint, bigint>) (turns: bigint) =
    let place = turns % (bigint wheel.Count)
    wheel[place]

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let dialNumbers = parseContent lines
    let wheel = constructWheel dialNumbers    
    turnWheel wheel 202520252025I 
