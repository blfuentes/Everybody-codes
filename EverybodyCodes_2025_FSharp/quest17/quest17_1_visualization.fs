module quest17_1_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Drawing
open System.Drawing.Imaging
open System.IO

//let path = "quest17/test_input_01.txt"
let path = "quest17/quest17_input_01.txt"

let mutable Xv, Yv = (0, 0)
let mutable maxX, maxY = (0, 0)

let parseContent (lines: string array) =
    maxY <- lines.Length
    maxX <- lines[0].ToCharArray() |> Array.length
    let field = Array2D.create maxY maxX 0
    for y in 0 .. lines.Length - 1 do
        for x in 0 .. lines[y].ToCharArray().Length - 1 do
            if lines[y][x] = '@' then
                Xv <- x
                Yv <- y
                field[y, x] <- 0
            else
                field[y, x] <- int((lines[y][x]).ToString())
    field

let isInRange (Xv, Yv) (Xc, Yc) R =
    (Xv - Xc) * (Xv - Xc) + (Yv - Yc) * (Yv - Yc) <= R * R

let printField (field: int[,]) =
    for y in 0 .. maxY - 1 do
        for x in 0 .. maxX - 1 do
            if (x, y) = (Xv, Yv) then
                printf "@"
            else
                printf "%s" (field[y, x].ToString())
        printfn ""

let calculateRange (field: int[,]) (radius: int) =
    let points = 
        seq {
            for y in 0 .. maxY - 1 do
                for x in 0 .. maxX - 1 do
                if isInRange (Xv, Yv) (x, y) radius then
                    yield field[y, x]
        }
    points |> Seq.sum

let visualizeToPNG (field: int[,]) (radius: int) (filename: string) =
    // Find max value for color scaling
    let maxValue = 
        seq {
            for y in 0 .. maxY - 1 do
                for x in 0 .. maxX - 1 do
                    if (x, y) <> (Xv, Yv) then
                        yield field[y, x]
        } |> Seq.max
    
    let cellSize = 30
    let width = maxX * cellSize
    let height = maxY * cellSize
    
    use bitmap = new Bitmap(width, height)
    use g = Graphics.FromImage(bitmap)
    g.Clear(Color.White)
    
    // Configure text rendering
    g.TextRenderingHint <- System.Drawing.Text.TextRenderingHint.AntiAlias
    use font = new Font("Segoe UI Emoji", float32 cellSize * 0.6f, FontStyle.Regular)
    
    for y in 0 .. maxY - 1 do
        for x in 0 .. maxX - 1 do
            let screenX = x * cellSize
            let screenY = y * cellSize
            
            if (x, y) = (Xv, Yv) then
                // Draw volcano emoji
                use sf = new StringFormat()
                sf.Alignment <- StringAlignment.Center
                sf.LineAlignment <- StringAlignment.Center
                use brush = new SolidBrush(Color.Black)
                g.DrawString("🌋", font, brush, 
                    RectangleF(float32 screenX, float32 screenY, float32 cellSize, float32 cellSize), sf)
            else
                let value = field[y, x]
                let inRange = isInRange (Xv, Yv) (x, y) radius
                
                let cellColor = 
                    if inRange then
                        // Grey shades for points in range
                        let greyValue = 150 + (value * 50 / (maxValue + 1))
                        Color.FromArgb(greyValue, greyValue, greyValue)
                    else
                        // Green shades from light to dark based on value
                        let greenIntensity = 255 - (value * 200 / (maxValue + 1))
                        Color.FromArgb(50, greenIntensity, 50)
                
                use brush = new SolidBrush(cellColor)
                g.FillRectangle(brush, screenX, screenY, cellSize, cellSize)
                
                // Draw value text
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
    let circleX = Xv * cellSize + cellSize / 2 - radius * cellSize
    let circleY = Yv * cellSize + cellSize / 2 - radius * cellSize
    let circleDiameter = radius * 2 * cellSize
    g.DrawEllipse(radiusPen, circleX, circleY, circleDiameter, circleDiameter)
    
    // Ensure folder exists
    let fullPath = Path.Combine(VisualizationFolder, filename)
    bitmap.Save(fullPath, ImageFormat.Png)
    
    //printfn "Visualization saved to: %s" fullPath
    fullPath

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let field = parseContent lines
    
    // Create visualization
    visualizeToPNG field 10 "quest17_1_visualization.png" |> ignore
    
    calculateRange field 10