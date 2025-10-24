module quest13_part01

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

//let path = "quest13/test_input_01.txt"
let path = "quest13/quest13_input_01.txt"

type CellType =
    | EMPTY
    | WALL
    | ROOM of Level: int

type RoomCell = {
    Row: int
    Col: int
    CellType: CellType
}

let parseContent(lines: string array) =
    let chamber = Array2D.create lines.Length lines.[0].Length { Row = 0; Col = 0; CellType = EMPTY }
    let mutable startPos = None
    let mutable endPos = None
    for rowIdx,line in Array.indexed lines do
        for colIdx, ch in Array.indexed (line.ToCharArray()) do
            let cellType =
                match ch with
                | '#' -> WALL
                | ' ' -> EMPTY                
                | _ when ch = 'S' -> 
                    startPos <- Some (rowIdx, colIdx)
                    ROOM 0
                | _ when ch = 'E' -> 
                    endPos <- Some (rowIdx, colIdx)
                    ROOM 0
                | _ -> ROOM (int (string ch))
            let cell = { Row = rowIdx; Col = colIdx; CellType = cellType }
            chamber[rowIdx, colIdx] <- cell

    (chamber, startPos, endPos)

let pathFind (chamber: RoomCell[,]) (startPos: int * int) (endPos: int * int) =
    let neighbors (cell: RoomCell) =
        let deltas = [(-1, 0); (1, 0); (0, -1); (0, 1)]
        deltas
        |> List.map (fun (dr, dc) -> (cell.Row + dr, cell.Col + dc))
        |> List.filter (fun (r, c) ->
            r >= 0 && r < chamber.GetLength(0) &&
            c >= 0 && c < chamber.GetLength(1) &&
            match chamber[r, c].CellType with
            | ROOM _ -> true
            | _ -> false
        )
        |> List.map (fun (r, c) -> chamber[r, c])
    let frontier = new PriorityQueue<RoomCell, int>()
    let cameFrom = new Dictionary<RoomCell, RoomCell>()
    let costSoFar = new Dictionary<RoomCell, int>()

    let startCell = chamber[startPos |> fst, startPos |> snd]
    frontier.Enqueue(startCell, 0)
    cameFrom.Add(startCell, startCell)
    costSoFar.Add(startCell, 0)

    let mutable result = -1
    let mutable pathFound = false
    while frontier.Count > 0 && not pathFound do
        let current = frontier.Dequeue()
        if current.Row = (endPos |> fst) && current.Col = (endPos |> snd) then
            result <- costSoFar[current]
            pathFound <- true
        for nextCell in neighbors current do
            let movementCost = 1
            let levelDifference = 
                match (current.CellType, nextCell.CellType) with
                | (ROOM fromdata, ROOM todata) -> abs(fromdata - todata)
                | _ -> 0

            let newCost = costSoFar[current] + movementCost + levelDifference

            if not (costSoFar.ContainsKey(nextCell)) || newCost < costSoFar[nextCell] then
                costSoFar[nextCell] <- newCost
                let priority = newCost + abs(nextCell.Row - (endPos |> fst)) + abs(nextCell.Col - (endPos |> snd))
                frontier.Enqueue(nextCell, priority)
                cameFrom[nextCell] <- current
    result

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (chamber, startPos, endPos) = parseContent(lines)
    pathFind chamber (startPos.Value) (endPos.Value)