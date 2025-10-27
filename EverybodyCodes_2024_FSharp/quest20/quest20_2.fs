module quest20_part02

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic     

//let path = "quest20/test_input_02.txt"
let path = "quest20/quest20_input_02.txt"

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
    | CheckpointA
    | CheckpointB
    | CheckpointC
    | Start

type Glider = {
    X: int
    Y: int
    Facing: Direction
    Altitude: int
    Checkpoints: int
}

module Glider =
    let turnLeft (glider: Glider) =
        let newFacingValue = (int glider.Facing - 1 + 4) % 4
        { glider with Facing = enum<Direction>(newFacingValue) }

    let turnRight (glider: Glider) =
        let newFacingValue = (int glider.Facing + 1) % 4
        { glider with Facing = enum<Direction>(newFacingValue) }

    let canMove (glider: Glider) (grid: CellType[][]) =
        let nextPos =
            match glider.Facing with
            | Direction.UP -> Some(glider.X, glider.Y - 1)
            | Direction.RIGHT -> Some(glider.X + 1, glider.Y)
            | Direction.DOWN -> Some(glider.X, glider.Y + 1)
            | Direction.LEFT -> Some(glider.X - 1, glider.Y)

        match nextPos with
        | None -> false
        | Some (nx, ny) ->
            if ny < 0 || ny >= grid.Length || nx < 0 || nx >= grid[0].Length then
                false
            else
                let cellType = grid[ny][nx]
                if cellType = CellType.Wall then false
                elif cellType = CellType.Start then
                    // Can only enter Start if all checkpoints are collected and altitude is enough
                    glider.Checkpoints = 3 && glider.Altitude >= 10000
                else
                    true

    let move (glider: Glider) (grid: CellType[][]) =
        let newPos =
            match glider.Facing with
            | Direction.UP -> (glider.X, glider.Y - 1)
            | Direction.RIGHT -> (glider.X + 1, glider.Y)
            | Direction.DOWN -> (glider.X, glider.Y + 1)
            | Direction.LEFT -> (glider.X - 1, glider.Y)
        
        let newX, newY = newPos
        let cell = grid[newY][newX]
        
        let newAltitude, newCheckpoints =
            match cell with
            | CellType.Up -> (glider.Altitude + 1, glider.Checkpoints)
            | CellType.Down -> (glider.Altitude - 2, glider.Checkpoints)
            | CellType.CheckpointA when glider.Checkpoints = 0 -> (glider.Altitude - 1, 1)
            | CellType.CheckpointB when glider.Checkpoints = 1 -> (glider.Altitude - 1, 2)
            | CellType.CheckpointC when glider.Checkpoints = 2 -> (glider.Altitude - 1, 3)
            | _ -> (glider.Altitude - 1, glider.Checkpoints)
        
        { glider with X = newX; Y = newY; Altitude = newAltitude; Checkpoints = newCheckpoints }

    let isFinished (glider: Glider) (grid: CellType[][]) =
        grid[glider.Y][glider.X] = CellType.Start && glider.Checkpoints = 3

let parseContent (lines: string array) =
    let mutable startPos = (0, 0)
    let grid =
        lines
        |> Array.mapi (fun y line ->
            line.ToCharArray()
            |> Array.mapi (fun x char ->
                match char with
                | '.' -> CellType.Empty
                | '#' -> CellType.Wall
                | 'S' -> startPos <- (x, y); CellType.Start
                | '-' -> CellType.Down
                | '+' -> CellType.Up
                | 'A' -> CellType.CheckpointA
                | 'B' -> CellType.CheckpointB
                | 'C' -> CellType.CheckpointC
                | _ -> failwithf "Unknown char %c" char
            )
        )
    (grid, startPos)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let grid, start = parseContent lines

    let positionQueue = Queue<list<Glider * int>>()
    let initialGlider = { X = fst start; Y = snd start; Facing = Direction.DOWN; Altitude = 10000; Checkpoints = 0 }
    positionQueue.Enqueue([(initialGlider, 0)])

    let visited = Array4D.create grid.Length grid[0].Length 4 4 -1
    let mutable finishedGliders = []

    while positionQueue.Count > 0 && List.isEmpty finishedGliders do
        let positions = positionQueue.Dequeue()
        let mutable nextPositions = []

        for glider, step in positions do
            let visitedAltitude = visited[glider.Y, glider.X, glider.Checkpoints, int glider.Facing]
            if visitedAltitude < glider.Altitude then
                visited[glider.Y, glider.X, glider.Checkpoints, int glider.Facing] <- glider.Altitude

                if glider.Altitude > 0 then
                    let moves =
                        [
                            glider // Forward
                            Glider.turnLeft glider // Turn Left
                            Glider.turnRight glider // Turn Right
                        ]
                        |> List.filter (fun g -> Glider.canMove g grid)
                        |> List.map (fun g -> Glider.move g grid)

                    for nextGlider in moves do
                        if Glider.isFinished nextGlider grid then
                            finishedGliders <- (nextGlider, step + 1) :: finishedGliders
                        else
                            nextPositions <- (nextGlider, step + 1) :: nextPositions
        
        if not (List.isEmpty nextPositions) then
            positionQueue.Enqueue(nextPositions)

    if List.isEmpty finishedGliders then
        -1 // No solution found
    else
        finishedGliders |> List.minBy snd |> snd
