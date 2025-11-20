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
        //printfn "Processing range %d/%d" (idx + 1) numbers.Length
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

// Optimized version ~0.1s
let findWheelValue (numbers: (bigint*bigint) array) (turns: bigint) =
    // build lock range starts
    let lockList, start, total =
        numbers
        |> Array.fold (fun (lock, start, total, forward) (from', to') ->
            let len = to' - from' + 1I
            let newTotal = total + len
            if forward then
                (lock @ [(from', to')], start, newTotal, not forward)
            else
                ((from', to') :: lock, start + 1, newTotal, not forward)
        ) ([(1I, 1I)], 0, 1I, true)
        |> fun (lock, start, total, _) -> (lock, start, total)
    let lockSize = List.length lockList
    let totalTurns = (turns + 1I) % total

    // find the value for the turn
    let rec find idx turns =
        let i = idx % lockSize
        let (from', to') = List.item i lockList
        let rngLen = to' - from' + 1I
        if turns + rngLen >= totalTurns then
            let diff = turns + rngLen - totalTurns
            if i >= start then to' - diff else from' + diff
        else find (idx + 1) (turns + rngLen)
    find start 0I

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let dialNumbers = parseContent lines
    //let wheel = constructWheel dialNumbers    
    //turnWheel wheel 202520252025I 
    findWheelValue dialNumbers 202520252025I
