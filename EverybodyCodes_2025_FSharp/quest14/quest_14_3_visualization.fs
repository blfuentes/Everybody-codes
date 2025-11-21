module quest14_3_visualization

open EverybodyCodes_2025_FSharp.Modules
open System
open System.Collections.Generic
open System.Drawing
open System.Drawing.Imaging

//let path = "quest14/test_input_03.txt"
let path = "quest14/quest14_input_03.txt"

type State = Active | Inactive
type Position = { Row: int; Col: int }
type Cell = { Position: Position; State: State }

let mutable maxRows = 34
let mutable maxCols = 34
let mutable maxPatternRows = 0
let mutable maxPatternCols = 0

let mutable fromRowMatch = 0
let mutable toRowMatch = 0
let mutable fromColMath = 0
let mutable toColMatch = 0

let neighbors = [| (-1,-1); (-1,1); (1,-1); (1,1) |]

let parseContent (lines: string array) =
    maxPatternRows <- lines.Length
    maxPatternCols <- lines[0].Length
    let grid: Cell[,] = Array2D.zeroCreate maxPatternRows maxPatternCols
    lines
    |> Array.iteri (fun row line ->
        line.ToCharArray()
        |> Array.iteri (fun col ch ->
            let state =
                match ch with
                | '#' -> Active
                | '.' -> Inactive
                | _ -> failwithf "Invalid character '%c' in input." ch
            grid[row, col] <- { Position = { Row = row; Col = col }; State = state }))
    grid

let countActiveNeighbours (grid: Cell[,]) row col =
    neighbors
    |> Array.sumBy (fun (dRow, dCol) ->
        let nRow, nCol = row + dRow, col + dCol
        if nRow >= 0 && nRow < maxRows && nCol >= 0 && nCol < maxCols && 
            grid[nRow, nCol].State = Active then 1 else 0)

let step (grid: Cell[,]) =
    Array2D.init maxRows maxCols (fun row col ->
        let live = countActiveNeighbours grid row col
        match grid[row, col], live with
        | cell, count when ((cell.State = Active) && count % 2 = 1) -> 
            { Position = cell.Position; State = Active }
        | cell, count when cell.State = Inactive && (count % 2 = 0) -> 
            { Position = cell.Position; State = Active }
        | _ -> 
            { Position = { Row = row; Col = col }; State = Inactive })

let printGrid (grid: Cell[,]) =
    let buffer = System.Text.StringBuilder()
    //Console.Clear()
    for row in 0 .. maxRows - 1 do
        for col in 0 .. maxCols - 1 do
            let ch =
                match grid[row, col].State with
                | Active -> '#'
                | Inactive -> '.'
            buffer.Append(ch) |> ignore
        buffer.AppendLine() |> ignore
    Console.Write(buffer.ToString())
    //Thread.Sleep(500)


let countActiveCells (grid: Cell[,]) =
    let mutable count = 0I
    for row in 0 .. maxRows - 1 do
        for col in 0 .. maxCols - 1 do
            if grid[row, col].State = Active then
                count <- count + 1I
    count

let centerOfGridMatches (grid: Cell[,]) (pattern: Cell[,]) =
    let startRow = (maxRows - Array2D.length1 pattern) / 2
    let startCol = (maxCols - Array2D.length2 pattern) / 2
    let mutable matches = true
    for row in 0 .. maxPatternRows - 1 do
        for col in 0 .. maxPatternCols - 1 do
            if grid[startRow + row, startCol + col].State <> pattern[row, col].State then
                matches <- false
    matches

let gridHashKey (grid: Cell[,]) =    
    let sb = System.Text.StringBuilder(maxRows * maxCols)
    for r in 0 .. maxRows - 1 do
        for c in 0 .. maxCols - 1 do
            let ch = 
                match grid[r, c].State with
                | Active -> "#"
                | Inactive -> "."
                | _ -> failwith "state not vali..."
            sb.Append(ch) |> ignore
    sb.ToString()

