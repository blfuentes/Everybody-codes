module quest08_part02

open System
open EverybodyCodes_2025_FSharp.Modules

let path = "quest08/test_input_02.txt"
//let path = "quest08/quest08_input_02.txt"

let parseContent (lines: string) =
    lines.Split(",") |> Array.map int

let findCenterTimes (nails: int array) (numOfNails: int) =
    nails
    |> Array.pairwise
    |> Array.groupBy(fun (a, b) ->
        abs(a - b)
    )
    |> Array.filter(fun (a, _) -> a > 0)
    |> Array.map (fun (a, b) -> b.Length)
    |> Array.sum

let generateCirclePoints (n: int) (r: float) : (int * (float * float)) list =
    [ for i in 0 .. n - 1 ->
        let angle = 2.0 * Math.PI * float i / float n
        let x = r * cos angle
        let y = r * sin angle
        (i+1, (x, y)) ]

//let buildMap(numOfNails: int) =
//    let numOfSections = 4
//    let nailsForSection = numOfNails / numOfSections
//    let mutatedNails =
//        [1..numOfNails]
//        |> Seq.map(fun n ->
//            let section = (n - 1) / numOfSections

//            (n, (n - 1) / (nailsForSection))
//        )
//    mutatedNails

let findCollisions (points: (int * (float * float)) list) =
    let mapping = points |> dict
    0

let printCircle(points: (float * float) list) =
    points
    |> List.iteri (fun i (x, y) ->
    printfn "Point %2d: (%.4f, %.4f)" (i + 1) x y)

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let nails = parseContent lines
    let numOfNails = 12
    //let numOfNails = 256
    //let mapOfNails = buildMap numOfNails
    let circle = generateCirclePoints numOfNails (float(numOfNails) / 4.)
    //printCircle circle
    //printfn "num of nails %A" mapOfNails
    //findCenterTimes nails numOfNails
