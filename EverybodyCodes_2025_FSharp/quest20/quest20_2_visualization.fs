module quest20_2_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Drawing
open System.Drawing.Imaging
open System.IO

//let path = "quest20/test_input_02.txt"
let path = "quest20/quest20_input_02.txt"

type Facing =
    | Down
    | Up
type CellType =
    | Wall
    | None
    | Trampoling

type Trampoling = {
    Id: string
    Position: int * int
    Orientation: Facing
    Kind: CellType
}

let mutable maxRow = 0
let mutable maxCol = 0
let mutable startPos: int * int = (0,0)
let mutable endPos: int * int = (0,0)

let TrampolingCollection = new Dictionary<int*int, Trampoling>()

let parseContent (lines: string array) =
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    let trampoling = Array2D.init (maxRow + 1) (maxCol + 1) (fun r c ->
        let id = sprintf "%d,%d" r c
        match r % 2 with
        | 0 ->
            match c % 2 with
            | 0 -> match lines[r][c] with
                    | '.' -> { Id = id; Position = (r, c); Orientation = Down; Kind = None }
                    | 'T' -> 
                        let t = { Id = id; Position = (r, c); Orientation = Down; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | '#' -> { Id = id; Position = (r, c); Orientation = Down; Kind = Wall }
                    | 'S' -> 
                        startPos <- (r, c)
                        let t = { Id = id; Position = (r, c); Orientation = Down; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | 'E' ->
                        endPos <- (r, c)
                        let t = { Id = id; Position = (r, c); Orientation = Down; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | _ -> failwith "Invalid character"
            | 1 -> match lines[r][c] with
                    | '.' -> { Id = id; Position = (r, c); Orientation = Up; Kind = None }
                    | 'T' -> 
                        let t = { Id = id; Position = (r, c); Orientation = Up; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | '#' -> { Id = id; Position = (r, c); Orientation = Up; Kind = Wall }
                    | 'S' -> 
                        startPos <- (r, c)
                        let t = { Id = id; Position = (r, c); Orientation = Up; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | 'E' ->
                        endPos <- (r, c)
                        let t = { Id = id; Position = (r, c); Orientation = Up; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | _ -> failwith "Invalid character"
            | _ -> failwith "Invalid character"
        | 1 ->
            match c % 2 with
            | 0 -> match lines[r][c] with
                    | '.' -> { Id = id; Position = (r, c); Orientation = Up; Kind = None }
                    | 'T' -> 
                        let t = { Id = id; Position = (r, c); Orientation = Up; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | '#' -> { Id = id; Position = (r, c); Orientation = Up; Kind = Wall }
                    | 'S' -> 
                        startPos <- (r, c)
                        let t = { Id = id; Position = (r, c); Orientation = Up; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | 'E' ->
                        endPos <- (r, c)
                        let t = { Id = id; Position = (r, c); Orientation = Up; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | _ -> failwith "Invalid character"
            | 1 -> match lines[r][c] with
                    | '.' -> { Id = id; Position = (r, c); Orientation = Down; Kind = None }
                    | 'T' -> 
                        let t = { Id = id; Position = (r, c); Orientation = Down; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | '#' -> { Id = id; Position = (r, c); Orientation = Down; Kind = Wall }
                    | 'S' -> 
                        startPos <- (r, c)
                        let t = { Id = id; Position = (r, c); Orientation = Down; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | 'E' ->
                        endPos <- (r, c)
                        let t = { Id = id; Position = (r, c); Orientation = Down; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | _ -> failwith "Invalid character"
            | _ -> failwith "Invalid character"
        | _ -> failwith "Invalid value"
    )
    trampoling

let printTrampoling (trampoling: Trampoling[,]) =
    for r in 0 .. maxRow do
        for c in 0 .. maxCol do
            match trampoling[r, c].Kind with
            | Wall -> printf "#"
            | None -> printf "."
            | Trampoling -> 
                if (r, c) = startPos then
                    printf "S"
                elif (r, c) = endPos then
                    printf "E"
                else
                    match trampoling[r, c].Orientation with
                    | Down -> 
                        // print triangle pointing down
                        printf "D"
                    | Up -> 
                        // print triangle pointing up
                        printf "U"
        printfn ""

let findConnections() =
    let pairs = HashSet<Trampoling * Trampoling>()
    for kvp in TrampolingCollection do
        let (r, c) = kvp.Key
        let t1 = kvp.Value
        let neighbors = 
            [ (r-1, c); (r+1, c); (r, c-1); (r, c+1); ]
        for (nr, nc) in neighbors do
            if TrampolingCollection.ContainsKey((nr, nc)) then
                let t2 = TrampolingCollection[(nr, nc)]
                let add = 
                    if r = nr then
                        t1.Orientation <> t2.Orientation                        
                    elif r < nr then
                        t1.Orientation = Up && t2.Orientation = Down
                    else
                        t1.Orientation = Down && t2.Orientation = Up
                if add then
                    if not(pairs.Contains((t1, t2)) || pairs.Contains((t2, t1))) then
                            pairs.Add((t1, t2)) |> ignore
    pairs

let findShortestPath (trampoling: Trampoling[,]) (connections: HashSet<Trampoling * Trampoling>)=
    let getNeighbors (row, col) =
        [ (row-1, col); (row+1, col); (row, col-1); (row, col+1) ]
        |> List.filter (fun (r, c) ->
            let inRange = r >= 0 && r <= maxRow && c >= 0 && c <= maxCol
            if inRange then
                let from = trampoling[row, col]
                let to' = trampoling[r, c]
                connections.Contains((from, to')) || connections.Contains((to', from))
            else 
                false
        )
    let visited = HashSet<int*int>()
    let parent = Dictionary<int*int, int*int>()
    let queue = Queue<int*int>()
    queue.Enqueue(startPos)
    visited.Add(startPos) |> ignore

    let mutable found = false
    while queue.Count > 0 && not found do
        let current = queue.Dequeue()
        if current = endPos then 
            found <- true
        else
            for n in getNeighbors current do
                if not (visited.Contains(n)) then
                    visited.Add(n) |> ignore
                    parent[n] <- current
                    queue.Enqueue(n)

    let rec buildPath acc pos =
        if pos = startPos then startPos :: acc
        elif parent.ContainsKey(pos) then buildPath (pos :: acc) parent[pos]
        else []

    let path = if found then buildPath [] endPos else []
    path.Length, path
    
let visualizeToImage (trampoling: Trampoling[,]) (path: (int*int) list) (outputPath: string) =
    let cellSize = 50  // Increased size for consistent emoji rendering
    let width = (maxCol + 1) * cellSize
    let height = (maxRow + 1) * cellSize
    
    use bitmap = new Bitmap(width, height)
    use g = Graphics.FromImage(bitmap)
    g.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.AntiAlias
    g.TextRenderingHint <- System.Drawing.Text.TextRenderingHint.AntiAlias
    
    // Fill background
    g.Clear(Color.FromArgb(240, 240, 245))  // Light gray-blue background
    
    // Convert path to set for quick lookup
    let pathSet = Set.ofList path
    
    // Create a dictionary to store path directions
    let pathDirections = Dictionary<int*int, int*int>()
    for i in 0 .. path.Length - 2 do
        pathDirections.[path.[i]] <- path.[i + 1]
    
    // Draw each cell
    for r in 0 .. maxRow do
        for c in 0 .. maxCol do
            let rect = Rectangle(c * cellSize, r * cellSize, cellSize, cellSize)
            let cell = trampoling[r, c]
            let isOnPath = pathSet.Contains((r, c))
            
            // Draw cell background with colors
            let backgroundColor = 
                if (r, c) = startPos then Color.FromArgb(144, 238, 144)  // Light green for start
                elif (r, c) = endPos then Color.FromArgb(255, 223, 0)  // Bright yellow for goal/star
                elif isOnPath then Color.FromArgb(50, 205, 50)  // Lime green for path
                elif cell.Kind = Wall then Color.FromArgb(139, 69, 19)  // Saddle brown for walls/bricks
                elif cell.Kind = None then Color.FromArgb(47, 79, 79)  // Dark slate gray for holes
                elif cell.Kind = Trampoling then
                    match cell.Orientation with
                    | Down -> Color.FromArgb(173, 216, 230)  // Light blue for down trampolines
                    | Up -> Color.FromArgb(255, 182, 193)  // Light pink for up trampolines
                else Color.White
            
            use brush = new SolidBrush(backgroundColor)
            g.FillRectangle(brush, rect)
            
            // Determine emoji based on cell type and position
            let emoji = 
                if (r, c) = startPos then "🚪"  // Open door for start
                elif (r, c) = endPos then "⭐"  // Star for goal
                elif cell.Kind = Wall then "🧱"  // Brick wall
                elif cell.Kind = None then "🕳️"  // Black hole for None
                elif cell.Kind = Trampoling then
                    if isOnPath then "🟢"  // Green circle for path on trampoline
                    else
                        match cell.Orientation with
                        | Down -> "🔽"  // Down triangle
                        | Up -> "🔼"    // Up triangle
                else "  "
            
            // Draw emoji with fixed width font for consistency
            use font = new Font("Segoe UI Emoji", 24.0f, FontStyle.Regular)
            use textBrush = new SolidBrush(Color.Black)
            use sf = new StringFormat()
            sf.Alignment <- StringAlignment.Center
            sf.LineAlignment <- StringAlignment.Center
            
            // Create a centered rectangle for emoji
            let emojiRect = RectangleF(float32(c * cellSize), float32(r * cellSize), float32 cellSize, float32 cellSize)
            g.DrawString(emoji, font, textBrush, emojiRect, sf)
            
            // Draw grid lines with darker color for better visibility
            use gridPen = new Pen(Color.FromArgb(128, 128, 128), 2.0f)
            g.DrawRectangle(gridPen, rect)
    
    // Draw directional arrows on the path
    use arrowPen = new Pen(Color.White, 3.0f)
    arrowPen.EndCap <- System.Drawing.Drawing2D.LineCap.ArrowAnchor
    arrowPen.CustomEndCap <- new System.Drawing.Drawing2D.AdjustableArrowCap(5.0f, 5.0f)
    
    for i in 0 .. path.Length - 2 do
        let (r1, c1) = path.[i]
        let (r2, c2) = path.[i + 1]
        
        // Calculate center points of cells
        let centerX1 = float32(c1 * cellSize + cellSize / 2)
        let centerY1 = float32(r1 * cellSize + cellSize / 2)
        let centerX2 = float32(c2 * cellSize + cellSize / 2)
        let centerY2 = float32(r2 * cellSize + cellSize / 2)
        
        // Calculate direction vector
        let dx = centerX2 - centerX1
        let dy = centerY2 - centerY1
        let distance = sqrt(dx * dx + dy * dy)
        
        // Normalize and shorten the arrow to not overlap with emojis
        let shortenBy = 15.0f
        let startX = centerX1 + (dx / distance) * shortenBy
        let startY = centerY1 + (dy / distance) * shortenBy
        let endX = centerX2 - (dx / distance) * shortenBy
        let endY = centerY2 - (dy / distance) * shortenBy
        
        // Draw the arrow
        g.DrawLine(arrowPen, startX, startY, endX, endY)
    
    // Add legend with colored backgrounds
    let legendY = height + 15
    use finalBitmap = new Bitmap(width, height + 140)
    use gFinal = Graphics.FromImage(finalBitmap)
    gFinal.Clear(Color.White)
    gFinal.DrawImage(bitmap, 0, 0)
    
    gFinal.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.AntiAlias
    gFinal.TextRenderingHint <- System.Drawing.Text.TextRenderingHint.AntiAlias
    
    // Draw legend title
    use titleFont = new Font("Arial", 14.0f, FontStyle.Bold)
    use legendBrush = new SolidBrush(Color.Black)
    gFinal.DrawString("Legend:", titleFont, legendBrush, 10.0f, float32(legendY - 5))
    
    // Draw legend items with colored boxes
    use legendFont = new Font("Segoe UI Emoji", 11.0f)
    use labelFont = new Font("Arial", 10.0f)
    
    let legendItems = [
        ("🚪", "Start", Color.FromArgb(144, 238, 144))
        ("⭐", "Goal", Color.FromArgb(255, 223, 0))
        ("🔼", "Up Trampoline", Color.FromArgb(255, 182, 193))
        ("🔽", "Down Trampoline", Color.FromArgb(173, 216, 230))
        ("🕳️", "Empty/Hole", Color.FromArgb(47, 79, 79))
        ("🧱", "Wall/Brick", Color.FromArgb(139, 69, 19))
        ("🟢", "Path", Color.FromArgb(50, 205, 50))
    ]
    
    let mutable xOffset = 10.0f
    let mutable yOffset = legendY + 25
    for (emoji, text, bgColor) in legendItems do
        // Draw colored background box
        let boxRect = Rectangle(int xOffset, yOffset, 25, 20)
        use bgBrush = new SolidBrush(bgColor)
        gFinal.FillRectangle(bgBrush, boxRect)
        use boxPen = new Pen(Color.Black, 1.0f)
        gFinal.DrawRectangle(boxPen, boxRect)
        
        // Draw emoji
        gFinal.DrawString(emoji, legendFont, legendBrush, xOffset + 30.0f, float32(yOffset))
        
        // Draw text with appropriate color for readability
        use textBrush = if bgColor.GetBrightness() < 0.5f then new SolidBrush(Color.White) else new SolidBrush(Color.Black) 
        gFinal.DrawString(text, labelFont, textBrush, xOffset + 55.0f, float32(yOffset + 3))
        if bgColor.GetBrightness() < 0.5f then textBrush.Dispose()
        
        xOffset <- xOffset + 150.0f
        if xOffset > float32(width - 150) then
            xOffset <- 10.0f
            yOffset <- yOffset + 30
    
    // Add path length info
    use infoBrush = new SolidBrush(Color.DarkBlue)
    use infoFont = new Font("Arial", 12.0f, FontStyle.Bold)
    gFinal.DrawString(sprintf "Path Length: %d steps" (path.Length - 1), infoFont, Brushes.DarkBlue, 10.0f, float32(height + 120))
    
    // Save image
    finalBitmap.Save(Path.Combine(VisualizationFolder, outputPath), ImageFormat.Png)
    printfn "Image saved to: %s" (Path.Combine(VisualizationFolder, outputPath))

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let trampolingMap = parseContent lines
    //printTrampoling trampolingMap
    let connections = findConnections()
    let pathLength, path = findShortestPath trampolingMap connections
    
    // Generate visualization
    let outputPath = "quest20_2_visualization.png"
    visualizeToImage trampolingMap path outputPath
    
    pathLength - 1