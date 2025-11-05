module quest02_part01

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest02/test_input_01.txt"
let path = "quest02/quest02_input_01.txt"

let parseContent(lines: string[]) =
    let (X, Y) =
        (int(lines[0].Replace("A=[", "").Replace("]", "").Split(",")[0]),
        int(lines[0].Replace("A=[", "").Replace("]", "").Split(",")[1]))
    (X, Y)

let Add (X1, Y1) (X2, Y2) =
    (X1 + X2, Y1 + Y2)

let Multiply (X1, Y1) (X2, Y2) =
    (X1 * X2 - Y1 * Y2, X1 * Y2 + Y1 * X2)

let Divide (X1, Y1) (X2, Y2) =
    (X1 / X2, Y1 / Y2)

let Cycle (X, Y) (Ax, Ay) =
    Add (Divide (Multiply (X, Y) (X, Y)) (10, 10)) (Ax, Ay)

let Run (X, Y) numOfCycles =
    [1.. numOfCycles]
    |> List.fold (fun acc _ -> Cycle acc (X, Y)) (0, 0)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (X, Y) = parseContent lines
    let (x, y) = Run (X, Y) 3
    sprintf "[%i,%i]" x y
