module quest16_2

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest16/test_input_02.txt"
let path = "quest16/quest16_input_02.txt"

let parseContent(lines: string) =
    lines.Split(",") |> Array.map(fun v -> bigint.Parse(v))

let deconstructWall (numbers: bigint[]) =
    []


let calculateWall (instructions: bigint[]) =
    let wall = deconstructWall instructions
    wall
    |> List.reduce(fun acc i-> 
        acc * i
    )

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let instructions = parseContent(lines)
    calculateWall instructions
