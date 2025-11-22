module quest15_3

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System

//let path = "quest15/test_input_03.txt"
let path = "quest15/quest15_input_03.txt"

type Op =
    | Right of int
    | Left of int

let parseContent (lines: string) =
    lines.Split(",")
    |> Array.map (fun raw ->
        let line = raw.Trim()
        match line[0] with
        | 'R' -> Right (int (line.Substring(1)))
        | 'L' -> Left (int (line.Substring(1)))
        | _ -> failwith "Unknown operation"
    )

// build walls in form of line from (x,y) to (xn,yn)
let buildLongWalls (operations: Op array) : int * int * int * int * (int*int*int*int) list =
    let xStart, yStart = 0, 0
    let mutable dx, dy = 0, 1
    let segments = ResizeArray<int*int*int*int>()
    let mutable x, y = xStart, yStart
    for op in operations do
        match op with
        | Left n ->
            let ndx, ndy = -dy, dx
            dx <- ndx; dy <- ndy
            let xn = x + n * dx
            let yn = y + n * dy
            segments.Add (x, y, xn, yn)
            x <- xn; y <- yn
        | Right n ->
            let ndx, ndy = dy, -dx
            dx <- ndx; dy <- ndy
            let xn = x + n * dx
            let yn = y + n * dy
            segments.Add (x, y, xn, yn)
            x <- xn; y <- yn
    let xEnd, yEnd = x, y
    (xStart, yStart, xEnd, yEnd, List.ofSeq segments)

type CompressCoord(originalValues: seq<int64>, ?nBits:int) =
    let nBits = defaultArg nBits 64
    // generate random 64-bit value for compression
    let randBits : int64 =
        let bytes = Array.zeroCreate<byte> 8
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes)
        BitConverter.ToInt64(bytes, 0)

    let origVals = ResizeArray<int64>()
    let map = Dictionary<int64,int>()

    do
        originalValues
        |> Seq.map int64
        |> Seq.sort
        |> Seq.iter (fun x ->
            if origVals.Count = 0 || origVals[origVals.Count - 1] <> x then
                origVals.Add x
                map.Add ((x ^^^ randBits), map.Count)
        )

    // number of distinct original values
    member _.N = origVals.Count

    member _.CompValue(originalValue:int64) : int =
        map[originalValue ^^^ randBits]

    member _.OValue(compressedValue:int) : int64 =
        origVals[compressedValue]

    member _.NCompValues() : int =
        origVals.Count


