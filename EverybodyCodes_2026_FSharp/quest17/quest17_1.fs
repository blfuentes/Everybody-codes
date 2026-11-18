module quest17_1

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

//let path = "quest17/test_input_01.txt"
let path = "quest17/quest17_input_01.txt"

let squares = HashSet<(int*int)>()
let mutable(maxRow,maxCol) = (0, 0)
let parseContent(lines: string array) =
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    
    for r in 0..maxRow do
        for c in 0..maxCol do
            if lines[r][c] = '#' then
                squares.Add((r, c)) |> ignore

let dirs = [
    (0, 1); (0, -1); (-1, 0); (1, 0)
]

let findPath() =
    let ending = (maxRow, maxCol)
    let start = (0, 0)

    let toVisit = Queue<((int*int)*int)>()
    toVisit.Enqueue(start, 0)

    let visited = HashSet<(int*int)>()

    let mutable current = Unchecked.defaultof<(int*int)*int>
    let mutable pathLength = 0
    while toVisit.TryDequeue(&current) do
        let ((cr, cc), count) = current
        visited.Add(cr, cc) |> ignore
        if (cr, cc) = ending then
            pathLength <- count
        else
            for (dr, dc) in dirs do
                let newPos = (cr + dr, cc + dc)
                if  squares.Contains(newPos) && not (visited.Contains(newPos)) then
                    toVisit.Enqueue(newPos, count + 1)

    pathLength


let execute() =
    let lines = LocalHelper.GetContent path 2026 17 1
    parseContent lines
    findPath()
