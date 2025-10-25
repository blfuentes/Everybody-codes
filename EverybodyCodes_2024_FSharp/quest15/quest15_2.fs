module quest15_part02

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

type CellType =
    | Empty
    | Herb
    | Start
    | Lake
    | Wall

type Cell = {
    CellType: CellType
    Name: string
    Row: int
    Col: int
}

//let path = "quest15/test_input_02.txt"
let path = "quest15/quest15_input_02.txt"

let lengthsMemo = new Dictionary<(Cell*Cell), int>()

let parseContent(lines: string []) =
    let maxRows, maxCols = lines.Length, lines[0].Length
    let themap = Array2D.create maxRows maxCols { CellType = Empty; Name = "."; Row = -1; Col = -1 }
    let mutable startPos = { CellType = Start; Name = "S"; Row = -1; Col = -1 }
    let herbs =
        seq {
            for rIdx, line in lines |> Array.indexed do
                for cIdx, ch in line |> Seq.indexed do
                    let cellType =
                        match ch with
                        | '.' when rIdx = 0 ->
                            startPos <- { CellType = Start; Name="S"; Row = rIdx; Col = cIdx }
                            Start
                        | '.' -> Empty
                        | '#' -> Wall
                        | '~' -> Lake
                        | _  -> Herb
                    let cell = { CellType = cellType; Name = string(ch); Row = rIdx; Col = cIdx } 
                    if cellType.IsHerb then
                        yield cell
                    themap[rIdx, cIdx] <- cell
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
            (themap[x, y].CellType <> Wall && themap[x, y].CellType <> Lake))
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

    if lengthsMemo.ContainsKey((initPos, endPos)) then
        result <- lengthsMemo[(initPos,endPos)]
        result
    elif lengthsMemo.ContainsKey((endPos, initPos)) then
        result <- lengthsMemo[(endPos,initPos)]
        result
    else
        while frontier.Count > 0 && not pathFound do
            let current = frontier.Dequeue()
            if current.Row = endPos.Row && current.Col = endPos.Col then
                result <- costSoFar[current]
                pathFound <- true
                if not (lengthsMemo.ContainsKey((initPos, endPos))) then
                    lengthsMemo.Add((initPos, endPos), result)
                if not (lengthsMemo.ContainsKey((endPos, initPos))) then
                    lengthsMemo.Add((endPos, initPos), result)
            else
                for nextCell in neighbors current do
                    let newCost = costSoFar[current] + 1

                    if not (costSoFar.ContainsKey(nextCell)) || newCost < costSoFar[nextCell] then
                        costSoFar[nextCell] <- newCost
                        let priority = newCost + abs(nextCell.Row - endX) + abs(nextCell.Col - endY)
                        frontier.Enqueue(nextCell, priority)
                        cameFrom[nextCell] <- current
        result

//let findAllShortPaths (themap: Cell[,]) (initPos: Cell) (herbs: Set<Cell>) =
//    let total = herbs.Count

//    herbs
//    |> Seq.mapi (fun idx endPos ->
//        printfn "Finding paths from main trunk position %d of %d: %A" idx total endPos
//        findDistance themap initPos endPos)
//    |> Seq.min

//let allPermutations (herbsByName: IDictionary<string, Cell list>) =
//    // Convert dictionary values to array of lists for easier processing
//    let allLists = herbsByName.Values |> Array.ofSeq
    
//    let rec cartesianProduct lists =
//        match lists with
//        | [] -> [[]]  // Base case: empty list produces a single empty combination
//        | currentList::remainingLists ->
//            [ for element in currentList do
//                for combination in cartesianProduct remainingLists do
//                    yield element::combination ]
    
//    cartesianProduct (allLists |> List.ofArray)

let rec combinations list =
    match list with
    | [] -> [ [] ]
    | x::xs ->
        let rest = combinations xs
        rest @ (rest |> List.map (fun subset -> x :: subset))

//let allPermutations (herbsByName: IDictionary<string, Cell list>) =
//    let rec cartesianProduct lists =
//        match lists with
//        | [] -> [[]]
//        | currentList::remainingLists ->
//            [ for element in currentList do
//                for combination in cartesianProduct remainingLists do
//                    yield element::combination ]

//    let rec permutations = function
//        | [] -> [[]]
//        | list -> 
//            list 
//            |> List.collect (fun x -> 
//                permutations (List.filter ((<>) x) list) 
//                |> List.map (fun xs -> x::xs))
    
