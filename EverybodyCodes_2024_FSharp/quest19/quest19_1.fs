module quest19_1

open EverybodyCodes_2024_FSharp.Modules

//let path = "quest19/test_input_01.txt"
let path = "quest19/quest19_input_01.txt"

type RotationMap = { From: int * int; To: int * int }

let ROT =
    [|
        { From = (-1, -1); To = (0, -1) }
        { From = (0, -1); To = (1, -1) }
        { From = (1, -1); To = (1, 0) }
        { From = (1, 0); To = (1, 1) }
        { From = (1, 1); To = (0, 1) }
        { From = (0, 1); To = (-1, 1) }
        { From = (-1, 1); To = (-1, 0) }
        { From = (-1, 0); To = (-1, -1) }
    |]

let parseContent (lines: string array) =
    let separatorIndex = lines |> Array.findIndex (fun line -> System.String.IsNullOrEmpty(line))
    let rots = lines[0..separatorIndex-1] |> String.concat "" |> fun s -> s.ToCharArray()
    let grid = lines[separatorIndex+1..] |> Array.map (fun line -> line.ToCharArray())
    (rots, grid)

let rotate (grid: char[][]) (center: int * int) (dir: char) =
    let centerX, centerY = center
    let tmp = Array2D.init 3 3 (fun y x -> grid[centerY + y - 1][centerX + x - 1])

    for mapping in ROT do
        let fromCoords, toCoords =
            if dir = 'R' then (mapping.From, mapping.To)
            else (mapping.To, mapping.From) // Reverse for 'L'

        let fromX, fromY = fst fromCoords + 1, snd fromCoords + 1
        let toX, toY = fst toCoords, snd toCoords
        
        grid[centerY + toY][centerX + toX] <- tmp[fromY, fromX]
    grid

let findQuack (grid: char[][]) =
    grid
    |> Array.tryPick (fun row ->
        match (row |> Array.tryFindIndex ((=) '>'), row |> Array.tryFindIndex ((=) '<')) with
        | (Some startIdx, Some endIdx) when startIdx < endIdx ->
            Some(row[startIdx + 1 .. endIdx - 1] |> System.String)
        | _ -> None
    )

let findWord (rots: char[], grid: char[][], repeats: int) =
    let mutable currentGrid = grid
    let mutable rotIndex = 0

    for _ in 1..repeats do
        for y in 1 .. currentGrid.Length - 2 do
            for x in 1 .. currentGrid[0].Length - 2 do
                currentGrid <- rotate currentGrid (x, y) rots[rotIndex % rots.Length]
                rotIndex <- rotIndex + 1
    
    findQuack currentGrid

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let rots, grid = parseContent lines
    findWord (rots, grid, 1)