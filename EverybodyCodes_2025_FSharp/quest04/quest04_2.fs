module quest04_part02

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest04/test_input_02.txt"
let path = "quest04/quest04_input_02.txt"

let parseContent(lines: string array) =
    lines |> Array.map decimal

let calculateRatio ((g1, g2): decimal*decimal) =
    g1 / g2

let calculateCycle (gears: decimal array) =
    let oneTurn =
        gears
        |> Array.pairwise
        |> Array.map calculateRatio
        |> Array.reduce (*)
    oneTurn

let calcuateTarget (gears: decimal array) (target: decimal) =
    let oneTurn = calculateCycle (gears |> Array.rev)
    target * oneTurn


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let gears = parseContent lines
    let value = System.Math.Ceiling(calcuateTarget gears (decimal 10000000000000.))
    let formatted = value.ToString("F0", System.Globalization.CultureInfo.InvariantCulture)
    sprintf "%s" formatted
