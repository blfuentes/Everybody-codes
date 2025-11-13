module quest08_part03_visualization

open System
open System.Drawing
open System.Drawing.Imaging
open EverybodyCodes_2025_FSharp.Modules
open System.Drawing.Drawing2D

//let path = "quest08/test_input_03.txt"
let path = "quest08/quest08_input_03.txt"

let parseContent (lines: string) =
    lines.Split(",") |> Array.map int

let findCenterTimes (nails: int array) (numOfNails: int) =
    nails
    |> Array.pairwise
    |> Array.groupBy(fun (a, b) ->
        abs(a - b)
    )
    |> Array.filter(fun (a, _) -> a > 0)
    |> Array.map (fun (a, b) -> b.Length)
    |> Array.sum

let generateCirclePoints (n: int) (r: float) : (int * (float * float)) list =
    [ for i in 0 .. n - 1 ->
        let angle = 2.0 * Math.PI * float i / float n
        let x = Math.Round(r * cos angle, 2)
        let y = Math.Round(r * sin angle, 2)
        (i+1, (x, y)) ]

[<Struct>]
type Thread = {
    X1: float; Y1: float
    X2: float; Y2: float
    MinX: float; MaxX: float
    MinY: float; MaxY: float
    A: float; B: float; C: float
}

let inline createThread x1 y1 x2 y2 =
    let a = y2 - y1
    let b = x1 - x2
    let c = a * x1 + b * y1
    {
        X1 = x1; Y1 = y1
        X2 = x2; Y2 = y2
        MinX = min x1 x2; MaxX = max x1 x2
        MinY = min y1 y2; MaxY = max y1 y2
        A = a; B = b; C = c
    }

let drawThreadsToBitmap (threads: Thread array) (outputPath: string) =
    // Find bounds for all threads
    let allX = threads |> Array.collect (fun t -> [|t.X1; t.X2|])
    let allY = threads |> Array.collect (fun t -> [|t.Y1; t.Y2|])
    let minX = Array.min allX
    let maxX = Array.max allX
    let minY = Array.min allY
    let maxY = Array.max allY
    
    // Add padding and calculate scale
    let padding = 50.0
    let width = 2000
    let height = 2000
    let scaleX = float(width - int(2.0 * padding)) / (maxX - minX)
    let scaleY = float(height - int(2.0 * padding)) / (maxY - minY)
    let scale = min scaleX scaleY
    
    // Transform function to convert world coordinates to screen coordinates
    let transform x y =
        let screenX = int(padding + (x - minX) * scale)
        let screenY = int(padding + (y - minY) * scale)
        (screenX, screenY)
    
    use bitmap = new Bitmap(width, height)
    use graphics = Graphics.FromImage(bitmap)
    
    // Set high quality rendering
    graphics.SmoothingMode <- SmoothingMode.AntiAlias
    graphics.Clear(Color.White)
    
    // Draw threads
    //use threadPen = new Pen(Color.FromArgb(100, 0, 100, 200), 1.0f)
    use threadPen = new Pen(Color.FromArgb(100, 0, 100, 200), 1.0f)
    for t in threads do
        let (x1, y1) = transform t.X1 t.Y1
        let (x2, y2) = transform t.X2 t.Y2
        graphics.DrawLine(threadPen, x1, y1, x2, y2)
    
    // Draw circle points
    use pointBrush = new SolidBrush(Color.Red)
    let pointRadius = 3
    for t in threads do
        let (x1, y1) = transform t.X1 t.Y1
        let (x2, y2) = transform t.X2 t.Y2
        graphics.FillEllipse(pointBrush, x1 - pointRadius, y1 - pointRadius, pointRadius * 2, pointRadius * 2)
        graphics.FillEllipse(pointBrush, x2 - pointRadius, y2 - pointRadius, pointRadius * 2, pointRadius * 2)
    
    // Save bitmap
    bitmap.Save(outputPath, ImageFormat.Png)

let visualizeThreads (nails: int array) (points: (int * (float * float)) list) (outputPath: string) =
    let mapping = points |> dict
    
    // Build threads
    let threads =
        nails
        |> Array.pairwise
        |> Array.map(fun (a, b) ->
            let (x1, y1) = mapping[a]
            let (x2, y2) = mapping[b]
            createThread x1 y1 x2 y2)
    
    drawThreadsToBitmap threads outputPath

let findCollisions (nails: int array) (points: (int * (float * float)) list) =
    let mapping = points |> dict
    
    // Build existing threads as structs for better performance
    let existingThreads =
        nails
        |> Array.pairwise
        |> Array.map(fun (a, b) ->
            let (x1, y1) = mapping[a]
            let (x2, y2) = mapping[b]
            createThread x1 y1 x2 y2)
    
    let maxIntersections =
        let numPoints = mapping.Count
        let mutable maxCount = 0
        
        for i in 1 .. numPoints - 1 do
            for j in i + 1 .. numPoints do
                let (x1, y1) = mapping[i]
                let (x2, y2) = mapping[j]
                let minX1 = min x1 x2
                let maxX1 = max x1 x2
                let minY1 = min y1 y2
                let maxY1 = max y1 y2
                
                let mutable knots = 0
                
                for t in existingThreads do
                    // Quick bounding box check - skip if boxes don't overlap
                    if not (maxX1 < t.MinX || t.MaxX < minX1 || maxY1 < t.MinY || t.MaxY < minY1) then
                        // Exact overlap check from original
                        if ((x1 = t.X1 && y1 = t.Y1) || (x2 = t.X1 && y2 = t.Y1)) && 
                           ((x1 = t.X2 && y1 = t.Y2) || (x2 = t.X2 && y2 = t.Y2)) then
                            knots <- knots + 1
                        
                        // Intersection check using pre-computed coefficients
                        let a1 = y2 - y1
                        let b1 = x1 - x2
                        let c1 = a1 * x1 + b1 * y1

                        let determinant = a1 * t.B - t.A * b1

                        if determinant <> 0.0 then
                            let ix = Math.Round((t.B * c1 - b1 * t.C) / determinant, 2)
                            let iy = Math.Round((a1 * t.C - t.A * c1) / determinant, 2)
                            
                            let exclude =
                                (ix = x1 && iy = y1) || (ix = x2 && iy = y2) ||
                                (ix = t.X1 && iy = t.Y1) || (ix = t.X2 && iy = t.Y2) ||
                                (ix < minX1) || (ix > maxX1) ||
                                (iy < minY1) || (iy > maxY1) ||
                                (ix < t.MinX) || (ix > t.MaxX) ||
                                (iy < t.MinY) || (iy > t.MaxY)

                            if not exclude then
                                knots <- knots + 1
                
                if knots > maxCount then
                    maxCount <- knots
        
        maxCount
    
    maxIntersections

let printCircle(points: (float * float) list) =
    points
    |> List.iteri (fun i (x, y) ->
    printfn "Point %2d: (%.4f, %.4f)" (i + 1) x y)

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let nails = parseContent lines
    let numOfNails = 256
    let circle = generateCirclePoints numOfNails (float(numOfNails) / 4.)
    
    // Generate visualization
    let outputPath = "quest08_threads_visualization.png"
    visualizeThreads nails circle outputPath
    
    // Run collision detection
    findCollisions nails circle