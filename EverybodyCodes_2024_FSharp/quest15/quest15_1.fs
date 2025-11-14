module quest15_1

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

type CellType =
    | Empty
    | Herb
    | Start
    | Wall

type Cell = {
    CellType: CellType
    Row: int
    Col: int
}

//let path = "quest15/test_input_01.txt"
let path = "quest15/quest15_input_01.txt"

let parseContent(lines: string []) =
    let maxRows, maxCols = lines.Length, lines[0].Length
    let themap = Array2D.create maxRows maxCols { CellType = Wall; Row = -1; Col = -1 }
    let mutable startPos = { CellType = Start; Row = -1; Col = -1 }
    let herbs =
        seq {
            for rIdx, line in lines |> Array.indexed do
                for cIdx, ch in line |> Seq.indexed do
                    let cellType =
                        match ch with
                        | '.' when rIdx = 0 ->
                            startPos <- { CellType = Start; Row = rIdx; Col = cIdx }
                            Start
                        | '.' -> Empty
                        | 'H' -> Herb
                        | '#' -> Wall
                        | _ -> failwith "Unknown cell type"
                    let cell = { CellType = cellType; Row = rIdx; Col = cIdx } 
                    if cellType.IsHerb then
                        yield cell
                    themap[rIdx, cIdx] <- { CellType = cellType; Row = rIdx; Col = cIdx } 
        } |> Set.ofSeq

    (themap, herbs, startPos)

let findDistance (themap: Cell[,]) (initPos: Cell) (endPos: Cell) =
    let (endX, endY) = (endPos.Row, endPos.Col)
    let maxRows, maxCols = themap.GetLength(0), themap.GetLength(1)
    let neighbors (cell: Cell) =
        let deltas = [(-1, 0); (1, 0); (0, -1); (0, 1)]
        deltas
        |> List.map (fun (dx, dy) -> (cell.Row + dx, cell.Col + dy))
        |> List.filter (fun (x, y) ->
            x >= 0 && x < maxRows && y >= 0 && y < maxCols &&
            themap[x, y].CellType <> Wall)
        |> List.map (fun (x, y) -> themap[x, y])

    let frontier = new PriorityQueue<Cell, int>()
    let cameFrom = new Dictionary<Cell, Cell>()
    let costSoFar = new Dictionary<Cell, int>()

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
                costSoFar[nextCell] <- newCost
                let priority = newCost + abs(nextCell.Row - endX) + abs(nextCell.Col - endY)
                frontier.Enqueue(nextCell, priority)
                cameFrom[nextCell] <- current
    result

let findAllShortPaths (themap: Cell[,]) (initPos: Cell) (herbs: Set<Cell>) =
    let total = herbs.Count

    herbs
    |> Seq.mapi (fun idx endPos ->
        findDistance themap initPos endPos)
    |> Seq.min

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (themap, herbs, startPos) = parseContent lines
    
    (findAllShortPaths themap startPos herbs) * 2
