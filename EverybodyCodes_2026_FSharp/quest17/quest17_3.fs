module quest17_3

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

//let path = "quest17/test_input_03.txt"
 let path = "quest17/quest17_input_03.txt"

let squares = HashSet<(int*int)>()
let mutable(maxRow,maxCol) = (0, 0)
let parseContent(lines: string array) =
    squares.Clear()
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    
    for r in 0..maxRow do
        for c in 0..maxCol do
            if lines[r][c] = '#' then
                squares.Add((r, c)) |> ignore

let directions = [
    (0, 1); (1, 0); (0, -1); (-1, 0)
]

let shortestEdgePath()=
    // orientate stones to the right
    let edges = HashSet<int * int * int>()
    for (row, col) in squares do
        if not (squares.Contains(row - 1, col)) then edges.Add(row, col, 0) |> ignore
        if not (squares.Contains(row, col + 1)) then edges.Add(row, col + 1, 1) |> ignore
        if not (squares.Contains(row + 1, col)) then edges.Add(row + 1, col + 1, 2) |> ignore
        if not (squares.Contains(row, col - 1)) then edges.Add(row + 1, col, 3) |> ignore

    let start = (0, 0, 0)

    let visited = HashSet<int * int * int>()
    let mutable edge = start
    let mutable distance = 0
    let mutable targetDistance = 0
    let mutable finished = false

    while not finished do
        let (row, col, direction) = edge
        let (dr, dc) = directions[direction]
        let nextRow, nextCol = row + dr, col + dc
        distance <- distance + 1
        if (nextRow, nextCol) = (maxRow + 1, maxCol + 1) then
            targetDistance <- distance

        // turn right
        let nextDirection =
            [ (direction + 1) % 4; direction; (direction + 3) % 4; (direction + 2) % 4 ]
            |> List.find (fun d -> edges.Contains(nextRow, nextCol, d))

        edge <- (nextRow, nextCol, nextDirection)
        finished <- edge = start

    min targetDistance (distance - targetDistance)

let execute() =
    let lines = LocalHelper.GetContent path 2026 17 3
    parseContent lines
    shortestEdgePath()
