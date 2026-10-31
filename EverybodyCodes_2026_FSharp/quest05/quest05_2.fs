module quest05_2

open EverybodyCodes_2026_FSharp.Modules
open quest05_utilities
open System.Collections.Generic

// let path = "quest05/test_input_02.txt"
let path = "quest05/quest05_input_02.txt"

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

let execute() =
    let map = LocalHelper.GetContent path 2026 5 2 |> Array.map _.ToCharArray()
    let initialmap = travelMap map
    let sandMap = reconstructSand initialmap map
    sandMap.Count
