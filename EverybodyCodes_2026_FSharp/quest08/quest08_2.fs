module quest08_2

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

//let path = "quest08/test_input_02.txt"
let path = "quest08/quest08_input_02.txt"

let mutable (maxRow, maxCol) = (0, 0)

let parseContent(lines: string array) =
    let ents = Dictionary<int*int, int>()
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    for rIdx in 0..maxRow do
        for cIdx in 0..maxCol do
            let c = lines[rIdx][cIdx]
            if c <> '.' then
                ents.Add((rIdx, cIdx), int(c) - int('0'))
    ents
let printMap (ents: Dictionary<int*int, int>) =
    for r in 0..maxRow do
        for c in 0..maxCol do
            let key = (r, c)
            if ents.ContainsKey(key) then
                printf "%d" ents[key]
            else
                printf "."
        printfn "%s" System.Environment.NewLine

let safeEntValue (ents: Dictionary<int*int, int>) (r, c) =
    let mutable entValue = 0
    ents.TryGetValue((r, c), &entValue) |> ignore
    entValue

let clone (ents: Dictionary<int*int, int>) =
    let cloned = Dictionary<int*int, int>()
    for e in ents do
        cloned.Add(e.Key, e.Value)
    cloned

let updateShadowAtKey (ents: Dictionary<int*int, int>) currentShadow key =
    if ents.ContainsKey(key) then
        let entSize = safeEntValue ents key
        if currentShadow > entSize then
            ents.Remove(key) |> ignore
            currentShadow - 1
        else
            entSize
    else
        currentShadow

let applyShadowAlongPath (ents: Dictionary<int*int, int>) (startRow, startCol) steps (rowStep, colStep) =
    let mutable currentShadow = safeEntValue ents (startRow, startCol)
    for counter in 1..steps do
        let key = (startRow + (rowStep * counter), startCol + (colStep * counter))
        currentShadow <- updateShadowAtKey ents currentShadow key

let applyDiagonalShadow
    (ents: Dictionary<int*int, int>)
    initRow
    firstPhaseStart
    secondPhaseStart
    direction =
    for dr in 0..initRow do
        let start = firstPhaseStart dr
        applyShadowAlongPath ents start (initRow + dr) direction
        //printfn "After diagonal %d" dr
        //printMap ents

    for dc in 1..initRow do
        let start = secondPhaseStart dc
        applyShadowAlongPath ents start (initRow * 2 - (dc - 1)) direction
        //printfn "After lateral %d" dc
        //printMap ents

let applyHorizontalShadow (ents: Dictionary<int*int, int>) firstColByRow colStep =
    for r in 0..maxRow do
        let firstCol = firstColByRow r
        let mutable currentShadow = safeEntValue ents (r, firstCol)
        for c in 0..maxCol do
            let key = (r, firstCol + (colStep * c))
            currentShadow <- updateShadowAtKey ents currentShadow key
        //printfn "After Row %d" r
        //printMap ents

let shadowLefTop (ents: Dictionary<int*int, int>) =
    //printfn "Direction ↘"
    let initRow = maxRow / 2

    applyDiagonalShadow
        ents
        initRow
        (fun dr -> (initRow - dr, dr))
        (fun dc -> (0, initRow + dc * 2))
        (1, 1)

    ents.Values.Count

let shadowRightTop (ents: Dictionary<int*int, int>) =
    //printfn "Direction ↙"
    let initRow = maxRow / 2

    applyDiagonalShadow
        ents
        initRow
        (fun dr -> (initRow - dr, maxCol - dr))
        (fun dc -> (0, maxCol - (initRow + dc * 2)))
        (1, -1)

    ents.Values.Count

let shadowRight (ents: Dictionary<int*int, int>) =
    //printfn "Direction ←"
    applyHorizontalShadow
        ents
        (fun r -> ents.Keys |> Seq.filter (fun (rk, _) -> rk = r) |> Seq.map snd |> Seq.max)
        -1

    ents.Values.Count

let shadowRightBottom (ents: Dictionary<int*int, int>) =
    //printfn "Direction ↖"
    let initRow = maxRow / 2

    applyDiagonalShadow
        ents
        initRow
        (fun dr -> (initRow + dr, maxCol - dr))
        (fun dc -> (maxRow, maxCol - (initRow + dc * 2)))
        (-1, -1)

    ents.Values.Count

let shadowLeftBottom (ents: Dictionary<int*int, int>) = 
    //printfn "Direction ↗"
    let initRow = maxRow / 2

    applyDiagonalShadow
        ents
        initRow
        (fun dr -> (initRow + dr, dr))
        (fun dc -> (maxRow, initRow + dc * 2))
        (-1, 1)

    ents.Values.Count

let shadowLeft (ents: Dictionary<int*int, int>) =
    //printfn "Direction →"
    applyHorizontalShadow
        ents
        (fun r -> ents.Keys |> Seq.filter (fun (rk, _) -> rk = r) |> Seq.map snd |> Seq.min)
        1

    ents.Values.Count

let applyShadow (ents: Dictionary<int*int, int>) =
    //printfn "Initial map"
    //printMap (ents)

    // left-top
    let lt = shadowLefTop (clone ents)

    // right-top
    let rt = shadowRightTop (clone ents)

    // right
    let r = shadowRight (clone ents)

    // right-bottom
    let rb = shadowRightBottom (clone ents)

    // left-bottom
    let lb = shadowLeftBottom (clone ents)

    // left
    let l = shadowLeft (clone ents)

    lt + rt + r + rb + lb + l
    //l

let execute() =
    let lines = LocalHelper.GetContent path 2026 8 1
    let ents = parseContent lines
    applyShadow ents
