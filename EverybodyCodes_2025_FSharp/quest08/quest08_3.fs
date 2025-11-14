module quest08_3

open System
open EverybodyCodes_2025_FSharp.Modules
open System.Threading.Tasks

//let path = "quest08/test_input_03.txt"
let path = "quest08/quest08_input_03.txt"

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
        let x = Math.Round(r * cos angle, 2)
        let y = Math.Round(r * sin angle, 2)
        (i+1, (x, y)) ]

let intersection (x1, y1, x2, y2) (x3, y3, x4, y4) =
    let a1 = y2 - y1
    let b1 = x1 - x2
    let c1 = a1 * x1 + b1 * y1

    let a2 = y4 - y3
    let b2 = x3 - x4
    let c2 = a2 * x3 + b2 * y3

    let determinant = a1 * b2 - a2 * b1

    if determinant = 0.0 then
        None
    else
        let x = Math.Round((b2 * c1 - b1 * c2) / determinant, 2)
        let y = Math.Round((a1 * c2 - a2 * c1) / determinant, 2)
        Some (x, y)



let findCollisions (nails: int array) (points: (int * (float * float)) list) =
    let mapping = points |> dict

    let threads =
        nails
        |> Array.pairwise
        |> Array.map(fun (a, b) ->
            let (x1, y1) = mapping[a]
            let (x2, y2) = mapping[b]
            (x1, y1, x2, y2))

    let possibleThreads =
        [| for i in 1 .. mapping.Count - 1 do
               for j in i + 1 .. mapping.Count do
                   let (x1, y1) = mapping[i]
                   let (x2, y2) = mapping[j]
                   yield (x1, y1, x2, y2) |]

    let results = Array.zeroCreate possibleThreads.Length
    Parallel.For(0, possibleThreads.Length, fun idx ->
        let thread = possibleThreads[idx]
        let mutable knots = 0
        let (x1, y1, x2, y2) = thread
        for otherThread in threads do
            let (x3, y3, x4, y4) = otherThread
            if ((x1 = x3 && y1 = y3) || (x2 = x3 && y2 = y3)) && 
                ((x1 = x4 && y1 = y4) || (x2 = x4 && y2 = y4)) then   
                knots <- knots + 1
            match intersection thread otherThread with
            | Some (ix, iy) ->
                let exclude =
                    (ix = x1 && iy = y1) || (ix = x2 && iy = y2) ||
                    (ix = x3 && iy = y3) || (ix = x4 && iy = y4) ||
                    ( (ix < Math.Min(x1, x2)) || (ix > Math.Max(x1, x2)) ||
                      (iy < Math.Min(y1, y2)) || (iy > Math.Max(y1, y2)) ||
                      (ix < Math.Min(x3, x4)) || (ix > Math.Max(x3, x4)) ||
                      (iy < Math.Min(y3, y4)) || (iy > Math.Max(y3, y4)) )
                if not exclude then
                    knots <- knots + 1
            | None -> ()
        results[idx] <- knots
    ) |> ignore
    Array.max results

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let nails = parseContent lines
    //let numOfNails = 8
    let numOfNails = 256
    let circle = generateCirclePoints numOfNails (float(numOfNails) / 4.)
    findCollisions nails circle
