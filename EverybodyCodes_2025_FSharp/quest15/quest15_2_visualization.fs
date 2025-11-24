module quest15_2_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Drawing
open System.Drawing.Imaging
open System.IO
open System.Threading.Tasks

//let path = "quest15/test_input_02.txt"
let path = "quest15/quest15_input_02.txt"

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

// Cache pens and brushes to avoid repeated allocations
let cachedPens = Dictionary<Color, Pen>()
let cachedBrushes = Dictionary<Color, SolidBrush>()

let getPen (color: Color) =
    match cachedPens.TryGetValue(color) with
    | true, pen -> pen
    | false, _ ->
        let pen = new Pen(color, 1.0f)
        cachedPens.Add(color, pen)
        pen

let getBrush (color: Color) =
    match cachedBrushes.TryGetValue(color) with
    | true, brush -> brush
    | false, _ ->
        let brush = new SolidBrush(color)
        cachedBrushes.Add(color, brush)
        brush

// Get JPEG codec info and set quality
let getJpegCodec () =
    ImageCodecInfo.GetImageEncoders()
    |> Array.find (fun codec -> codec.MimeType = "image/jpeg")

let getEncoderParameters (quality: int64) =
    let parameters = new EncoderParameters(1)
    parameters.Param.[0] <- new EncoderParameter(Encoder.Quality, quality)
    parameters

let saveAsJpeg (bitmap: Bitmap) (fileName: string) (quality: int64) =
    let tempFileName = fileName + ".tmp"
    try
        let codec = getJpegCodec()
        let encoderParams = getEncoderParameters(quality)
        use encoderParams = encoderParams
        bitmap.Save(fileName, codec, encoderParams)
    with
    | ex ->
        if File.Exists(tempFileName) then
            File.Delete(tempFileName)
        printfn "Error saving JPEG to %s: %s" fileName ex.Message
        reraise()

// Use LockBits for faster pixel access
let drawCellsOptimized (bitmap: Bitmap) (minX, minY, maxX, maxY) (pixelSize: int) (visitedSet: Set<int*int>) (walls: Set<int*int>) =
    let bitmapData = bitmap.LockBits(
        System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
        ImageLockMode.WriteOnly,
        PixelFormat.Format24bppRgb)
    
    let stride = bitmapData.Stride
    let ptr = bitmapData.Scan0
    
    // Define color values (BGR format for 24-bit RGB)
    let whitePixel = (byte 255, byte 255, byte 255)  // BGR: White
    let (B, G, R) = (byte 0, byte 165, byte 255)   // BGR: Orange
    let bluePixel = (byte 255, byte 0, byte 0)       // BGR: Blue
    
    let unsafe_draw () =
        for y in minY .. maxY do
            for x in minX .. maxX do
                if walls.Contains(x, y) then
                    // Fill orange rectangle for wall
                    let startScreenX = (x - minX) * pixelSize
                    let startScreenY = (y - minY) * pixelSize
                    
                    for py in 0 .. pixelSize - 1 do
                        for px in 0 .. pixelSize - 1 do
                            let pixelX = startScreenX + px
                            let pixelY = startScreenY + py
                            let offset = pixelY * stride + pixelX * 3
                            System.Runtime.InteropServices.Marshal.WriteInt32(System.IntPtr(ptr.ToInt64() + int64 offset), 
                                ((int B) ||| (int G <<< 8) ||| (int R <<< 16)))
                elif visitedSet.Contains(x, y) then
                    // Draw blue X
                    let startScreenX = (x - minX) * pixelSize
                    let startScreenY = (y - minY) * pixelSize
                    
                    for i in 0 .. pixelSize - 1 do
                        // Main diagonal
                        let offset1 = (startScreenY + i) * stride + (startScreenX + i) * 3
                        System.Runtime.InteropServices.Marshal.WriteByte(System.IntPtr(ptr.ToInt64() + int64 offset1), 255uy) // B
                        
                        // Anti-diagonal
                        let offset2 = (startScreenY + i) * stride + (startScreenX + pixelSize - 1 - i) * 3
                        System.Runtime.InteropServices.Marshal.WriteByte(System.IntPtr(ptr.ToInt64() + int64 offset2), 255uy) // B
    
    unsafe_draw()
    bitmap.UnlockBits(bitmapData)

