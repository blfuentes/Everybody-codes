module quest14_part03

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

//let path = "quest14/test_input_03.txt"
let path = "quest14/quest14_input_03.txt"

type ActionType =
    | U
    | D
    | L
    | R
    | F
    | B

type Operation = {
    Action: ActionType
    Value: int
}

let parseContent(lines: string) =
    let parts = lines.Split(',')
    parts
    |> Array.map (fun p ->
        { Action =
            match p[0] with
            | 'U' -> U
            | 'D' -> D
            | 'L' -> L
            | 'R' -> R
            | 'F' -> F
            | 'B' -> B
            | _ -> failwith "Unknown action"
          Value = int (p[1..]) }
    )

let walkPath (operations: Operation array) =
    let mutable position: int*int*int = (0,0,0) // x,y,z
    let mutable visited = Set.empty<int*int*int>
    operations
    |> Array.iter (fun op ->
        match op with
        | { Action = F; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx, cy, cz + 1)
                visited <- visited.Add(position)
        | { Action = B; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx, cy, cz - 1)
                visited <- visited.Add(position)
        | { Action = D; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx, cy - 1, cz)
                visited <- visited.Add(position)
        | { Action = U; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx, cy + 1, cz)
                visited <- visited.Add(position)
        | { Action = L; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx - 1, cy, cz)
                visited <- visited.Add(position)
        | { Action = R; Value = v } ->
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx + 1, cy, cz)
                visited <- visited.Add(position)
    )
    (visited, position)  

let findDistance (available: Set<int*int*int>) (initPos: int*int*int) (endPos: int*int*int) =
    let (endX, endY, endZ) = endPos

    let neighbors (cell: int*int*int) =
        let deltas = [(-1, 0, 0); (1, 0, 0); (0, -1, 0); (0, 1, 0); (0, 0, 1); (0, 0, -1)]
        let x,y,z  = cell
        deltas
        |> List.map (fun (dx, dy, dz) -> (x + dx, y + dy, z + dz))
        |> List.filter (fun (x, y, z) ->
            available.Contains((x, y, z))
        )

    let frontier = new PriorityQueue<int*int*int, int>()
    let cameFrom = new Dictionary<int*int*int, int*int*int>()
    let costSoFar = new Dictionary<int*int*int, int>()

    let startCell = initPos
    frontier.Enqueue(startCell, 0)
    cameFrom.Add(startCell, startCell)
    costSoFar.Add(startCell, 0)

    let mutable result = -1
    let mutable pathFound = false
    while frontier.Count > 0 && not pathFound do
        let current = frontier.Dequeue()
        if current = endPos then
            result <- costSoFar[current]
            pathFound <- true
        for nextCell in neighbors current do
            let newCost = costSoFar[current] + 1

            if not (costSoFar.ContainsKey(nextCell)) || newCost < costSoFar[nextCell] then
                let nextX, nextY, nextZ = nextCell
                costSoFar[nextCell] <- newCost
                let priority = newCost + abs(nextX - endX) + abs(nextY - endY) + abs(nextZ - endZ)
                frontier.Enqueue(nextCell, priority)
                cameFrom[nextCell] <- current
    result

let findAllShortPaths (available: Set<int*int*int>) (mainTrunk: seq<int*int*int>) (endPositions: seq<int*int*int>) =
    let total = mainTrunk |> Seq.length
    mainTrunk
    |> Seq.mapi (fun idx initPos ->
        printfn "Finding paths from main trunk position %d of %d: %A" idx total initPos
        let minDistance =
            endPositions
            |> Seq.sumBy (fun endPos ->
                findDistance available initPos endPos)
        minDistance
    )
    |> Seq.min

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let operationsCollection = lines |> Array.map parseContent
    let allpoints =         
        operationsCollection
            |> Array.map walkPath
    let finalPositions =
        allpoints
        |> Array.map fst
        |> Array.reduce (fun acc set -> Set.union acc set)
    let leaves =
        allpoints
        |> Array.map snd
        |> Set.ofArray

    let maintrunk =
        finalPositions
        |> Seq.filter(fun (x, _, z) -> x = 0 && z = 0)
    
    findAllShortPaths finalPositions maintrunk leaves