module quest02_visualization

open System.Drawing
open EverybodyCodes_2025_FSharp.Modules

//let path = "quest02/test_input_03.txt"
let path = "quest02/quest02_input_03.txt"

let parseContent(lines: string[]) =
    let (X, Y) =
        (bigint.Parse(lines[0].Replace("A=[", "").Replace("]", "").Split(",")[0]),
        bigint.Parse(lines[0].Replace("A=[", "").Replace("]", "").Split(",")[1]))
    (X, Y)

let Add (X1, Y1) (X2, Y2) =
    (X1 + X2, Y1 + Y2)

let Multiply (X1, Y1) (X2, Y2) =
    (X1 * X2 - Y1 * Y2, X1 * Y2 + Y1 * X2)

let Divide (X1, Y1) (X2, Y2) =
    (X1 / X2, Y1 / Y2)

let Cycle (X, Y) (Ax, Ay) =
    Add (Divide (Multiply (X, Y) (X, Y)) (100000I, 100000I)) (Ax, Ay)

let InRange (X, Y) =
    X >= -1000000I && X <= 1000000I && Y >= -1000000I && Y <= 1000000I

let Examinate (X, Y) =
    let rec checkCycles cycleCount currentR =
        if cycleCount > 100 then
            (true, cycleCount)
        else
            let nextR = Cycle currentR (X, Y)
            if not (InRange nextR) then
                (false, cycleCount)
            else
                checkCycles (cycleCount + 1) nextR

    let (ok, cycles) = checkCycles 1 (0I, 0I)
    (ok, cycles)

let Visualize (X, Y) (points: (((bigint*bigint)*int) seq)) =
    let normalize value =
        value * 250 / 100

    let width, height = 10001, 10001
    let bmp = new Bitmap(width, height)
    points
    |> Seq.iter(fun ((x, y), c) -> 
        let (px, py) = (int(x - X), int(y - Y))
        if c > 100 then
            bmp.SetPixel(px, py, Color.Red)
        else
            let customColor = Color.FromArgb(normalize c, 0, 0)
            bmp.SetPixel(px, py, customColor)
    )
    bmp.Save("quest02_visualize.bmp")


let Run (X, Y) =
    let (initX, initY) = (X, Y)
    let (endX, endY) = (X + 1000I, Y + 1000I)
    let points =
        seq {
            for y in [initY .. endY] do
                for x in [initX .. endX] do
                    let (ok, cycles) = Examinate (x, y)
                    yield ((x, y), cycles)
        }
    Visualize (X, Y) points |> ignore
    let result =
        points 
        |> Seq.filter(fun (_, c) -> c = 101)
        |> Seq.length
    result

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (X, Y) = parseContent lines
    Run (X, Y)
