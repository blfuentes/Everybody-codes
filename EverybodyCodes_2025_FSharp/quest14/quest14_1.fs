module quest14_1

open EverybodyCodes_2025_FSharp.Modules
open System

//let path = "quest14/test_input_01.txt"
let path = "quest14/quest14_input_01.txt"

type State = Active | Inactive
type Position = { Row: int; Col: int }
type Cell = { Position: Position; State: State }

let mutable maxRows = 0
let mutable maxCols = 0
let neighbors = [| (-1,-1); (-1,1); (1,-1); (1,1) |]

let parseContent (lines: string array) =
    maxRows <- lines.Length
    maxCols <- lines[0].Length
    let grid: Cell[,] = Array2D.zeroCreate maxRows maxCols
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
    let mutable count = 0
    for row in 0 .. maxRows - 1 do
        for col in 0 .. maxCols - 1 do
            if grid[row, col].State = Active then
                count <- count + 1
    count

let run grid numOfSteps=
    let mutable currentGrid = grid
    let mutable activeCount = 0
    for round in 1 .. numOfSteps do
        //printfn "Round %d" round
        currentGrid <- step currentGrid
        activeCount <- activeCount + countActiveCells currentGrid
        //printGrid currentGrid
        //printfn "Active Cells: %d" activeCount
    activeCount


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let grid = parseContent lines
    run grid 10
