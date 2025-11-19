module quest12_3

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Threading.Tasks
open System.Collections.Concurrent

//let path = "quest12/test_input_03.txt"
let path = "quest12/quest12_input_03.txt"

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

// Slow around ~1min...
//let findBestThreeBarrels(barrelsMap: Barrel[,]) =
//    let rows = barrelsMap.GetLength(0)
//    let cols = barrelsMap.GetLength(1)
//    let allBarrels =
//        [ for r in 0 .. rows - 1 do
//            for c in 0 .. cols - 1 do
//                yield barrelsMap[r, c] ]
//    let allPositions = allBarrels |> List.map (fun b -> b.Position) |> Set.ofList

//    let mutable remaining = allPositions
//    let mutable totalDestroyed = Set.empty<Position>

//    for _ in 1 .. 3 do
//        let candidates = new ConcurrentBag<Barrel * Set<Position>>()
//        Parallel.For(0, allBarrels.Length, fun i ->
//            let barrel = allBarrels[i]
//            if remaining.Contains(barrel.Position) then
//                let affected = floodFill barrelsMap barrel |> Set.intersect remaining
//                candidates.Add(barrel, affected)
//        ) |> ignore
//        let best =
//            candidates
//            |> Seq.maxBy (fun (_, affected) -> affected.Count)
//        let bestAffected = snd best
//        remaining <- remaining - bestAffected
//        totalDestroyed <- totalDestroyed + bestAffected

//    totalDestroyed.Count

let findBestThreeBarrels(barrelsMap: Barrel[,]) =
    let rows = barrelsMap.GetLength(0)
    let cols = barrelsMap.GetLength(1)
    let allBarrels =
        [| for r in 0 .. rows - 1 do
            for c in 0 .. cols - 1 do
                yield barrelsMap[r, c] |]
    
    let points =
        allBarrels
        |> Array.sortBy (fun b -> b.Size)
        |> Array.map (fun b -> b.Position)
        |> ResizeArray

    let sets = ResizeArray<Set<Position>>()
    let pointsHash = HashSet<Position>(points)

    while points.Count > 0 do
        let start = points[points.Count - 1]
        points.RemoveAt(points.Count - 1)
        let barrel = barrelsMap[start.Row, start.Col]
        let affected = floodFill barrelsMap barrel
        pointsHash.ExceptWith(affected)
        let remaining = ResizeArray()
        for p in points do 
            if pointsHash.Contains(p) then 
                remaining.Add(p)
        points.Clear()
        for p in remaining do 
            points.Add(p)
        sets.Add(affected)

    let mutable setsWork : Set<Position>[] = sets |> Seq.toArray
    
    // First fireball
    Array.sortInPlaceWith (fun (s1: Set<Position>) (s2: Set<Position>) -> compare s2.Count s1.Count) setsWork
    let first = setsWork[0]
    setsWork <- setsWork[1..]
    for i in 0 .. setsWork.Length - 1 do
        setsWork[i] <- Set.union setsWork[i] first
    
    // Second fireball
    Array.sortInPlaceWith (fun (s1: Set<Position>) (s2: Set<Position>) -> compare s2.Count s1.Count) setsWork
    let second = setsWork[0]
    setsWork <- setsWork[1..]
    for i in 0 .. setsWork.Length - 1 do
        setsWork[i] <- Set.union setsWork[i] second
    
    // Third fireball
    Array.sortInPlaceWith (fun (s1: Set<Position>) (s2: Set<Position>) -> compare s2.Count s1.Count) setsWork
    let third = setsWork[0]
    
    third.Count
    

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let barrelsMap = parseContent lines
    findBestThreeBarrels barrelsMap
