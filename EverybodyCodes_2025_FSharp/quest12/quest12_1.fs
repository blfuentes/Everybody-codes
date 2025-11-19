module quest12_1

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest12/test_input_01.txt"
let path = "quest12/quest12_input_01.txt"

type Position = { Row: int; Col: int }

type Barrel = { Position: Position; Size: int }

let parseContent (lines: string array) =
    let barrelsMap = Array2D.zeroCreate<Barrel> lines.Length lines[0].Length
    lines
    |> Array.iteri (fun row line ->
        line.ToCharArray()
        |> Array.iteri (fun col sizeStr ->
            let size = int (sizeStr.ToString())
            barrelsMap[row, col] <- { Position = { Row = row; Col = col }; Size = size }
        )
    )
    barrelsMap

let printBarrelsMap (barrelsMap: Barrel[,]) =
    let rows = barrelsMap.GetLength(0)
    let cols = barrelsMap.GetLength(1)
    for row in 0 .. rows - 1 do
        for col in 0 .. cols - 1 do
            printf "%d " barrelsMap[row, col].Size
        printfn ""

// Flood fill
let floodFill (maze: Barrel[,]) (initBarrel: Barrel) : Set<Position> =
    let getNeighbors (maze: Barrel[,]) (pos: Position) =
        let rows = maze.GetLength(0)
        let cols = maze.GetLength(1)
        [ (pos.Row-1, pos.Col); (pos.Row+1, pos.Col); (pos.Row, pos.Col-1); (pos.Row, pos.Col+1) ]
        |> List.filter (fun (r, c) ->
            r >= 0 && r < rows && c >= 0 && c < cols &&
            maze[r,c].Size <= maze[pos.Row, pos.Col].Size
            //match maze[r, c].Kind with Wall -> false | _ -> true
        ) |> List.map (fun (r, c) -> { Row = r; Col = c })
    let start = initBarrel.Position
    let visited = HashSet<Position>()
    let queue = Queue<Position>()
    queue.Enqueue(start)
    visited.Add(start) |> ignore

    while queue.Count > 0 do
        let current = queue.Dequeue()
        for n in getNeighbors maze current do
            if not (visited.Contains(n)) then
                visited.Add(n) |> ignore
                queue.Enqueue(n)

    visited |> Set.ofSeq

let igniteBarrels(barrelsMap: Barrel[,]) =
    // Placeholder for ignition logic
    let initialBarrel = barrelsMap[0, 0]
    let affectedPositions = floodFill barrelsMap initialBarrel
    affectedPositions.Count

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let barrelsMap = parseContent lines
    printBarrelsMap barrelsMap
    igniteBarrels barrelsMap
