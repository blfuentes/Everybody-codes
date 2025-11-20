module quest13_2

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest13/test_input_02.txt"
let path = "quest13/quest13_input_02.txt"

let parseContent (lines: string array) =
    lines |> Array.map(fun line -> (int(line.Split("-")[0]), int(line.Split("-")[1])))

let constructWheel (numbers: (int*int) array) =
    let numberOfElements = 
        numbers
        |> Array.sumBy(fun (from', to') -> to' - from' + 1)

    let wheel = Array.create (numberOfElements + 1) 0   
    wheel[0] <- 1
    
    let mutable clockwise = 1
    let mutable counterwise = wheel.Length - 1
    for idx in 0..numbers.Length - 1 do
        let from', to' = numbers[idx]
        if idx % 2 = 0 then
            for i in from'..to' do
                wheel[clockwise] <- i
                clockwise <- clockwise + 1
        else
            for i in from'..to' do
                wheel[counterwise] <- i
                counterwise <- counterwise - 1
    wheel

let turnWheel(wheel: int array) (turns: int) =
    wheel[turns % wheel.Length]

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let dialNumbers = parseContent lines
    let wheel = constructWheel dialNumbers
    turnWheel wheel 20252025 
