module quest19_1_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest19/test_input_01.txt"
let path = "quest19/quest19_input_01.txt"

let empty = new HashSet<int*int>()
let walls = new HashSet<int>()

let mutable maxX = 0
let mutable maxY = 0

let reverseX (x: int) =
    maxX - x
let reverseY (y: int) =
    maxY - y

let parseContent (lines: string array) =
    lines
    |> Array.iter(fun line ->
        let parts = line.Split(",")
        let x = parts[0] |> int
        let endingY = (parts[1] |> int) + (parts[2] |> int)
        walls.Add(x) |> ignore
        if endingY > maxY then
            maxY <- endingY + (x - maxX)
        maxX <- x
    )
    lines
    |> Array.iter(fun line ->
        let parts = line.Split(",")
        let x = parts[0] |> int
        let startingY = parts[1] |> int 
        let endingY = (parts[1] |> int) + (parts[2] |> int)
        for y in startingY .. (endingY-1) do
            let (eY, eX) = (reverseY y, x)
            if not (empty.Contains((eY, eX))) then
                empty.Add(eY, eX) |> ignore
    )

let printGrid (path: (int*int) list)=
    for y in 0 .. maxY do
        for x in 0 .. maxX do
            if List.contains (x, y) path then
                printf "O"
            else
                if walls.Contains(x) then
                    printf "%c" (if empty.Contains((y, x)) then '.' else '#')
                else
                    printf "."
        printfn ""

let aStar (start: int*int) =
    let isFree (x: int, y: int) =
        empty.Contains((y, x)) || (not (walls.Contains(x)))

    let getNeighbors (x, y) =
        [ 
            (x+1, y-1); // moving up
            (x+1, y+1) // letting it fall
        ]
        |> List.filter (fun (x, y) ->
            x >= 0 && x <= maxX && y >= 0 && y <= maxY && isFree (x, y)
        )
    
    // gScore with number of up presses
    let gScore = Dictionary<int*int, int>()
    let fScore = Dictionary<int*int, int>()
    let parent = Dictionary<int*int, int*int>()
    let closedSet = HashSet<int*int>()
    let openSet = SortedSet<(int * (int*int))>(Comparer<(int * (int*int))>.Create(fun (f1, c1) (f2, c2) ->
        if f1 <> f2 then compare f1 f2 else compare c1 c2
    ))

    gScore[start] <- 0
    fScore[start] <- 0
    openSet.Add((fScore[start], start)) |> ignore

    let mutable found = false
    let mutable goal = (maxX, 0)
    
    while openSet.Count > 0 && not found do
        let (_, current) = openSet.Min
        openSet.Remove((fScore[current], current)) |> ignore
        
        if closedSet.Contains(current) then
            () // already processed - skip duplicate
        elif (fst current) = maxX then
            found <- true // early termination
            goal <- current
        else
            closedSet.Add(current) |> ignore
            for n in getNeighbors current do
                if not (closedSet.Contains(n)) then
                    // +1 if moving up, +0 if letting it fall
                    let moveCost = if (snd n) < (snd current) then 1 else 0
                    let tentativeG = gScore[current] + moveCost
                    
                    if not (gScore.ContainsKey(n)) || tentativeG < gScore[n] then
                        if fScore.ContainsKey(n) then
                            openSet.Remove((fScore[n], n)) |> ignore
                        parent[n] <- current
                        gScore[n] <- tentativeG
                        // Heuristic: estimate remaining up presses needed
                        // Simple heuristic: distance to goal
                        let h = abs(maxX - (fst n))
                        fScore[n] <- tentativeG + h
                        openSet.Add((fScore[n], n)) |> ignore

    let rec buildPath acc pos =
        if pos = start then start :: acc
        elif parent.ContainsKey(pos) then buildPath (pos :: acc) parent[pos]
        else []

    let path = if found then buildPath [] goal else []
    path.Length, path

let calculatePathCost (path: (int*int) list) =
    path
    |> List.pairwise
    |> List.sumBy(fun ((x1, y1), (x2, y2)) ->
        if y2 < y1 then 1 else 0
    )

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent lines
    let pathLength, path = aStar (0, maxY)
    //printGrid path
    let cost = calculatePathCost path
    cost