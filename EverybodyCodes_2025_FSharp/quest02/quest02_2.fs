module quest02_2

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest02/test_input_02.txt"
let path = "quest02/quest02_input_02.txt"

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
            true
        else
            let nextR = Cycle currentR (X, Y)
            if not (InRange nextR) then
                false
            else
                checkCycles (cycleCount + 1) nextR

    if checkCycles 1 (0I, 0I) then
        Some (X, Y)
    else
        None

let Run (X, Y) =
    let (initX, initY) = (X, Y)
    let (endX, endY) = (X + 1000I, Y + 1000I)
    let points =
        seq {
            for y in [initY ..10I.. endY] do
                for x in [initX ..10I..endX] do
                    match Examinate (x, y) with
                    | Some (a, b) -> 
                        yield (a, b)
                    | None -> 
                        ()
        }
    points |> Seq.length

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (X, Y) = parseContent lines
    Run (X, Y)    
