module quest08_2_visualization

open System
open EverybodyCodes_2025_FSharp.Modules
open System.Drawing
open System.Drawing.Drawing2D
open System.Drawing.Imaging

//let path = "quest08/test_input_02.txt"
let path = "quest08/quest08_input_02.txt"

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

let intersection (x1, y1, x2, y2) (x3, y3, x4, y4) =
    let a1 = y2 - y1
    let b1 = x1 - x2
    let c1 = a1 * x1 + b1 * y1

    let a2 = y4 - y3
    let b2 = x3 - x4
    let c2 = a2 * x3 + b2 * y3

    let determinant = a1 * b2 - a2 * b1

    if determinant = 0.0 then
        None
    else
        let x = Math.Round((b2 * c1 - b1 * c2) / determinant, 2)
        let y = Math.Round((a1 * c2 - a2 * c1) / determinant, 2)
        Some (x, y)

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

let findCollisions (nails: int array) (points: (int * (float * float)) list) =
    let mapping = points |> dict

    let threads = ResizeArray<(float * float * float * float)>()
    let mutable knots = 0
    nails
    |> Array.pairwise
    |> Array.iter(fun (a, b) ->
        let (x1, y1) = mapping[a]
        let (x2, y2) = mapping[b]
        let thread = (x1, y1, x2, y2)
        for otherThread in threads do
            let (x3, y3, x4, y4) = otherThread
            match intersection thread otherThread with
            | Some (ix, iy) ->
                // Exclude intersections at the endpoints
                // exclude if they intersect farther than the endpoints
                let exclude =
                    (ix = x1 && iy = y1) || (ix = x2 && iy = y2) ||
                    (ix = x3 && iy = y3) || (ix = x4 && iy = y4) ||
                    ( (ix < Math.Min(x1, x2)) || (ix > Math.Max(x1, x2)) ||
                      (iy < Math.Min(y1, y2)) || (iy > Math.Max(y1, y2)) ||
                      (ix < Math.Min(x3, x4)) || (ix > Math.Max(x3, x4)) ||
                      (iy < Math.Min(y3, y4)) || (iy > Math.Max(y3, y4)) )

                if not exclude then
                    knots <- knots + 1
            | None -> ()
        threads.Add(thread)
    )
    knots

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

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let nails = parseContent lines
    //let numOfNails = 8
    let numOfNails = 256
    let circle = generateCirclePoints numOfNails (float(numOfNails) / 4.)

    // Generate visualization
    let outputPath = "quest08_02_threads_visualization.png"
    visualizeThreads nails circle outputPath

    findCollisions nails circle