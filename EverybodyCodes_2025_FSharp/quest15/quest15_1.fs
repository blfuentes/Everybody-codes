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

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let operations = parseContent lines
    let (xStart, yStart, xEnd, yEnd, wall) = buildMap operations
    let (length, _ ) = bfs (xEnd, yEnd) (xStart, yStart) (Set.remove (xStart, yStart) wall)
    length