let visualizeMapBase (minX, minY, maxX, maxY) (visitedPoints: (int*int) list) (walls: Set<int*int>) =
    ensureFolder (Path.Combine(VisualizationFolder, "quest15_2_visualization"))
    
    let pixelSize = 3
    let width = (maxX - minX + 1) * pixelSize
    let height = (maxY - minY + 1) * pixelSize
    
    let bitmap = new Bitmap(width, height)
    use g = Graphics.FromImage(bitmap)
    g.Clear(Color.White)
    
    // Pre-convert visited list to set once
    let visitedSet = Set.ofList visitedPoints
    
    // Pre-allocate pens and brushes
    let wallBrush = getBrush Color.Orange
    let currentBrush = getBrush Color.Yellow
    let currentPen = getPen Color.Black
    let visitedPen = getPen Color.Blue
    
    // Draw all cells in a single pass using Graphics (still fast for this purpose)
    for y in minY .. maxY do
        for x in minX .. maxX do
            let screenX = (x - minX) * pixelSize
            let screenY = (y - minY) * pixelSize
            
            if walls.Contains(x, y) then
                g.FillRectangle(wallBrush, screenX, screenY, pixelSize, pixelSize)
            elif visitedSet.Contains(x, y) then
                g.DrawLine(visitedPen, screenX, screenY, screenX + pixelSize, screenY + pixelSize)
                g.DrawLine(visitedPen, screenX + pixelSize, screenY, screenX, screenY + pixelSize)
    
    g.Dispose()

    // Save as JPEG with quality 85 (good balance between size and quality)
    let fileName = Path.Combine(VisualizationFolder, "quest15_2_visualization", "frame_base.jpg")
    saveAsJpeg bitmap fileName 85L
    bitmap.Dispose()

    fileName


let visualizeMap (minX, minY, maxX, maxY) (visitedPoints: (int*int) list) (walls: Set<int*int>) (currentPoint: int*int) (frameNumber: int) =
    let pixelSize = 3
    let fileName = Path.Combine(VisualizationFolder, "quest15_2_visualization", $"frame_{frameNumber:D4}.jpg")
    
    try
        // Load the existing base frame
        use bitmap = new Bitmap(fileName)
        use g = Graphics.FromImage(bitmap)
        
        // Pre-allocate pens and brushes
        let currentBrush = getBrush Color.Yellow
        let currentPen = getPen Color.Black
        
        // Draw current point last (on top)
        if not (walls.Contains(currentPoint)) then
            let (x, y) = currentPoint
            let screenX = (x - minX) * pixelSize
            let screenY = (y - minY) * pixelSize
            if pixelSize >= 3 then
                g.FillEllipse(currentBrush, screenX, screenY, pixelSize, pixelSize)
                g.DrawEllipse(currentPen, screenX, screenY, pixelSize, pixelSize)
            else
                // For very small pixels, just fill the rectangle
                g.FillRectangle(currentBrush, screenX, screenY, pixelSize, pixelSize)
        
        g.Dispose()
        
        // IMPORTANT: Save to temporary file first, then move
        let tempFileName = fileName + ".tmp"
        saveAsJpeg bitmap tempFileName 85L
        
        // Now bitmap is done being used, dispose it
        bitmap.Dispose()
        
        // Now we can safely replace the original file
        if File.Exists(fileName) then
            File.Delete(fileName)
        File.Move(tempFileName, fileName, true)
        
        fileName
    with
    | ex ->
        printfn "Error in visualizeMap for frame %d: %s" frameNumber ex.Message
        reraise()

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let operations = parseContent lines
    let (xStart, yStart, xEnd, yEnd, wall) = buildMap operations
    let (length, path ) = bfs (xEnd, yEnd) (xStart, yStart) (Set.remove (xStart, yStart) wall)
    
    // Pre-compute all visited paths sequentially (this is necessary)
    let allFrameData = 
        path
        |> List.mapi (fun i point -> 
            let visitedUpToNow = path |> List.take (i + 1)
            (i, point, visitedUpToNow)
        )
    
    // Visualize the path in parallel with chunking
    let minX = wall |> Seq.map fst |> Seq.min |> fun x -> x - 1
    let maxX = wall |> Seq.map fst |> Seq.max |> fun x -> x + 1
    let minY = wall |> Seq.map snd |> Seq.min |> fun y -> y - 1
    let maxY = wall |> Seq.map snd |> Seq.max |> fun y -> y + 1
    
    let width = (maxX - minX + 1) * 3
    let height = (maxY - minY + 1) * 3
    //printfn "Bitmap dimensions: %dx%d pixels" width height

    // Create base frame
    visualizeMapBase (minX, minY, maxX, maxY) [] wall |> ignore

    // Copy base frame for each frame in the sequence
    let baseFramePath = Path.Combine(VisualizationFolder, "quest15_2_visualization", "frame_base.jpg")
    for i in 0 .. path.Length - 1 do
        let targetFramePath = Path.Combine(VisualizationFolder, "quest15_2_visualization", $"frame_{i:D4}.jpg")
        File.Copy(baseFramePath, targetFramePath, true)

    // Draw current point on each frame
    allFrameData
    |> List.iter (fun (i, point, visitedUpToNow) ->
        let fileName = visualizeMap (minX, minY, maxX, maxY) visitedUpToNow wall point i
        ignore()
        //if (i + 1) % 100 = 0 then
        //    printfn "Saved frame %d of %d" (i + 1) path.Length
    )
    
    //printfn "Visualization complete: %d frames" path.Length
    length