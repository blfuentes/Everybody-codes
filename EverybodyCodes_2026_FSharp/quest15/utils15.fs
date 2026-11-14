module quest15_utilities

open System.Collections.Generic

type Point = int * int

type Cave =
    { Start: Point
      Finish: Point
      Walkable: HashSet<Point>
      Deposits: HashSet<Point> }

let parseMap (lines: string array) =
    let walkable = HashSet<Point>()
    let deposits = HashSet<Point>()
    let mutable startPoint = None
    let mutable finishPoint = None

    for row in 0 .. lines.Length - 1 do
        for column in 0 .. lines[row].Length - 1 do
            let position = (row, column)
            match lines[row][column] with
            | '@' ->
                walkable.Add position |> ignore
                deposits.Add position |> ignore
            | '.' -> walkable.Add position |> ignore
            | 'S' ->
                walkable.Add position |> ignore
                startPoint <- Some position
            | 'E' ->
                walkable.Add position |> ignore
                finishPoint <- Some position
            | _ -> ()

    let requireMarker marker position =
        position
        |> Option.defaultWith (fun () -> invalidOp $"missing {marker}")

    { Start = requireMarker "start" startPoint
      Finish = requireMarker "end" finishPoint
      Walkable = walkable
      Deposits = deposits }

let private directions = [| (1, 0); (-1, 0); (0, 1); (0, -1) |]

let availableMoves (previousDirection: Point) =
    directions
    |> Array.filter (fun (rowChange, columnChange) ->
        (rowChange, columnChange) <> (-fst previousDirection, -snd previousDirection))

let buildDepositStates (cave: Cave) (directions: Point array) =
    let states = ResizeArray<Point * Point>()
    let stateIndices = Dictionary<Point * Point, int>()

    // direction to know how the deposit was reached
    for deposit in cave.Deposits do
        for direction in directions do
            let previousPosition =
                (fst deposit - fst direction, snd deposit - snd direction)

            if cave.Walkable.Contains previousPosition then
                let state = (deposit, direction)
                stateIndices.Add(state, states.Count)
                states.Add state

    states, stateIndices
