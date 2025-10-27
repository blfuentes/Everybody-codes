module quest20_part03

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest20/test_input_03.txt"
let path = "quest20/quest20_input_03.txt"

type Direction =
    | UP = 0
    | RIGHT = 1
    | DOWN = 2
    | LEFT = 3

type CellType =
    | Wall
    | Down
    | Up
    | Empty

[<Struct>]
type Glider = {
    X: int
    Y: int
    Facing: Direction
    Altitude: int
}

module Glider =
    let turnLeft (glider: Glider) =
        let newFacingValue = (int glider.Facing - 1 + 4) % 4
        { glider with Facing = enum<Direction>(newFacingValue) }

    let turnRight (glider: Glider) =
        let newFacingValue = (int glider.Facing + 1) % 4
        { glider with Facing = enum<Direction>(newFacingValue) }

    let canMove (glider: Glider) (grid: CellType[][]) =
        match glider.Facing with
        | Direction.UP -> glider.Y > 0 && grid[glider.Y - 1][glider.X] <> CellType.Wall
        | Direction.RIGHT -> glider.X < grid[0].Length - 1 && grid[glider.Y][glider.X + 1] <> CellType.Wall
        | Direction.DOWN -> glider.Y < grid.Length - 1 && grid[glider.Y + 1][glider.X] <> CellType.Wall
        | Direction.LEFT -> glider.X > 0 && grid[glider.Y][glider.X - 1] <> CellType.Wall

    let move (glider: Glider) (grid: CellType[][]) =
        let newPos =
            match glider.Facing with
            | Direction.UP -> (glider.X, glider.Y - 1)
            | Direction.RIGHT -> (glider.X + 1, glider.Y)
            | Direction.DOWN -> (glider.X, glider.Y + 1)
            | Direction.LEFT -> (glider.X - 1, glider.Y)
        
        let newX, newY = newPos
        let cell = grid[newY][newX]
        
        let newAltitude =
            match cell with
            | CellType.Up -> glider.Altitude + 1
            | CellType.Down -> glider.Altitude - 2
            | _ -> glider.Altitude - 1
        
        { glider with X = newX; Y = newY; Altitude = newAltitude }

    let isFinished (glider: Glider) (grid: CellType[][]) =
        glider.Y = grid.Length - 1

let parseContent (lines: string array) =
    let mutable origStart = (0, 0)
    let baseGrid =
        lines
        |> Array.mapi (fun y line ->
            line.ToCharArray()
            |> Array.mapi (fun x char ->
                match char with
                | '.' -> CellType.Empty
                | '#' -> CellType.Wall
                | 'S' -> origStart <- (x, y); CellType.Empty
                | '-' -> CellType.Down
                | _ -> CellType.Up
            )
        )
    
    let expandedGrid = Array.concat [baseGrid; baseGrid; baseGrid]
    (expandedGrid, origStart)

let bfs (startX, startY, startAlt) (grid: CellType[][]) (altLoss: Dictionary<int, (int * int * int * bool)>) =
    let visited = Dictionary<Glider, int>()
    let stack = Stack<Glider>()
    let initialGlider = { X = startX; Y = startY; Facing = Direction.DOWN; Altitude = startAlt }
    stack.Push(initialGlider)
    
    let mutable maxDist = 0

    while stack.Count > 0 do
        let g = stack.Pop()

        if g.Altitude <= 0 then
            if altLoss.ContainsKey(startX) then
                let _, _, currentBestY, didFinish = altLoss[startX]
                if not didFinish && g.Y > currentBestY then
                    maxDist <- max maxDist g.Y
                    altLoss[startX] <- (altLoss[startX] |> fun (a,b,_,d) -> (a, g.X, g.Y, d))
        elif not (visited.ContainsKey(g)) then
            visited.Add(g, g.Altitude)

            if Glider.isFinished g grid then
                if altLoss.ContainsKey(startX) then
                    let loss = startAlt - g.Altitude + 1
                    let currentBestLoss, _, _, _ = altLoss[startX]
                    if loss < currentBestLoss then
                        altLoss[startX] <- (loss, g.X, g.Y, true)
            else
                let moves =
                    [
                        g // Forward
                        Glider.turnLeft g // Turn Left
                        Glider.turnRight g // Turn Right
                    ]
                    |> List.filter (fun gl -> Glider.canMove gl grid)
                    |> List.map (fun gl -> Glider.move gl grid)
                
                moves |> List.iter stack.Push
    
    maxDist

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let grid, origStart = parseContent lines
    let gridHeight = grid.Length

    let altitudeLoss =
        [0 .. grid[0].Length - 1]
        |> List.filter (fun x -> grid[0][x] <> CellType.Wall)
        |> List.map (fun x -> KeyValuePair(x, (10000, 0, 0, false)))
        |> fun kvs -> new Dictionary<int, (int * int * int * bool)>(kvs)

    let startQueue = Queue<Glider>()
    let initialSegmentAlt = gridHeight + 1
    startQueue.Enqueue({ X = fst origStart; Y = 0; Facing = Direction.DOWN; Altitude = initialSegmentAlt })
    
    let visitedStarts = HashSet<int>()

    while startQueue.Count > 0 do
        let start = startQueue.Dequeue()
        if not (visitedStarts.Contains(start.X)) then
            visitedStarts.Add(start.X) |> ignore
            bfs (start.X, start.Y, start.Altitude) grid altitudeLoss
            
            let _, finX, _, didFinish = altitudeLoss[start.X]
            if didFinish then
                let nextGlider = { X = finX; Y = 0; Facing = Direction.DOWN; Altitude = initialSegmentAlt }
                startQueue.Enqueue(nextGlider)

    let mutable currX = fst origStart
    let mutable currAlt = 384400
    let mutable segmentCounter = 0

    let mutable canContinue = true
    while currAlt > 0 && canContinue do
        let loss, finX, _, finished = altitudeLoss[currX]
        if not finished || currAlt <= loss then
            canContinue <- false
        else
            currAlt <- currAlt - loss
            currX <- finX
            segmentCounter <- segmentCounter + 1
    
    let finalAltLoss = Dictionary<int, (int * int * int * bool)>()
    finalAltLoss.Add(currX, (10000, 0, 0, false))
    let finalMaxDist = bfs (currX, 0, currAlt) grid finalAltLoss

    (segmentCounter * gridHeight) + finalMaxDist
