module quest20_3

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest20/test_input_03.txt"
let path = "quest20/quest20_input_03.txt"

type Facing =
    | Down
    | Up
type CellType =
    | Wall
    | None
    | Trampoling

type Trampoling = {
    Id: string
    Position: int * int
    Orientation: Facing
    Kind: CellType
}

let parseContent(lines: string array) =
    let grid = Array2D.init (lines.Length) (lines[0].Length) (fun r c -> lines[r][c])
    grid

let rotate (grid: char[,]) =
    let nRows = grid.GetLength(0)
    let nCols = grid.GetLength(1)
    let gridNew = Array2D.create<char> nRows nCols '.'
    
    for c in 0 .. 2 .. nCols - 1 do
        let mutable rn = c / 2
        let mutable cn = nCols - 1 - c / 2
        
        for r in 0 .. nRows - 1 do
            // first diagonal
            if r + c < nCols && grid[r, r + c] <> '.' then
                gridNew[rn, cn] <- grid[r, r + c]
                cn <- cn - 1
            
            // second diagonal
            if r + c + 1 < nCols && grid[r, r + c + 1] <> '.' then
                gridNew[rn, cn] <- grid[r, r + c + 1]
                cn <- cn - 1
    
    gridNew

let printTrampoling (trampoling: Trampoling[,]) (startPos: int*int) (endPos: int*int) =
    let maxRow = trampoling.GetLength(0) - 1
    let maxCol = trampoling.GetLength(1) - 1
    for r in 0 .. maxRow do
        for c in 0 .. maxCol do
            match trampoling[r, c].Kind with
            | Wall -> printf "#"
            | None -> printf "."
            | Trampoling -> 
                if (r, c) = startPos then
                    printf "S"
                elif (r, c) = endPos then
                    printf "E"
                else
                    match trampoling[r, c].Orientation with
                    | Down -> 
                        // print triangle pointing down
                        printf "D"
                    | Up -> 
                        // print triangle pointing up
                        printf "U"
        printfn ""

let startPosition (grid: char[,]) =
    let rows = grid.GetLength(0)
    let cols = grid.GetLength(1)
    seq {
        for row in 0 .. rows - 1 do
            for col in 0 .. cols - 1 do
                if grid[row, col] = 'S' then yield (row, col)
    } |> Seq.head

let findNeighbours (row: int) (col: int) (s: int) (nRows: int) (nCols: int) =
    let possible =
        if (row + col) &&& 1 = 1 then
            // Odd sum: r,c same, left, right, down
            [(row, col); (row, col - 1); (row, col + 1); (row + 1, col)]
        else
            // Even sum: r,c same, left, right, up
            [(row, col); (row, col - 1); (row, col + 1); (row - 1, col)]
    
    possible
    |> List.filter (fun (rn, cn) ->
        rn >= 0 && rn < nRows && rn <= cn && cn <= nCols - 1 - rn
    )
    |> List.map (fun (rn, cn) -> (rn, cn, (s + 1) % 3))

// BFS with 3 rotating grids
let findShortestPath (grids: char[,] array) =
    let (startRow, startCol) = startPosition grids[0]
    let start = (startRow, startCol, 0)
    let nRows = grids[0].GetLength(0)
    let nCols = grids[0].GetLength(1)
    
    let distances = Dictionary<int * int * int, int>()
    let mutable current = [start]
    
    distances[start] <- 0
    
    let mutable result = System.Int32.MaxValue
    let mutable found = false
    
    while current.Length > 0 && not found do
        let newTodo = ResizeArray<int * int * int>()
        
        for (r, c, s) in current do
            if grids[s][r, c] = 'E' then
                result <- distances[(r, c, s)]
                found <- true
            else
                for (rn, cn, sn) in findNeighbours r c s nRows nCols do
                    let ch = grids[sn][rn, cn]
                    if (ch = 'T' || ch = 'S' || ch = 'E') && not (distances.ContainsKey((rn, cn, sn))) then
                        distances[(rn, cn, sn)] <- distances[(r, c, s)] + 1
                        newTodo.Add((rn, cn, sn))
        
        current <- List.ofSeq newTodo
    
    result  
  
let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let initialGrid = parseContent lines
    let firstRotation = rotate initialGrid
    let secondRotation = rotate firstRotation
    let grids = [| initialGrid; firstRotation; secondRotation |]
    findShortestPath grids
    
