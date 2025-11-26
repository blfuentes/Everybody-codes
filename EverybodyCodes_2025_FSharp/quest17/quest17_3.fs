module quest17_3

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest17/test_input_03.txt"
let path = "quest17/quest17_input_03.txt"

let mutable Xv, Yv = (0, 0)
let mutable maxX, maxY = (0, 0)
let mutable Xe, Ye = (0, 0)

let parseContent (lines: string array) =
    maxX <- lines.Length  // X = rows (height)
    maxY <- lines[0].Length  // Y = columns (width)
    let field = Array2D.create maxX maxY 0
    for x in 0 .. lines.Length - 1 do
        for y in 0 .. lines[x].Length - 1 do
            match lines[x][y] with
            | 'S' ->
                Xe <- x
                Ye <- y
                field[x, y] <- 0
            | '@' ->
                Xv <- x
                Yv <- y
                field[x, y] <- 0
            | _ ->
                field[x, y] <- int((lines[x][y]).ToString())
    field

let isInRange (Xv, Yv) (Xc, Yc) R =
    (Xv - Xc) * (Xv - Xc) + (Yv - Yc) * (Yv - Yc) <= R * R

let printField (field: int[,]) =
    for x in 0 .. maxX - 1 do
        for y in 0 .. maxY - 1 do
            if (x, y) = (Xv, Yv) then
                printf "@"
            else
                printf "%s" (field[x, y].ToString())
        printfn ""

type Coord3D = int * int * int

let dijkstra (grid: Dictionary<int*int, int>) (start: int*int) (centre: int*int) (limit: int) =
    if grid.Count = 0 then
        System.Int32.MaxValue
    else
        let getNeighbors (current: Coord3D) =
            let (cellX, cellY, cellZ) = current
            [(1, 0); (-1, 0); (0, 1); (0, -1)]
            |> List.map (fun (dirX, dirY) ->
                let adjX = cellX + dirX
                let adjY = cellY + dirY
                let adjZ =
                    if adjY < snd centre then
                        if cellX < fst centre && adjX >= fst centre then 1
                        elif adjX < fst centre && cellX >= fst centre then 0
                        else cellZ
                    else cellZ
                (adjX, adjY, adjZ))
            |> List.filter (fun (x, y, _) -> grid.ContainsKey((x, y)))
        
        let startNode = (fst start, snd start, 0)
        let goalNode = (fst start, snd start, 1)
        
        let dist = Dictionary<Coord3D, int>()
        let visited = HashSet<Coord3D>()
        
        let pq = SortedSet<int * Coord3D>(Comparer<int * Coord3D>.Create(fun (d1, c1) (d2, c2) ->
            if d1 <> d2 then compare d1 d2 else compare c1 c2
        ))
        
        dist[startNode] <- 0
        pq.Add((0, startNode)) |> ignore
        
        let mutable result = System.Int32.MaxValue
        
        while pq.Count > 0 do
            let (d, current) = pq.Min
            pq.Remove((d, current)) |> ignore
            
            if current = goalNode then
                result <- d
                pq.Clear()
            elif d > limit then
                () // over the limit...
            elif visited.Contains(current) then
                () // already processed ...
            else
                visited.Add(current) |> ignore
                
                for neighbor in getNeighbors current do
                    let (adjX, adjY, _) = neighbor
                    let alt = d + grid[(adjX, adjY)]
                    if alt <= limit && (not (dist.ContainsKey(neighbor)) || alt < dist[neighbor]) then
                        dist[neighbor] <- alt
                        pq.Add((alt, neighbor)) |> ignore
        
        result

let shortestPath (field: int[,]) =
    let volcano = (Xv, Yv)
    let start = (Xe, Ye)
    
    let rec searchRadius R =
        // printfn "trying radius R=%d, limit=%d" R (30 * (R + 1))
        // build volcano excluding radius
        let volcanoGrid = Dictionary<int*int, int>()
        for x in 0 .. maxX - 1 do
            for y in 0 .. maxY - 1 do
                let distSq = (x - Xv) * (x - Xv) + (y - Yv) * (y - Yv)
                if distSq > R * R then
                    volcanoGrid[(x, y)] <- field[x, y]
        
        volcanoGrid[start] <- 0
        
        // shortest path 30 * (R + 1)
        let limit = 30 * (R + 1)
        let minFound = dijkstra volcanoGrid start volcano limit
        
        //printfn "found distance: %d" minFound
        
        if minFound < limit then
            let result = minFound * R
            //printfn "result = %d * %d = %d" minFound R result
            result
        else
            searchRadius (R + 1)
    
    searchRadius 0

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let field = parseContent lines
    //printfn "Field size: %dx%d, Volcano at (%d,%d), Start at (%d,%d)" maxX maxY Xv Yv Xe Ye
    shortestPath field
