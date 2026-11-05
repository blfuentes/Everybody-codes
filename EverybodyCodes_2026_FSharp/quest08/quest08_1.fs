module quest08_1

open System.Collections.Generic
open EverybodyCodes_2026_FSharp.Modules

//let path = "quest08/test_input_01.txt"
let path = "quest08/quest08_input_01.txt"

let ents = Dictionary<int*int, int>()
let mutable (maxRow, maxCol) = (0, 0)

let parseContent(lines: string array) =
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    for rIdx in 0..maxRow do
        for cIdx in 0..maxCol do
            let c = lines[rIdx][cIdx]
            if c <> '.' then
                ents.Add((rIdx, cIdx), int(c) - int('0'))

let printMap () =
    for r in 0..maxRow do
        for c in 0..maxCol do
            let key = (r, c)
            if ents.ContainsKey(key) then
                printf "%d" ents[key]
            else
                printf "."
        printfn "%s" System.Environment.NewLine

let applyShadow() =
    //printfn "Initial map"
    //printMap ()
    let mutable (initRow, initCol) = (maxRow / 2, 0)
    // traveling from left to top
    for dr in 0..initRow do
        let (r, c) = (initRow - dr, dr)
        let mutable diagonalSize = initRow + dr
        let mutable counter = 1
        let mutable currentShadow = ents[(r, c)]
        while diagonalSize > 0 do
            let (cr, cc) = (r + counter, c + counter)
            let key = (cr, cc)
            if ents.ContainsKey(key) then
                let entsize = ents[key]
                currentShadow <-
                    if currentShadow > entsize then
                        ents.Remove(key) |> ignore
                        currentShadow - 1
                    else
                        ents[key]
            diagonalSize <- diagonalSize - 1
            counter <- counter + 1
        //printfn "After diagonal %d" dr
        //printMap ()
    // traveling from top to right
    for dc in 1..(initRow) do
        let (r, c) = (0, initRow + dc * 2)
        let mutable diagonalSize = initRow * 2 - (dc - 1)
        let mutable counter = 1
        let mutable currentShadow = ents[(r, c)]
        while diagonalSize > 0 do
            let (cr, cc) = (r + counter, c + counter)
            let key = (cr, cc)
            if ents.ContainsKey(key) then
                let entsize = ents[key]
                currentShadow <-
                    if currentShadow > entsize then
                        ents.Remove(key) |> ignore
                        currentShadow - 1
                    else
                        ents[key]
            diagonalSize <- diagonalSize - 1
            counter <- counter + 1
        //printfn "After lateral %d" dc
        //printMap()
    ents.Values.Count

let execute() =
    let lines = LocalHelper.GetContent path 2026 8 1
    parseContent lines
    applyShadow ()
