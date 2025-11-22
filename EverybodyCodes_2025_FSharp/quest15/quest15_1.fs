module quest15_1

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest15/test_input_01.txt"
let path = "quest15/quest15_input_01.txt"

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
    // folder: (x,y,dx,dy,wall) -> op -> new state
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

// using "manhattan BFS" avoiding walls
let distanceToStart (xStart:int) (yStart:int) (xEnd:int) (yEnd:int) (wall: Set<(int*int)>) : int =
    let dist = Dictionary<(int*int), int>()
    let queue = Queue<(int*int)>()
    dist.Add((xStart, yStart), 0)
    queue.Enqueue((xStart, yStart))

    let rec loop () =
        if queue.Count = 0 then failwith "No path found"
        else
            let (x, y) = queue.Dequeue()
            let d = dist[(x, y)]
            let neighbors = [| (x - 1, y); (x + 1, y); (x, y - 1); (x, y + 1) |]
            let mutable found: int option = None
            for (xn, yn) in neighbors do
                if found.IsNone then
                    if xn = xEnd && yn = yEnd then
                        found <- Some (d + 1)
                    elif not (wall.Contains (xn, yn)) then
                        let mutable existing = 0
                        if dist.TryGetValue((xn, yn), &existing) then
                            if d + 1 < existing then
                                dist[(xn, yn)] <- d + 1
                                queue.Enqueue((xn, yn))
                        else
                            dist[(xn, yn)] <- d + 1
                            queue.Enqueue((xn, yn))
            match found with
            | Some value -> value
            | None -> loop ()
    loop ()

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let operations = parseContent lines
    let (xStart, yStart, xEnd, yEnd, wall) = buildMap operations
    distanceToStart xStart yStart xEnd yEnd wall
