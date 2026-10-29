module quest16_1

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest16/test_input_01.txt"
let path = "quest16/quest16_input_01.txt"

let parseContent(lines: string) =
    lines.Split(",") |> Array.map int

let calculateWall (instructions: int[]) (wallLength: int) =
    instructions
    |> Array.sumBy(fun i -> 
        wallLength / i
    )

let execute() =
    let lines = (LocalHelper.GetContent path 2025 16 1)[0]
    let instructions = parseContent(lines)
    calculateWall instructions 90
