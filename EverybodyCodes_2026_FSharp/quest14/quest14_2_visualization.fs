module quest14_2_visualization

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open System.IO
open quest14_utilities

// let path = "quest14/test_input_02.txt"
// let path = "quest14/test_input_02b.txt"
let path = "quest14/quest14_input_02.txt"

let minimumBindings (grid: string array) =
    let height = grid.Length
    let width = grid[0].Length

    let pieces =
        grid
        |> Array.collect (fun row -> row.ToCharArray())
        |> Array.distinct

    let pieceIds = Dictionary<char, int>()
    pieces |> Array.iteri (fun index piece -> pieceIds.Add(piece, index))

    let frameCosts = Array.zeroCreate<int> pieces.Length
    let seamCosts = Dictionary<int * int, int>()

    let addSeam first second =
        let key = if first < second then first, second else second, first
        match seamCosts.TryGetValue key with
        | true, cost -> seamCosts[key] <- cost + 1
        | false, _ -> seamCosts.Add(key, 1)

    for row in 0 .. height - 1 do
        for column in 0 .. width - 1 do
            let current = pieceIds[grid[row][column]]

            if row = 0 then
                frameCosts[current] <- frameCosts[current] + 1
            else
                let above = pieceIds[grid[row - 1][column]]
                if current <> above then addSeam current above

            if column = 0 then
                frameCosts[current] <- frameCosts[current] + 1
            else
                let left = pieceIds[grid[row][column - 1]]
                if current <> left then addSeam current left

            if row = height - 1 then
                frameCosts[current] <- frameCosts[current] + 1

            if column = width - 1 then
                frameCosts[current] <- frameCosts[current] + 1

    let frameNode = pieces.Length
    let seamEdges =
        seamCosts
        |> Seq.map (fun entry ->
            let first, second = entry.Key
            first, second, entry.Value)
        |> Seq.toArray

    let frameEdges =
        [| for piece in 0 .. pieces.Length - 1 do
               if frameCosts[piece] > 0 then
                   yield piece, frameNode, frameCosts[piece] |]

    // Kruskal algorithm to find the minimum cost to connect all pieces to the frame
    // https://en.wikipedia.org/wiki/Kruskal%27s_algorithm
    let edges =
        Array.append seamEdges frameEdges
        |> Array.sortBy (fun (_, _, cost) -> cost)

    let parents = Array.init (pieces.Length + 1) id
    let ranks = Array.zeroCreate<int> (pieces.Length + 1)

    let rec find node =
        if parents[node] = node then
            node
        else
            parents[node] <- find parents[node]
            parents[node]

    let union first second =
        let firstRoot = find first
        let secondRoot = find second

        if firstRoot = secondRoot then
            false
        else
            if ranks[firstRoot] < ranks[secondRoot] then
                parents[firstRoot] <- secondRoot
            elif ranks[firstRoot] > ranks[secondRoot] then
                parents[secondRoot] <- firstRoot
            else
                parents[secondRoot] <- firstRoot
                ranks[firstRoot] <- ranks[firstRoot] + 1

            true

    let mutable total = 0
    let mutable connectedPieces = 0

    for first, second, cost in edges do
        if connectedPieces < pieces.Length && union first second then
            total <- total + cost
            connectedPieces <- connectedPieces + 1

    if connectedPieces <> pieces.Length then
        invalidOp "invalid connection???"

    total

let execute() =
    let lines = LocalHelper.GetContent path 2026 14 2
    let outputPath = Path.Combine(VisualizationFolder, "quest14_2_visualization.bmp")
    writeBitmap outputPath lines
    minimumBindings lines