//    herbsByName.Values 
//    |> Array.ofSeq
//    |> List.ofArray
//    |> cartesianProduct
//    |> List.collect permutations

//let findAllPaths(themap: Cell[,]) (herbsByName: IDictionary<string, Cell list>) (startPos: Cell) =
//    let allPossibleWalks = allPermutations herbsByName
//    let fullPath = allPossibleWalks |> List.map(fun l -> [startPos] @ l @ [startPos])
//    fullPath
//    |> List.mapi(fun idx p -> 
//        printfn "Finding paths: %d of %d" idx fullPath.Length
//        p
//        |> List.pairwise
//        |> List.map(fun (a, b) -> findDistance themap a b)
//        |> List.sum
//    )
//    |> List.min

//let findAllPaths2(themap: Cell[,]) (herbsByName: IDictionary<string, Cell list>) (startPos: Cell) =
//    let allPossibleWalks = allPermutations herbsByName
//    let fullPaths = allPossibleWalks |> List.map(fun l -> [startPos] @ l @ [startPos])
//    let mutable minDistance = System.Int32.MaxValue
//    for idx, path in Array.indexed (fullPaths |> Array.ofList) do
//        if idx % 500 = 0 then
//            printfn "Finding paths: %d of %d" idx fullPaths.Length
//        let mutable pathDistance = 0
//        let segments = path |> List.pairwise
//        let mutable doContinue = true
//        let mutable sIdx = 0
//        while doContinue do
//            if segments.Length = sIdx then
//                doContinue <- false
//            else
//                let (a, b) = segments.Item(sIdx)
//                let segmentDistance = findDistance themap a b
//                pathDistance <- pathDistance + segmentDistance
//                if minDistance < pathDistance then
//                    doContinue <- false
//            sIdx <- sIdx + 1
//        if pathDistance < minDistance then
//            minDistance <- pathDistance
//    minDistance

let findAllPaths3(themap: Cell[,]) (herbsByName: IDictionary<string, Cell list>) (startPos: Cell) =
    let maxRows, maxCols = themap.GetLength(0), themap.GetLength(1)
    let distances = new Dictionary<string*string, int list>()
    for rIdx in [0..maxRows-1] do
        for cIdx in [0..maxCols-1] do
            let origin = themap[rIdx, cIdx]
            if origin.CellType = Herb || origin.CellType = Start then
                for trIdx in [0..maxRows-1] do
                    for tcIdx in [0..maxCols-1] do
                        if rIdx <> trIdx || cIdx <> tcIdx then
                            let target = themap[trIdx, tcIdx]
                            if (target.CellType = Herb || target.CellType = Start) && origin.Name <> target.Name then
                                let distance = findDistance themap origin target
                                if not (distances.ContainsKey((origin.Name, target.Name))) then
                                    distances.Add((origin.Name, target.Name), [distance])
                                elif not (distances.ContainsKey((target.Name, origin.Name))) then
                                    distances.Add((target.Name, origin.Name), [distance])
                                else
                                    distances[(origin.Name, target.Name)] <- distances[(origin.Name, target.Name)] @ [distance]
                                    distances[(target.Name, origin.Name)] <- distances[(target.Name, origin.Name)] @ [distance]
        
    let allPossibleWalks = combinations (herbsByName.Keys |> List.ofSeq)
    let fullPaths = allPossibleWalks |> List.map(fun l -> ["S"] @ l @ ["S"])
    let mutable minDistance = System.Int32.MaxValue
    for idx, path in Array.indexed (fullPaths |> Array.ofList) do
        if idx % 500 = 0 then
            printfn "Finding paths: %d of %d" idx fullPaths.Length
        let mutable pathDistance = 0
        let segments = path |> List.pairwise
        let mutable doContinue = true
        let mutable sIdx = 0
        while doContinue do
            if segments.Length = sIdx then
                doContinue <- false
            else
                let (a, b) = segments.Item(sIdx)
                let segmentDistance = 0 //findDistance themap a b
                pathDistance <- pathDistance + segmentDistance
                if minDistance < pathDistance then
                    doContinue <- false
            sIdx <- sIdx + 1
        if pathDistance < minDistance then
            minDistance <- pathDistance
    minDistance

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (themap, herbs, startPos) = parseContent lines
    
    let herbsByName =
        herbs
        |> Set.filter(fun h -> h.CellType.IsHerb)
        |> List.ofSeq
        |> List.groupBy(fun h -> h.Name)
        |> dict

    findAllPaths3 themap herbsByName startPos
    //findAllPaths themap herbsByName startPos

