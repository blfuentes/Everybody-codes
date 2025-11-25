module quest16_2_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Drawing
open System.IO
open System.Drawing.Imaging

//let path = "quest16/test_input_02.txt"
let path = "quest16/quest16_input_02.txt"

let parseContent(lines: string) =
    lines.Split(",") |> Array.map int

let deconstructWall (towers: int[]) =
    let spells = ResizeArray<bigint>()
    let simulated = Array.zeroCreate<bigint> (towers.Length)
    let wallLength = towers.Length

    towers
    |> Array.iteri(fun index towerHeight ->
        let tempHeight = simulated[index]
        if tempHeight < towerHeight then
            let diff = int(towerHeight - int(tempHeight))
            let pos = index + 1
            spells.Add(pos)
            for j in index..pos..wallLength-1 do
                simulated[j] <- simulated[j] + bigint(diff)
    )   
    spells

let buildWallVisualization (wall: int[]) (wallLength: int) (filename: string) =  
    // Find max height for scaling
    let maxHeight = if wall.Length > 0 then Array.max wall else 1
    
    // Image settings
    let cellWidth = 20
    let cellHeight = 20
    let width = wallLength * cellWidth
    let height = (maxHeight + 1) * cellHeight
    
    use bitmap = new Bitmap(width, height)
    use g = Graphics.FromImage(bitmap)
    g.Clear(Color.White)
    
    // Configure text rendering for emojis
    g.TextRenderingHint <- System.Drawing.Text.TextRenderingHint.AntiAlias
    use font = new Font("Segoe UI Emoji", 14.0f, FontStyle.Regular)
    use textBrush = new SolidBrush(Color.Brown)
    
    // Draw the wall using brick emoji
    let brickEmoji = "🧱"
    
    for col in 0 .. wallLength - 1 do
        let blockHeight = wall[col]
        for row in 0 .. blockHeight - 1 do
            let x = col * cellWidth
            let y = height - (row + 1) * cellHeight // Draw from bottom up
            
            // Draw emoji centered in cell
            use sf = new StringFormat()
            sf.Alignment <- StringAlignment.Center
            sf.LineAlignment <- StringAlignment.Center
            g.DrawString(brickEmoji, font, textBrush, 
                RectangleF(float32 x, float32 y, float32 cellWidth, float32 cellHeight), sf)
    
    // Draw ground line
    use groundPen = new Pen(Color.DarkGray, 2.0f)
    g.DrawLine(groundPen, 0, height - cellHeight, width, height - cellHeight)
    
    // Ensure folder exists
    let fullPath = Path.Combine(VisualizationFolder, filename)
    bitmap.Save(fullPath, ImageFormat.Png)
    
    fullPath

let calculateWall (towerHeights: int[]) =
    let spells = deconstructWall towerHeights
    spells
    |> Seq.reduce(fun acc i-> 
        acc * i
    )

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let towerHeights = parseContent(lines)
    buildWallVisualization towerHeights towerHeights.Length "quest16_2_visualization.png" |> ignore
    calculateWall towerHeights