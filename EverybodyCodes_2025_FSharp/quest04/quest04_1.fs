module quest04_1

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest04/test_input_01.txt"
let path = "quest04/quest04_input_01.txt"

let parseContent(lines: string array) =
    lines |> Array.map float

let calculateRatio ((g1, g2): float*float) =
    g1 / g2

let calculateCycle (gears: float array) =
    let oneTurn =
        gears
        |> Array.pairwise
        |> Array.map calculateRatio
        |> Array.reduce (*)
    oneTurn

let calcuateTarget (gears: float array) (target: float) =
    let oneTurn = calculateCycle gears
    target * oneTurn


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let gears = parseContent lines
    sprintf "%.0f" (System.Math.Floor(calcuateTarget gears 2025))
