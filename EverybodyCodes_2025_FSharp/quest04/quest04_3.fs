module quest04_3

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest04/test_input_03.txt"
let path = "quest04/quest04_input_03.txt"

let parseContent(lines: string array) =
    lines |> Array.map(fun s -> 
        let parts = s.Split("|")
        if parts.Length = 1 then
            (float parts[0], float parts[0])
        else
            (float parts[0], float parts[1])
    )

let calculateRatio (((_, g1o), (g2i, _)): ((float*float)*(float*float))) =
    g1o / g2i

let calculateCycle (gears: (float*float) array) =
    let oneTurn =
        gears
        |> Array.pairwise
        |> Array.map calculateRatio
        |> Array.reduce (*)
    oneTurn

let calcuateTarget (gears: (float*float) array) (target: float) =
    let oneTurn = calculateCycle gears
    target * oneTurn


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let gears = parseContent lines
    sprintf "%.0f" (System.Math.Floor(calcuateTarget gears 100))
