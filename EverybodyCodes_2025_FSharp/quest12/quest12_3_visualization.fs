module quest12_3_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Drawing
open System.Threading.Tasks
open System.Collections.Concurrent
open System.IO

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
    
    third
  
// Generate color based on barrel size (light blue to dark blue)
// Size 0 = lightest blue, Size 9 = darkest blue
let getSizeColor (size: int) : Color =
    match size with
    | 0 -> Color.FromArgb(200, 230, 255)  // Very light blue
    | 1 -> Color.FromArgb(180, 215, 255)
    | 2 -> Color.FromArgb(160, 200, 255)
    | 3 -> Color.FromArgb(140, 185, 255)
    | 4 -> Color.FromArgb(120, 170, 255)
    | 5 -> Color.FromArgb(100, 155, 255)
    | 6 -> Color.FromArgb(80, 140, 255)
    | 7 -> Color.FromArgb(60, 125, 255)
    | 8 -> Color.FromArgb(40, 110, 255)
    | _ -> Color.FromArgb(20, 95, 255)    // Very dark blue for size 9+

let colorBarrels (bitmap: Bitmap) (barrelsMap: Barrel[,]) (burntBarrels: Set<Position>) (pixelSize: int) =
    let burntColor = Color.Yellow

    let rows = barrelsMap.GetLength(0)
    let cols = barrelsMap.GetLength(1)
    
    use g = Graphics.FromImage(bitmap)
    
    for row in 0 .. rows - 1 do
        for col in 0 .. cols - 1 do
            let pos = { Row = row; Col = col }
            let barrel = barrelsMap[row, col]
            
            // Determine color: yellow if burnt, otherwise blue shades based on size
            let color =
                if burntBarrels.Contains(pos) then
                    burntColor
                else
                    getSizeColor barrel.Size
            
            // Draw filled rectangle for this barrel
            use brush = new SolidBrush(color)
            g.FillRectangle(brush, col * pixelSize, row * pixelSize, pixelSize, pixelSize)
            
            //// Draw border for clarity
            //use pen = new Pen(Color.DarkGray, 1.0f)
            //g.DrawRectangle(pen, col * pixelSize, row * pixelSize, pixelSize, pixelSize)

let createBarrelsImage (barrelsMap: Barrel[,]) (burntBarrels: Set<Position>) (pixelSize: int) =
    let bitmap = new Bitmap(barrelsMap.GetLength(1) * pixelSize, barrelsMap.GetLength(0) * pixelSize)
    colorBarrels bitmap barrelsMap burntBarrels pixelSize
    bitmap

let saveBarrelsImage (filePath: string) (bitmap: Bitmap) =
    bitmap.Save(filePath)
    bitmap.Dispose()

let visualizeBurntBarrels (barrelsMap: Barrel[,]) (burntBarrels: Set<Position>) =
    let pixelSize = 5  // Each barrel cell is 20x20 pixels
    let bitmap = createBarrelsImage barrelsMap burntBarrels pixelSize
    saveBarrelsImage (Path.Combine(VisualizationFolder, "quest12_3_visualization.png")) bitmap  


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let barrelsMap = parseContent lines
    let burntBarrels = findBestThreeBarrels barrelsMap
    visualizeBurntBarrels barrelsMap burntBarrels
    burntBarrels.Count
