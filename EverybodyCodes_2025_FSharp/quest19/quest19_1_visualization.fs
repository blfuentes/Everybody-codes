module quest19_1_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Drawing
open System.Drawing.Imaging
open System.IO

//let path = "quest19/test_input_01.txt"
let path = "quest19/quest19_input_01.txt"

let empty = new HashSet<int*int>()
let walls = new HashSet<int>()

let mutable maxX = 0
let mutable maxY = 0

let reverseX (x: int) =
    maxX - x
let reverseY (y: int) =
    maxY - y

let parseContent (lines: string array) =
    lines
    |> Array.iter(fun line ->
        let parts = line.Split(",")
        let x = parts[0] |> int
        let endingY = (parts[1] |> int) + (parts[2] |> int)
        walls.Add(x) |> ignore
        if endingY > maxY then
            maxY <- endingY + (x - maxX)
        maxX <- x
    )
    lines
    |> Array.iter(fun line ->
        let parts = line.Split(",")
        let x = parts[0] |> int
        let startingY = parts[1] |> int 
        let endingY = (parts[1] |> int) + (parts[2] |> int)
        for y in startingY .. (endingY-1) do
            let (eY, eX) = (reverseY y, x)
            if not (empty.Contains((eY, eX))) then
                empty.Add(eY, eX) |> ignore
    )

let printGrid (path: (int*int) list)=
    for y in 0 .. maxY do
        for x in 0 .. maxX do
            if List.contains (x, y) path then
                printf "O"
            else
                if walls.Contains(x) then
                    printf "%c" (if empty.Contains((y, x)) then '.' else '#')
                else
                    printf "."
        printfn ""

let calculatePathCost (path: (int*int) list) =
    path
    |> List.pairwise
    |> List.sumBy(fun ((x1, y1), (x2, y2)) ->
        if y2 < y1 then 1 else 0
    )

let visualizeToImage (filename: string) (path: (int*int) list) =
    let cellSize = 24  // Increased for better emoji rendering
    let width = (maxX + 1) * cellSize
    let height = (maxY + 1) * cellSize
    
    use bitmap = new Bitmap(width, height)
    use g = Graphics.FromImage(bitmap)
    g.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.AntiAlias
    g.TextRenderingHint <- System.Drawing.Text.TextRenderingHint.AntiAlias
    
    // Define colors
    let lightSkyBlue = Color.FromArgb(135, 206, 250)
    let brickRed = Color.FromArgb(178, 34, 34)
    let pathColor = Color.FromArgb(173, 216, 230) // Light blue for air
    let birdColor = Color.FromArgb(255, 215, 0) // Gold
    
    // Fill background
    g.Clear(Color.White)
    
    // Draw grid
    for y in 0 .. maxY do
        for x in 0 .. maxX do
            let rect = Rectangle(x * cellSize, y * cellSize, cellSize, cellSize)
            let isPath = List.contains (x, y) path
            let isLastPath = path.Length > 0 && (x, y) = List.last path
            let isWall = walls.Contains(x) && not (empty.Contains((y, x)))
            let isEmpty = empty.Contains((y, x)) || not (walls.Contains(x))
            
            if isLastPath then
                // Draw sky background for bird
                use bgBrush = new SolidBrush(lightSkyBlue)
                g.FillRectangle(bgBrush, rect)
                
                // Draw bird emoji
                use font = new Font("Segoe UI Emoji", 16.0f, FontStyle.Regular)
                use textBrush = new SolidBrush(Color.Black)
                use sf = new StringFormat()
                sf.Alignment <- StringAlignment.Center
                sf.LineAlignment <- StringAlignment.Center
                g.DrawString("🐦", font, textBrush, float32(rect.X + cellSize/2), float32(rect.Y + cellSize/2), sf)
            elif isPath then
                // Draw path as air with wind emoji
                use brush = new SolidBrush(pathColor)
                g.FillRectangle(brush, rect)
                
                // Draw wind emoji
                use font = new Font("Segoe UI Emoji", 14.0f, FontStyle.Regular)
                use textBrush = new SolidBrush(Color.DarkGray)
                use sf = new StringFormat()
                sf.Alignment <- StringAlignment.Center
                sf.LineAlignment <- StringAlignment.Center
                g.DrawString("💨", font, textBrush, float32(rect.X + cellSize/2), float32(rect.Y + cellSize/2), sf)
            elif isWall then
                // Draw brick wall with brick emoji
                use brush = new SolidBrush(brickRed)
                g.FillRectangle(brush, rect)
                
                // Draw brick emoji
                use font = new Font("Segoe UI Emoji", 14.0f, FontStyle.Regular)
                use textBrush = new SolidBrush(Color.Black)
                use sf = new StringFormat()
                sf.Alignment <- StringAlignment.Center
                sf.LineAlignment <- StringAlignment.Center
                g.DrawString("🧱", font, textBrush, float32(rect.X + cellSize/2), float32(rect.Y + cellSize/2), sf)
            elif isEmpty then
                // Draw empty space (light sky blue)
                use brush = new SolidBrush(lightSkyBlue)
                g.FillRectangle(brush, rect)
            
            // Draw grid lines
            use gridPen = new Pen(Color.LightGray, 0.5f)
            g.DrawRectangle(gridPen, rect)
    
    // Add legend
    use legendFont = new Font("Arial", 12.0f, FontStyle.Bold)
    use legendBrush = new SolidBrush(Color.Black)
    let legendY = height + 10
    
    // Extend bitmap to include legend
    use finalBitmap = new Bitmap(width, height + 60)
    use gFinal = Graphics.FromImage(finalBitmap)
    gFinal.Clear(Color.White)
    gFinal.DrawImage(bitmap, 0, 0)
    
    gFinal.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.AntiAlias
    gFinal.TextRenderingHint <- System.Drawing.Text.TextRenderingHint.AntiAlias
    
    // Draw legend
    let legendText = sprintf "Flappy Bird Path - Total Up Presses: %d" (calculatePathCost path)
    gFinal.DrawString(legendText, legendFont, legendBrush, 10.0f, float32 legendY)
    
    // Draw legend items with emojis
    use smallFont = new Font("Segoe UI Emoji", 10.0f)
    use labelFont = new Font("Arial", 10.0f)
    let legendItems = [
        ("🧱", "Wall (Brick)")
        ("💨", "Path (Wind)")
        ("🐦", "Bird")
    ]
    
    let mutable xOffset = 10.0f
    for (emoji, text) in legendItems do
        gFinal.DrawString(emoji, smallFont, legendBrush, xOffset, float32(legendY + 22))
        gFinal.DrawString(text, labelFont, legendBrush, xOffset + 25.0f, float32(legendY + 25))
        xOffset <- xOffset + 150.0f
    
    let fullPath = Path.Combine(VisualizationFolder, filename)
    finalBitmap.Save(fullPath, ImageFormat.Png)
    
    printfn "Visualization saved to: %s" fullPath
    fullPath

