module quest05_3

open EverybodyCodes_2026_FSharp.Modules
open quest05_utilities
open System.Collections.Generic

// let path = "quest05/test_input_03.txt"
let path = "quest05/quest05_input_03.txt"

let safePos (map: char array array) (nr, nc) =
    nr >= 0 && nr < map.Length && nc >= 0 && nc < map[0].Length

let neighbours (map: char array array) ((row, col): (int*int)) = seq {
    for r in [-1..1] do
        for c in [-1..1] do
            if r <> 0 || c <> 0 then
                if safePos map (row + r, col + c)  then
                    yield (row + r, col + c)
}

let reconstructSand (tMap: HashSet<int*int>) (map: char array array) =
    let sand = HashSet<int*int>()

    for (row, col) in tMap do
        let isMapEdge =
            row = 0 || row = map.Length - 1 || col = 0 || col = map[0].Length - 1
        let touchesWater =
            neighbours map (row, col)
            |> Seq.exists (fun pos -> not (tMap.Contains pos))

        if isMapEdge || touchesWater then
            sand.Add(row, col) |> ignore

    let mutable addedSand = true
    while addedSand do
        let newSand =
            tMap
            |> Seq.filter (fun pos ->
                not (sand.Contains pos)
                && (neighbours map pos
                    |> Seq.filter sand.Contains
                    |> Seq.length >= 5)
            )
            |> Seq.toArray

        addedSand <- newSand.Length > 0
        for pos in newSand do
            sand.Add pos |> ignore

    sand

let stampOffsets =
    [ for row in -2..2 do
        for col in -2..2 do
            if (abs row = 2 && abs col <= 1)
               || abs row = 1
               || (row = 0 && abs col = 2) then
                yield (row, col) ]

let addStampedTrees (land: HashSet<int*int>) (sand: HashSet<int*int>) (map: char array array) =
    let trees = HashSet<int*int>()

    for (row, col) in land do
        if not (sand.Contains(row, col)) then
            let patternFits =
                stampOffsets
                |> List.forall (fun (rowOffset, colOffset) ->
                    let pos = (row + rowOffset, col + colOffset)
                    safePos map pos
                    && land.Contains pos
                    && not (sand.Contains pos)
                )

            if patternFits then
                trees.Add(row, col) |> ignore

    trees

let growForest (land: HashSet<int*int>) (sand: HashSet<int*int>) (map: char array array) (trees: HashSet<int*int>) =
    for _ in 1..2 do
        let newTrees =
            land
            |> Seq.filter (fun pos ->
                not (sand.Contains pos)
                && not (trees.Contains pos)
                && (neighbours map pos
                    |> Seq.filter trees.Contains
                    |> Seq.length >= 4)
                && (neighbours map pos
                    |> Seq.exists sand.Contains
                    |> not)
            )
            |> Seq.toArray

        for pos in newTrees do
            trees.Add pos |> ignore

    trees

let execute() =
    let map = LocalHelper.GetContent path 2026 5 3 |> Array.map _.ToCharArray()
    let land = travelMap map
    let sand = reconstructSand land map
    let trees = addStampedTrees land sand map
    growForest land sand map trees |> fun forest -> forest.Count