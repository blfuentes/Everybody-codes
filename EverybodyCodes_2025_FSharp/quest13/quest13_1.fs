module quest13_1

open EverybodyCodes_2025_FSharp.Modules

let path = "quest13/test_input_01.txt"
//let path = "quest13/quest13_input_01.txt"

let parseContent (lines: string array) =
    lines |> Array.map int

let constructWheel (numbers: int array) =
    let wheel = Array.create (Array.length numbers + 1) 0
    wheel[0] <- 1
    let mutable clockwise = 1
    let mutable counterwise = numbers.Length
    for idx in 0..numbers.Length - 1 do
        if idx % 2 = 0 then
            wheel[clockwise] <- numbers[idx]
            clockwise <- clockwise + 1
        else
            wheel[counterwise] <- numbers[idx]
            counterwise <- counterwise - 1
    wheel

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let dialNumbers = parseContent lines
    let wheel = constructWheel dialNumbers
    wheel.Length
