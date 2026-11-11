module quest12_3

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

// let path = "quest12/test_input_03.txt"
//let path = "quest12/test_input_03b.txt"
//let path = "quest12/test_input_03c.txt"

let path = "quest12/quest12_input_03.txt"

let mutable(maxRow, maxCol) = (0, 0)

let availablePoints = HashSet<(int*int)>()

let parseContent (lines: string array) =
    availablePoints.Clear()
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
            visited.Add(start) |> ignore
            toVisit.Push(start)
            let mutable current = Unchecked.defaultof<(int*int)>
            while toVisit.TryPop(&current) do
                let (r, c) = current
                for (dr, dc) in dirs do
                    let (nr, nc) = (r+dr, c+dc)
                    if availablePoints.Contains(nr, nc) && visited.Add((nr, nc)) then
                        toVisit.Push((nr, nc))
            yield visited

            availablePoints.ExceptWith(visited)]
        
    spells

let countShapeSpells (fullshape: HashSet<int*int>) =
    let horizontalSegments = Dictionary<int*int, int>()
    let verticalSegments = Dictionary<int*int, int>()
    let edges = ResizeArray<ResizeArray<int>>()
    let mutable verticalCount = 0

    for (row, col) in fullshape |> Seq.sort do
        let horizontal =
            match horizontalSegments.TryGetValue((row, col - 1)) with
            | true, id -> id
            | false, _ ->
                edges.Add(ResizeArray<int>())
                edges.Count - 1

        let vertical =
            match verticalSegments.TryGetValue((row - 1, col)) with
            | true, id -> id
            | false, _ ->
                let id = verticalCount
                verticalCount <- verticalCount + 1
                id

        horizontalSegments.Add((row, col), horizontal)
        verticalSegments.Add((row, col), vertical)
        edges[horizontal].Add(vertical)

    let matchedHorizontal = Array.create verticalCount -1

    let rec tryMatch horizontal (visited: bool array) =
        edges[horizontal]
        |> Seq.exists (fun vertical ->
            if visited[vertical] then false
            else
                visited[vertical] <- true
                if matchedHorizontal[vertical] = -1 || tryMatch matchedHorizontal[vertical] visited then
                    matchedHorizontal[vertical] <- horizontal
                    true
                else false)

    // Konig theorem: maximum matching equals the minimum segment cover
    [0 .. edges.Count - 1]
    |> List.sumBy (fun horizontal ->
        if tryMatch horizontal (Array.create verticalCount false) then 1 else 0)

let execute() =
    let lines = LocalHelper.GetContent path 2026 12 3
    parseContent lines
    let spells = buildSections()
    spells |> Seq.sumBy countShapeSpells
