module quest12_2

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

//let path = "quest12/test_input_02.txt"
// let path = "quest12/test_input_02b.txt"

let path = "quest12/quest12_input_02.txt"

let mutable(maxRow, maxCol) = (0, 0)

let availablePoints = HashSet<(int*int)>()

let parseContent (lines: string array) =
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    for row in 0..maxRow do
        for col in 0..maxCol do
            let c = lines[row][col]
            if c = '#' then
                availablePoints.Add((row, col)) |> ignore

let dirs = [(-1, 0); (1,0); (0, -1); (0, 1)]

let buildSections() =
    let spells =
        [while availablePoints.Count > 0 do
            let visited = HashSet<(int*int)>()
            let toVisit = Stack<(int*int)>()

            let start = availablePoints |> Seq.head
            toVisit.Push(start)
            let mutable current = Unchecked.defaultof<(int*int)>
            while toVisit.TryPop(&current) do
                visited.Add(current) |> ignore
                let (r, c) = current
                for (dr, dc) in dirs do
                    let (nr, nc) = (r+dr, c+dc)
                    if not (visited.Contains(nr, nc)) && availablePoints.Contains(nr, nc) then
                        toVisit.Push((nr, nc))
            yield visited

            availablePoints.ExceptWith(visited)]
        
    spells

let splitIntoRectangles (points: seq<int*int>) : HashSet<int*int> list =
    let remaining = HashSet(points)
    let rectangles = ResizeArray<HashSet<int*int>>()

    let hasPoint r c = remaining.Contains(r, c)

    while remaining.Count > 0 do
        // top-left point
        let (r0, c0) =
            remaining
            |> Seq.minBy (fun (r, c) -> r, c)

        // max width on top row
        let mutable w = 0
        while hasPoint r0 (c0 + w) do
            w <- w + 1

        // max height keeping full width
        let mutable h = 0
        let mutable canGrow = true
        while canGrow do
            let row = r0 + h
            let fullRow =
                [0 .. w - 1]
                |> List.forall (fun dc -> hasPoint row (c0 + dc))
            if fullRow then h <- h + 1 else canGrow <- false

        // build rectangle points and remove
        let rect = HashSet<int*int>()
        for r in r0 .. r0 + h - 1 do
            for c in c0 .. c0 + w - 1 do
                rect.Add((r, c)) |> ignore
                remaining.Remove((r, c)) |> ignore

        rectangles.Add(rect)

    rectangles |> Seq.toList

let countShapeSpells (fullshape: HashSet<int*int>) = 

    let countSpells (spell: HashSet<int*int>) =
        let rows = spell |> Seq.map fst
        let cols = spell |> Seq.map snd

        let (minRow, maxRow) = (rows |> Seq.min, rows |> Seq.max)
        let (minCol, maxCol) = (cols |> Seq.min, cols |> Seq.max)

        let dRows = maxRow - minRow
        let dCols = maxCol - minCol

        (min dRows dCols) + 1
    let countSplit points =
        splitIntoRectangles points |> List.sumBy countSpells

    let horizontalFirst = countSplit fullshape
    let verticalFirst = fullshape |> Seq.map (fun (row, col) -> (col, row)) |> countSplit
    let rows = fullshape |> Seq.map fst |> Set.ofSeq |> Set.count
    let cols = fullshape |> Seq.map snd |> Set.ofSeq |> Set.count
    [horizontalFirst; verticalFirst; rows; cols] |> List.min

let execute() =
    let lines = LocalHelper.GetContent path 2026 12 2
    parseContent lines
    let spells = buildSections()
    spells |> Seq.sumBy countShapeSpells
