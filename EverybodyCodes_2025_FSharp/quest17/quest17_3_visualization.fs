module quest17_3_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Drawing
open System.Drawing.Imaging
open System.IO

//let path = "quest17/test_input_03.txt"
let path = "quest17/quest17_input_03.txt"

let mutable Xv, Yv = (0, 0)
let mutable maxX, maxY = (0, 0)
let mutable Xe, Ye = (0, 0)

let parseContent (lines: string array) =
    maxX <- lines.Length  // X = rows (height)
    maxY <- lines[0].Length  // Y = columns (width)
    let field = Array2D.create maxX maxY 0
    for x in 0 .. lines.Length - 1 do
        for y in 0 .. lines[x].Length - 1 do
            match lines[x][y] with
            | 'S' ->
                Xe <- x
                Ye <- y
                field[x, y] <- 0
            | '@' ->
                Xv <- x
                Yv <- y
                field[x, y] <- 0
            | _ ->
                field[x, y] <- int((lines[x][y]).ToString())
    field

let isInRange (Xv, Yv) (Xc, Yc) R =
    (Xv - Xc) * (Xv - Xc) + (Yv - Yc) * (Yv - Yc) <= R * R

let printField (field: int[,]) =
    for x in 0 .. maxX - 1 do
        for y in 0 .. maxY - 1 do
            if (x, y) = (Xv, Yv) then
                printf "@"
            else
                printf "%s" (field[x, y].ToString())
        printfn ""

type Coord3D = int * int * int

let dijkstraWithPath (grid: Dictionary<int*int, int>) (start: int*int) (centre: int*int) (limit: int) =
    if grid.Count = 0 then
        (System.Int32.MaxValue, [])
    else
        let getNeighbors (current: Coord3D) =
            let (cellX, cellY, cellZ) = current
            [(1, 0); (-1, 0); (0, 1); (0, -1)]
            |> List.map (fun (dirX, dirY) ->
                let adjX = cellX + dirX
                let adjY = cellY + dirY
                let adjZ =
                    if adjY < snd centre then
                        if cellX < fst centre && adjX >= fst centre then 1
                        elif adjX < fst centre && cellX >= fst centre then 0
                        else cellZ
                    else cellZ
                (adjX, adjY, adjZ))
            |> List.filter (fun (x, y, _) -> grid.ContainsKey((x, y)))
        
        let startNode = (fst start, snd start, 0)
        let goalNode = (fst start, snd start, 1)
        
        let dist = Dictionary<Coord3D, int>()
        let parent = Dictionary<Coord3D, Coord3D>()
        let visited = HashSet<Coord3D>()
        
        let pq = SortedSet<int * Coord3D>(Comparer<int * Coord3D>.Create(fun (d1, c1) (d2, c2) ->
            if d1 <> d2 then compare d1 d2 else compare c1 c2
        ))
        
        dist[startNode] <- 0
        pq.Add((0, startNode)) |> ignore
        
        let mutable result = System.Int32.MaxValue
        
        while pq.Count > 0 do
            let (d, current) = pq.Min
            pq.Remove((d, current)) |> ignore
            
            if current = goalNode then
                result <- d
                pq.Clear()
            elif d > limit then
                () // over the limit...
            elif visited.Contains(current) then
                () // already processed ...
            else
                visited.Add(current) |> ignore
                
                for neighbor in getNeighbors current do
                    let (adjX, adjY, _) = neighbor
                    let alt = d + grid[(adjX, adjY)]
                    if alt <= limit && (not (dist.ContainsKey(neighbor)) || alt < dist[neighbor]) then
                        dist[neighbor] <- alt
                        parent[neighbor] <- current
                        pq.Add((alt, neighbor)) |> ignore
        
        // Reconstruct path
        let rec buildPath acc node =
            let (first, second, _) = node
            if node = startNode then
                (first, second) :: acc
            elif parent.ContainsKey(node) then
                buildPath ((first, second) :: acc) parent[node]
            else
                acc
        
        let path = 
            if result < System.Int32.MaxValue then
                buildPath [] goalNode
                |> List.distinct  // Remove duplicates from z-layer
            else
                []
        
        (result, path)

let dijkstra (grid: Dictionary<int*int, int>) (start: int*int) (centre: int*int) (limit: int) =
    let (result, _) = dijkstraWithPath grid start centre limit
    result

let shortestPath (field: int[,]) =
    let volcano = (Xv, Yv)
    let start = (Xe, Ye)
    
    let rec searchRadius R =
        // build volcano excluding radius
        let volcanoGrid = Dictionary<int*int, int>()
        for x in 0 .. maxX - 1 do
            for y in 0 .. maxY - 1 do
                let distSq = (x - Xv) * (x - Xv) + (y - Yv) * (y - Yv)
                if distSq > R * R then
                    volcanoGrid[(x, y)] <- field[x, y]
        
        volcanoGrid[start] <- 0
        
        // shortest path 30 * (R + 1)
        let limit = 30 * (R + 1)
        let minFound = dijkstra volcanoGrid start volcano limit
        
        if minFound < limit then
            minFound * R
        else
            searchRadius (R + 1)
    
    searchRadius 0

