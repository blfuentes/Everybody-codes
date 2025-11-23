module quest15_1_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Drawing
open System.IO

//let path = "quest15/test_input_02.txt"
let path = "quest15/quest15_input_01.txt"

type Op =
    | Right of int
    | Left of int

let parseContent (lines: string) =
    lines.Split(",")
    |> Array.map (fun line ->
        match line[0] with
        | 'R' -> Right (int (line.Substring(1)))
        | 'L' -> Left (int (line.Substring(1)))
        | _ -> failwith "Unknown operation"
    )

let buildMap (operations: Op array) : int * int * int * int * Set<(int * int)> =
    let xStart, yStart = 0, 0
    // folder: (x,y,dx,dy,wall) -> op -> new state
    let folder (x, y, dx, dy, wall: Set<(int*int)>) op =
        match op with
        | Left n ->
            let ndx, ndy = -dy, dx
            let positions = seq { 1 .. n } |> Seq.map (fun i -> (x + ndx * i, y + ndy * i))
            let wall' = Seq.fold (fun (s:Set<(int*int)>) pos -> s.Add pos) wall positions
            let xn, yn = x + ndx * n, y + ndy * n
            (xn, yn, ndx, ndy, wall')
        | Right n ->
            let ndx, ndy = dy, -dx
            let positions = seq { 1 .. n } |> Seq.map (fun i -> (x + ndx * i, y + ndy * i))
            let wall' = Seq.fold (fun (s:Set<(int*int)>) pos -> s.Add pos) wall positions
            let xn, yn = x + ndx * n, y + ndy * n
            (xn, yn, ndx, ndy, wall')
    let init = (xStart, yStart, 0, 1, Set.empty.Add (xStart, yStart))
    let (xEnd, yEnd, _, _, wall) = Array.fold folder init operations
    (xStart, yStart, xEnd, yEnd, wall)

let printMap(walls: Set<(int*int)>) (points: HashSet<int*int>)=
    let minX = walls |> Seq.map fst |> Seq.min
    let maxX = walls |> Seq.map fst |> Seq.max
    let minY = walls |> Seq.map snd |> Seq.min
    let maxY = walls |> Seq.map snd |> Seq.max
    for y in minY .. maxY do
        for x in minX .. maxX do
            if points.Contains(x,y) then
                printf "O"
            else
                if walls.Contains (x, y) then
                    printf "#"
                else
                    printf "."
        printfn ""

let bfs (start: int*int) (goal: int*int) (walls: Set<int*int>) =
    let minX = walls |> Seq.map fst |> Seq.min |> fun x -> x - 1
    let maxX = walls |> Seq.map fst |> Seq.max |> fun x -> x + 1
    let minY = walls |> Seq.map snd |> Seq.min |> fun y -> y - 1
    let maxY = walls |> Seq.map snd |> Seq.max |> fun y -> y + 1

    let getNeighbors (x, y) =
        [ (x-1, y); (x+1, y); (x, y-1); (x, y+1) ]
        |> List.filter (fun (x, y) ->
            not(walls.Contains (x, y)) &&
            x >= minX && x < maxX && y >= minY && y < maxY
        )

    let visited = HashSet<int*int>()
    let parent = Dictionary<int*int, int*int>()
    let queue = Queue<int*int>()
    queue.Enqueue(start)
    visited.Add(start) |> ignore

    while queue.Count > 0 do
        let current = queue.Dequeue()
        if current = goal then 
            queue.Clear()
        else
            for n in getNeighbors current do
                if not (visited.Contains(n)) then
                    visited.Add(n) |> ignore
                    parent[n] <- current
                    queue.Enqueue(n)

    let rec buildPath acc pos =
        if pos = start then start :: acc
        elif parent.ContainsKey(pos) then buildPath (pos :: acc) parent[pos]
        else []

    let path = buildPath [] goal
    path.Length - 1, path

let ensureFolder (folderPath: string) =
    if not (Directory.Exists(folderPath)) then
        Directory.CreateDirectory(folderPath) |> ignore

let visualizeMap (visitedPoints: (int*int) list) (walls: Set<int*int>) (currentPoint: int*int) (frameNumber: int) =
    ensureFolder "quest15_1_visualization"
    
    let minX = walls |> Seq.map fst |> Seq.min |> fun x -> x - 1
    let maxX = walls |> Seq.map fst |> Seq.max |> fun x -> x + 1
    let minY = walls |> Seq.map snd |> Seq.min |> fun y -> y - 1
    let maxY = walls |> Seq.map snd |> Seq.max |> fun y -> y + 1
    
    let pixelSize = 20
    let width = (maxX - minX + 1) * pixelSize
    let height = (maxY - minY + 1) * pixelSize
    
    let bitmap = new Bitmap(width, height)
    use g = Graphics.FromImage(bitmap)
    
    // Set background to white
    g.Clear(Color.White)
    
    let visitedSet = Set.ofList visitedPoints
    
    // Draw each cell
    for y in minY .. maxY do
        for x in minX .. maxX do
            let screenX = (x - minX) * pixelSize
            let screenY = (y - minY) * pixelSize
            
            if walls.Contains(x, y) then
                // Draw wall with brick emoji representation (use orange color)
                use brush = new SolidBrush(Color.Orange)
                g.FillRectangle(brush, screenX, screenY, pixelSize, pixelSize)
                use pen = new Pen(Color.DarkOrange, 1.0f)
                g.DrawRectangle(pen, screenX, screenY, pixelSize, pixelSize)
            elif (x, y) = currentPoint then
                // Draw current point as smiley face (yellow circle with face)
                use brush = new SolidBrush(Color.Yellow)
                g.FillEllipse(brush, screenX, screenY, pixelSize, pixelSize)
                use pen = new Pen(Color.Black, 2.0f)
                g.DrawEllipse(pen, screenX, screenY, pixelSize, pixelSize)
                // Draw simple smiley face
                let centerX = screenX + pixelSize / 2
                let centerY = screenY + pixelSize / 2
                let dotSize = 2
                g.FillEllipse(Brushes.Black, centerX - 4, centerY - 4, dotSize, dotSize)
                g.FillEllipse(Brushes.Black, centerX + 4, centerY - 4, dotSize, dotSize)
                g.DrawArc(pen, screenX + 4, screenY + 4, pixelSize - 8, pixelSize - 8, 0, 180)
            elif visitedSet.Contains(x, y) then
                // Draw visited point as 'X'
                use pen = new Pen(Color.Blue, 2.0f)
                g.DrawLine(pen, screenX, screenY, screenX + pixelSize, screenY + pixelSize)
                g.DrawLine(pen, screenX + pixelSize, screenY, screenX, screenY + pixelSize)
            else
                // Draw free space as light gray grid
                use pen = new Pen(Color.LightGray, 0.5f)
                g.DrawRectangle(pen, screenX, screenY, pixelSize, pixelSize)
    
    // Save bitmap
    let fileName = Path.Combine("quest15_1_visualization", $"frame_{frameNumber:D4}.png")
    bitmap.Save(fileName)
    bitmap.Dispose()
    
    fileName

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let operations = parseContent lines
    let (xStart, yStart, xEnd, yEnd, wall) = buildMap operations
    let (length, path ) = bfs (xEnd, yEnd) (xStart, yStart) (Set.remove (xStart, yStart) wall)
    
    // Visualize the path
    path
    |> List.iteri (fun i point ->
        let visitedUpToNow = path |> List.take (i + 1)
        let fileName = visualizeMap visitedUpToNow wall point i
        printfn "Saved frame %d to %s" i fileName
    )
    
    length