let saveToBitmap (grid: Cell[,]) (filename: string) (ismatch: bool) =
    let cellSize = 10
    let width = maxCols * cellSize
    let height = maxRows * cellSize
    use bmp = new Bitmap(width, height)
    use g = Graphics.FromImage(bmp)
    g.Clear(Color.White)
    
    for row in 0 .. maxRows - 1 do
        for col in 0 .. maxCols - 1 do
            let brush = 
                match grid[row, col].State with
                | Active -> 
                    if ismatch then
                        if row >= fromRowMatch && row <= toRowMatch &&
                           col >= fromColMath && col <= toColMatch then
                            new SolidBrush(Color.Red) 
                        else                            
                            new SolidBrush(Color.Black)
                    else 
                        new SolidBrush(Color.Black)
                | Inactive -> 
                    if ismatch then 
                        if row >= fromRowMatch && row <= toRowMatch &&
                           col >= fromColMath && col <= toColMatch then
                            new SolidBrush(Color.Blue) 
                        else                                                       
                            new SolidBrush(Color.White)
                    else 
                        new SolidBrush(Color.White)
            g.FillRectangle(brush, col * cellSize, row * cellSize, cellSize, cellSize)
            brush.Dispose()
    
    //// Draw grid lines
    //use pen = new Pen(Color.LightGray, 1.0f)
    //for row in 0 .. maxRows do
    //    g.DrawLine(pen, 0, row * cellSize, width, row * cellSize)
    //for col in 0 .. maxCols do
    //    g.DrawLine(pen, col * cellSize, 0, col * cellSize, height)
    
    if System.IO.Directory.Exists("quest14_visualization") |> not then
        System.IO.Directory.CreateDirectory("quest14_visualization") |> ignore

    bmp.Save(filename, ImageFormat.Png)


let run pattern baseGrid numOfSteps =
    let mutable currentGrid = baseGrid
    let mutable activeCount = 0I
    let mutable stepNum = 0I
    let stateHistory = new Dictionary<string, bigint>()
    let mutable cycleDetected = false
    
    while stepNum < numOfSteps && not cycleDetected do
        currentGrid <- step currentGrid
        stepNum <- stepNum + 1I
        
        if centerOfGridMatches currentGrid pattern then
            activeCount <- activeCount + countActiveCells currentGrid
        
        let key = gridHashKey currentGrid
        
        if stateHistory.ContainsKey(key) then
            // find cycle
            let cycleStart = stateHistory[key]
            let cycleLength = stepNum - cycleStart
            
            //printfn "Detected cycle at step %A, cycle starts at step %A, cycle length: %A" stepNum cycleStart cycleLength
            //printfn "Actives accumulated so far: %A" activeCount
            
            // calculate actives in one complete cycle
            let mutable cycleActiveCount = 0I
            let cycleStartGrid = currentGrid
            for i in 1I .. cycleLength do
                currentGrid <- step currentGrid
                // save each bitmap of state

                //
                if centerOfGridMatches currentGrid pattern then
                    for j in 1..5 do
                        saveToBitmap currentGrid (sprintf "quest14_visualization/step_%A_%A.png" (stepNum + i) j) true
                    cycleActiveCount <- cycleActiveCount + countActiveCells currentGrid
                else
                    saveToBitmap currentGrid (sprintf "quest14_visualization/step_%A_0.png" (stepNum + i)) false
            
            //printfn "Actives per complete cycle: %A" cycleActiveCount
            
            // calculate remaining steps
            let remainingSteps = numOfSteps - stepNum
            let fullCycles = remainingSteps / cycleLength
            let leftoverSteps = remainingSteps % cycleLength
            
            //printfn "Remaining steps: %A, Full cycles: %A, Leftover steps: %A" remainingSteps fullCycles leftoverSteps
            
            // add full cycles
            activeCount <- activeCount + (cycleActiveCount * fullCycles)
            
            // count remaining stepe
            currentGrid <- cycleStartGrid
            for i in 1I .. leftoverSteps do
                currentGrid <- step currentGrid
                if centerOfGridMatches currentGrid pattern then
                    activeCount <- activeCount + countActiveCells currentGrid
            
            //printfn "Total active count: %A" activeCount
            //let missing = 278388552I - activeCount
            //printfn "Missing to match expected: %A" missing
            
            cycleDetected <- true
        else
            stateHistory[key] <- stepNum
    
    activeCount


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let pattern = parseContent lines
    let baseGrid = Array2D.init maxRows maxCols (fun row col ->
        { Position = { Row = row; Col = col }; State = Inactive })
    fromRowMatch <- (maxRows - maxPatternRows) / 2
    toRowMatch <- fromRowMatch + maxPatternRows - 1
    fromColMath <- (maxCols - maxPatternCols) / 2
    toColMatch <- fromColMath + maxPatternCols - 1
    run pattern baseGrid 1000000000I