let visualizePathToPNG (field: int[,]) (radius: int) (path: (int*int) list) (filename: string) =
    // Find max value for color scaling
    let maxValue = 
        seq {
            for x in 0 .. maxX - 1 do
                for y in 0 .. maxY - 1 do
                    if (x, y) <> (Xv, Yv) && (x, y) <> (Xe, Ye) then
                        yield field[x, y]
        } |> Seq.max
    
    let cellSize = 30
    let width = maxY * cellSize
    let height = maxX * cellSize
    
    use bitmap = new Bitmap(width, height)
    use g = Graphics.FromImage(bitmap)
    g.Clear(Color.White)
    
    // Configure text rendering
    g.TextRenderingHint <- System.Drawing.Text.TextRenderingHint.AntiAlias
    use emojiFont = new Font("Segoe UI Emoji", float32 cellSize * 0.6f, FontStyle.Regular)
    
    let pathSet = Set.ofList path
    
    for x in 0 .. maxX - 1 do
        for y in 0 .. maxY - 1 do
            let screenX = y * cellSize
            let screenY = x * cellSize
            
            if (x, y) = (Xv, Yv) then
                // Draw volcano emoji
                use sf = new StringFormat()
                sf.Alignment <- StringAlignment.Center
                sf.LineAlignment <- StringAlignment.Center
                use brush = new SolidBrush(Color.Black)
                g.DrawString("🌋", emojiFont, brush, 
                    RectangleF(float32 screenX, float32 screenY, float32 cellSize, float32 cellSize), sf)
            elif (x, y) = (Xe, Ye) then
                // Draw door emoji
                use sf = new StringFormat()
                sf.Alignment <- StringAlignment.Center
                sf.LineAlignment <- StringAlignment.Center
                use brush = new SolidBrush(Color.Black)
                g.DrawString("🚪", emojiFont, brush, 
                    RectangleF(float32 screenX, float32 screenY, float32 cellSize, float32 cellSize), sf)
            else
                let value = field[x, y]
                let distance = sqrt(float((x - Xv) * (x - Xv) + (y - Yv) * (y - Yv)))
                let distanceInt = int(ceil(distance))
                
                let (cellColor, showNumber) = 
                    if pathSet.Contains((x, y)) then
                        // Path cells - yellow
                        (Color.Gold, true)
                    elif distanceInt <= radius then
                        // Inside burnt radius - grey, no number
                        let greyValue = 150 + (value * 50 / (maxValue + 1))
                        (Color.FromArgb(greyValue, greyValue, greyValue), false)
                    else
                        // Outside - green shades from light to dark based on value
                        let greenIntensity = 255 - (value * 200 / (maxValue + 1))
                        (Color.FromArgb(50, greenIntensity, 50), true)
                
                use brush = new SolidBrush(cellColor)
                g.FillRectangle(brush, screenX, screenY, cellSize, cellSize)
                
                // Draw value text only if showNumber is true
                if showNumber then
                    use textBrush = new SolidBrush(Color.Black)
                    use textFont = new Font("Arial", float32 cellSize * 0.4f, FontStyle.Bold)
                    use sf = new StringFormat()
                    sf.Alignment <- StringAlignment.Center
                    sf.LineAlignment <- StringAlignment.Center
                    g.DrawString(value.ToString(), textFont, textBrush,
                        RectangleF(float32 screenX, float32 screenY, float32 cellSize, float32 cellSize), sf)
            
            // Draw grid border
            use pen = new Pen(Color.DarkGray, 1.0f)
            g.DrawRectangle(pen, screenX, screenY, cellSize, cellSize)
    
    // Draw radius circle around volcano
    use radiusPen = new Pen(Color.Red, 3.0f)
    radiusPen.DashStyle <- System.Drawing.Drawing2D.DashStyle.Dash
    let circleX = Yv * cellSize + cellSize / 2 - radius * cellSize
    let circleY = Xv * cellSize + cellSize / 2 - radius * cellSize
    let circleDiameter = radius * 2 * cellSize
    g.DrawEllipse(radiusPen, circleX, circleY, circleDiameter, circleDiameter)
    
    // Add legend
    use legendFont = new Font("Arial", 12.0f, FontStyle.Bold)
    use legendBrush = new SolidBrush(Color.Black)
    let legendText = sprintf "Radius: %d | Path Length: %d" radius path.Length
    g.DrawString(legendText, legendFont, legendBrush, 10.0f, 10.0f)
    
    // Ensure folder exists
    let fullPath = Path.Combine(VisualizationFolder, filename)
    bitmap.Save(fullPath, ImageFormat.Png)
    
    //printfn "Visualization saved to: %s" fullPath
    fullPath

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let field = parseContent lines
    
    // Find the optimal radius and path
    let volcano = (Xv, Yv)
    let start = (Xe, Ye)
    
    let rec searchRadiusWithVisualization R =
        let volcanoGrid = Dictionary<int*int, int>()
        for x in 0 .. maxX - 1 do
            for y in 0 .. maxY - 1 do
                let distSq = (x - Xv) * (x - Xv) + (y - Yv) * (y - Yv)
                if distSq > R * R then
                    volcanoGrid[(x, y)] <- field[x, y]
        
        volcanoGrid[start] <- 0
        
        let limit = 30 * (R + 1)
        let (minFound, path) = dijkstraWithPath volcanoGrid start volcano limit
        
        if minFound < limit then
            // Visualize the successful path
            visualizePathToPNG field R path "quest17_3_visualization.png" |> ignore
            minFound * R
        else
            searchRadiusWithVisualization (R + 1)
    
    searchRadiusWithVisualization 0