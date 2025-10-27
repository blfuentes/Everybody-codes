module quest19_part03

open EverybodyCodes_2024_FSharp.Modules

//let path = "quest19/test_input_03.txt"
let path = "quest19/quest19_input_03.txt"


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

// Made the rotate function generic to work with any grid type ('a[][])
let rotate (grid: 'a[][]) (center: int * int) (dir: char) =
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

let xformGrid (grid: 'a[][]) (xform: (int*int)[][]) =
    xform
    |> Array.map (fun row ->
        row
        |> Array.map (fun (origX, origY) -> grid[origY][origX])
    )

let runSinglePass (rots: char[]) (grid: 'a[][]) =
    let mutable currentGrid = grid
    let mutable rotIndex = 0
    for y in 1 .. currentGrid.Length - 2 do
        for x in 1 .. currentGrid[0].Length - 2 do
            currentGrid <- rotate currentGrid (x, y) rots[rotIndex % rots.Length]
            rotIndex <- rotIndex + 1
    currentGrid

let findWord (rots: char[], grid: char[][], repeats: int64) =
    let mutable mult = 1
    while (repeats % (1L <<< mult)) = 0L do
        mult <- mult + 1
    let identityGrid =
        Array.init grid.Length (fun y ->
            Array.init grid[0].Length (fun x -> (x, y))
        )
    
    let mutable xform = runSinglePass rots identityGrid
    for _ in 1 .. mult - 1 do
        xform <- xformGrid xform xform

    let mutable finalGrid = grid
    let numApplications = repeats / (1L <<< (mult - 1))
    for _ in 1L .. numApplications do
        finalGrid <- xformGrid finalGrid xform
    
    findQuack finalGrid

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let rots, grid = parseContent lines
    findWord (rots, grid, 1048576000L)
