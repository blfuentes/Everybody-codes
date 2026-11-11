module quest12_1

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

//let path = "quest12/test_input_01.txt"
let path = "quest12/quest12_input_01.txt"

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
    
    //let spells = HashSet<HashSet<(int*int)>>()

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
            //spells.Add(visited) |> ignore

            availablePoints.ExceptWith(visited)]
        
    spells

let countSpells (spell: HashSet<int*int>) =
    let rows = spell |> Seq.map fst
    let cols = spell |> Seq.map snd

    let (minRow, maxRow) = (rows |> Seq.min, rows |> Seq.max)
    let (minCol, maxCol) = (cols |> Seq.min, cols |> Seq.max)

    let dRows = maxRow - minRow
    let dCols = maxCol - minCol

    (min dRows dCols) + 1

let execute() =
    let lines = LocalHelper.GetContent path 2026 12 1
    parseContent lines
    let spells = buildSections()
    spells |> Seq.sumBy countSpells