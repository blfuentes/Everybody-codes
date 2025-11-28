module quest19_3

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest19/test_input_03.txt"
let path = "quest19/quest19_input_03.txt"

let parseContent (lines: string array) =
    let gaps = Dictionary<int, (int * int) list>()
    for line in lines do
        let parts = line.Split(",")
        let x = parts[0] |> int
        let y = parts[1] |> int
        let sz = parts[2] |> int
        let gap = (y, y + sz - 1)
        if gaps.ContainsKey(x) then
            gaps[x] <- gap :: gaps[x]
        else
            gaps[x] <- [gap]
    gaps

let canGetThere (x1: int) (y1: int) (x2: int) (y2: int) =
    abs(y2 - y1) <= (x2 - x1)

let findShortestPath (gaps: Dictionary<int, (int * int) list>) =
    let sortedKeys = gaps.Keys |> Seq.sort |> Seq.toList
    
    let computeReachable xPrev reachablePrev x =
        gaps[x]
        |> List.collect (fun (y1, y2) ->
            [y1 .. y2]
            |> List.filter (fun y -> 
                (x + y) % 2 = 0 &&
                reachablePrev |> Set.exists (fun yPrev -> canGetThere xPrev yPrev x y))
        )
        |> Set.ofList
    
    let (finalX, finalReachable) =
        sortedKeys
        |> List.fold (fun (xPrev, reachablePrev) x ->
            let reachable = computeReachable xPrev reachablePrev x
            (x, reachable)
        ) (0, Set.singleton 0)
    
    let ySol = finalReachable |> Set.minElement
    ySol + (finalX - ySol) / 2

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let gaps = parseContent lines
    let result = findShortestPath gaps
    result
