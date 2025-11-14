module quest20_1

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

//let path = "quest20/test_input_01.txt"
let path = "quest20/quest20_input_01.txt"

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
            | _ -> glider.Altitude - 1 // Empty or Start
        
        { glider with X = newX; Y = newY; Altitude = newAltitude }

let parseContent (lines: string array) =
    let mutable start = (0, 0)
    let grid =
        lines
        |> Array.mapi (fun y line ->
            line.ToCharArray()
            |> Array.mapi (fun x char ->
                match char with
                | '.' -> CellType.Empty
                | '#' -> CellType.Wall
                | 'S' -> 
                    start <- (x, y)
                    CellType.Empty
                | '-' -> CellType.Down
                | _ -> CellType.Up // Assuming any other char is Up
            )
        )
    (grid, start)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let grid, start = parseContent lines

    let positionStack = Stack<(Glider * int)>()
    let initialGlider = { X = fst start; Y = snd start; Facing = Direction.DOWN; Altitude = 1000 }
    positionStack.Push(initialGlider, 0)

    let visited = HashSet<(int * int * Direction * int * int)>()
    let dp = Dictionary<int, (int * Glider)>()

    while positionStack.Count > 0 do
        let glider, step = positionStack.Pop()

        if glider.Altitude <= 0 then
            match dp.TryGetValue(step) with
            | true, (currentBest, _) ->
                if currentBest < 0 then
                    dp[step] <- (0, glider)
            | false, _ ->
                dp.Add(step, (0, glider))
        elif step = 100 then
            match dp.TryGetValue(step) with
            | true, (currentBest, _) ->
                if currentBest < glider.Altitude then
                    dp[step] <- (glider.Altitude, glider)
            | false, _ ->
                dp.Add(step, (glider.Altitude, glider))
        else
            match dp.TryGetValue(step) with
            | true, (currentBest, _) ->
                if currentBest < glider.Altitude then
                    dp[step] <- (glider.Altitude, glider)
            | false, _ -> ()

            let state = (glider.X, glider.Y, glider.Facing, glider.Altitude, step)
            if not (visited.Contains(state)) then
                visited.Add(state) |> ignore
                if Glider.canMove glider grid then
                    let gmove = Glider.move glider grid
                    positionStack.Push(gmove, step + 1)
                
                let gleft = Glider.turnLeft glider
                if Glider.canMove gleft grid then
                    let gleftMove = Glider.move gleft grid
                    positionStack.Push(gleftMove, step + 1)

                let gright = Glider.turnRight glider
                if Glider.canMove gright grid then
                    let grightMove = Glider.move gright grid
                    positionStack.Push(grightMove, step + 1)

    let bestAt100, _ = dp[100]
    bestAt100