let aStar (start: int*int) =
    let isFree (x: int, y: int) =
        empty.Contains((y, x)) || (not (walls.Contains(x)))

    let getNeighbors (x, y) =
        [ 
            (x+1, y-1); // moving up
            (x+1, y+1) // letting it fall
        ]
        |> List.filter (fun (x, y) ->
            x >= 0 && x <= maxX && y >= 0 && y <= maxY && isFree (x, y)
        )
    
    // gScore with number of up presses
    let gScore = Dictionary<int*int, int>()
    let fScore = Dictionary<int*int, int>()
    let parent = Dictionary<int*int, int*int>()
    let closedSet = HashSet<int*int>()
    let openSet = SortedSet<(int * (int*int))>(Comparer<(int * (int*int))>.Create(fun (f1, c1) (f2, c2) ->
        if f1 <> f2 then compare f1 f2 else compare c1 c2
    ))

    gScore[start] <- 0
    fScore[start] <- 0
    openSet.Add((fScore[start], start)) |> ignore

    let mutable found = false
    let mutable goal = (maxX, 0)
    
    while openSet.Count > 0 && not found do
        let (_, current) = openSet.Min
        openSet.Remove((fScore[current], current)) |> ignore
        
        if closedSet.Contains(current) then
            () // already processed - skip duplicate
        elif (fst current) = maxX then
            found <- true // early termination
            goal <- current
        else
            closedSet.Add(current) |> ignore
            for n in getNeighbors current do
                if not (closedSet.Contains(n)) then
                    // +1 if moving up, +0 if letting it fall
                    let moveCost = if (snd n) < (snd current) then 1 else 0
                    let tentativeG = gScore[current] + moveCost
                    
                    if not (gScore.ContainsKey(n)) || tentativeG < gScore[n] then
                        if fScore.ContainsKey(n) then
                            openSet.Remove((fScore[n], n)) |> ignore
                        parent[n] <- current
                        gScore[n] <- tentativeG
                        // Heuristic: estimate remaining up presses needed
                        // Simple heuristic: distance to goal
                        let h = abs(maxX - (fst n))
                        fScore[n] <- tentativeG + h
                        openSet.Add((fScore[n], n)) |> ignore

    let rec buildPath acc pos =
        if pos = start then start :: acc
        elif parent.ContainsKey(pos) then buildPath (pos :: acc) parent[pos]
        else []

    let path = if found then buildPath [] goal else []
    path.Length, path

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent lines
    let pathLength, path = aStar (0, maxY)
    
    // Generate visualization
    visualizeToImage "quest19_1_visualization.png" path |> ignore
    
    //printGrid path
    let cost = calculatePathCost path
    cost