// dijkstra over compressed coordinates with Manhattan-like edge weights between original coordinates
let distanceToStart (xStart:int) (yStart:int) (xEnd:int) (yEnd:int)
              (wall: HashSet<(int*int)>)
              (compressionX: CompressCoord)
              (compressionY: CompressCoord) : int64 =
    // compressed boundaries
    let xMin = 0
    let xMax = compressionX.NCompValues() - 1
    let yMin = 0
    let yMax = compressionY.NCompValues() - 1

    let dist = Dictionary<(int*int), int64>()
    let getDist key =
        match dist.TryGetValue(key) with
        | true, v -> v
        | _ -> Int64.MaxValue
    dist[(xStart, yStart)] <- 0L

    let prioQueue = ResizeArray<int64 * int * int>()
    let swap i j =
        let t = prioQueue[i]
        prioQueue[i] <- prioQueue[j]
        prioQueue[j] <- t

    let keyOf (a,_,_) = a

    let rec siftUp (idx:int) =
        let mutable i = idx
        while i > 0 do
            let parent = (i - 1) / 2
            if keyOf prioQueue[i] < keyOf prioQueue[parent] then
                swap i parent
                i <- parent
            else i <- 0

    let rec siftDown (idx:int) =
        let mutable i = idx
        let mutable cont = true
        while cont do
            let left = 2 * i + 1
            let right = left + 1
            let mutable smallest = i
            let n = prioQueue.Count
            if left < n && keyOf prioQueue[left] < keyOf prioQueue[smallest] then smallest <- left
            if right < n && keyOf prioQueue[right] < keyOf prioQueue[smallest] then smallest <- right
            if smallest <> i then
                swap i smallest
                i <- smallest
            else cont <- false

    let doPush (d:int64) (x:int) (y:int) =
        prioQueue.Add (d, x, y)
        siftUp (prioQueue.Count - 1)

    let doPop () : (int64 * int * int) =
        let root = prioQueue[0]
        let last = prioQueue[prioQueue.Count - 1]
        prioQueue[0] <- last
        prioQueue.RemoveAt(prioQueue.Count - 1)
        if prioQueue.Count > 0 then siftDown 0
        root

    doPush 0L xStart yStart
    let mutable found : int64 option = None
    while prioQueue.Count > 0 && found.IsNone do
        let d, x, y = doPop()
        if d <> getDist (x, y) then () else
        if x = xEnd && y = yEnd then
            found <- Some d
        else
            // explore neighbors
            let neighbors = [| (x - 1, y); (x + 1, y); (x, y - 1); (x, y + 1) |]
            for (xn, yn) in neighbors do
                if xn < xMin || xn > xMax || yn < yMin || yn > yMax then () else
                let ox = compressionX.OValue(x)
                let oxn = compressionX.OValue(xn)
                let oy = compressionY.OValue(y)
                let oyn = compressionY.OValue(yn)
                let dx = abs (ox - oxn)
                let dy = abs (oy - oyn)
                let nd = d + dx + dy
                if ((xn, yn) = (xEnd, yEnd)) || not (wall.Contains (xn, yn)) then
                    let cur = getDist (xn, yn)
                    if nd < cur then
                        dist[(xn, yn)] <- nd
                        doPush nd xn yn
    match found with
    | Some v -> v
    | None -> failwith "No path found"

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let operations = parseContent lines

    // build wall lines
    let (xStart, yStart, xEnd, yEnd, wallLines) = buildLongWalls operations

    // gather x and y values (with +/-1 neighbors) as distinct int64 sequences
    let xValuesSeq =
        wallLines
        |> Seq.collect (fun (x1, y1, x2, y2) -> seq { x1 - 1; x1; x1 + 1; x2 - 1; x2; x2 + 1 })
        |> Seq.map int64
        |> Seq.distinct

    let yValuesSeq =
        wallLines
        |> Seq.collect (fun (x1, y1, x2, y2) -> seq { y1 - 1; y1; y1 + 1; y2 - 1; y2; y2 + 1 })
        |> Seq.map int64
        |> Seq.distinct

    let compressionX = CompressCoord(xValuesSeq)
    let compressionY = CompressCoord(yValuesSeq)

    let xStartComp = compressionX.CompValue(int64 xStart)
    let xEndComp = compressionX.CompValue(int64 xEnd)
    let yStartComp = compressionY.CompValue(int64 yStart)
    let yEndComp = compressionY.CompValue(int64 yEnd)

    // build compressed wall set by generating all compressed coordinates covered by segments
    let wallSeq =
        wallLines
        |> Seq.collect (fun (x1, y1, x2, y2) ->
            let x1c = compressionX.CompValue(int64 x1)
            let y1c = compressionY.CompValue(int64 y1)
            let x2c = compressionX.CompValue(int64 x2)
            let y2c = compressionY.CompValue(int64 y2)
            let xlo = min x1c x2c
            let xhi = max x1c x2c
            let ylo = min y1c y2c
            let yhi = max y1c y2c
            seq { for xc in xlo .. xhi do for yc in ylo .. yhi -> (xc, yc) }
        )
    let compresessWall = HashSet<(int * int)>(wallSeq)

    let result = distanceToStart xStartComp yStartComp xEndComp yEndComp compresessWall compressionX compressionY
    bigint result