module quest05_utilities

open System.Collections.Generic

let travelMap (map: char array array) =
    let rows = map.Length
    let cols = if rows = 0 then 0 else map[0].Length

    if cols = 0 then
        HashSet<int * int>()
    else
        let size = rows * cols
        let neighbourCounts = Array.zeroCreate<int> size
        let isLand = Array.zeroCreate<bool> size
        let isQueued = Array.zeroCreate<bool> size
        let frontier = Queue<int>()
        let land = HashSet<int * int>()

        let eachNeighbour index action =
            let row = index / cols
            let col = index % cols

            for rowOffset in -1..1 do
                for colOffset in -1..1 do
                    if rowOffset <> 0 || colOffset <> 0 then
                        let neighbourRow = row + rowOffset
                        let neighbourCol = col + colOffset

                        if neighbourRow >= 0 && neighbourRow < rows
                           && neighbourCol >= 0 && neighbourCol < cols then
                            action (neighbourRow * cols + neighbourCol)

        for index in 0..size - 1 do
            eachNeighbour index (fun neighbour ->
                if map[neighbour / cols][neighbour % cols] = '1' then
                    neighbourCounts[index] <- neighbourCounts[index] + 1
            )

        let startRow = rows / 2
        let startCol = cols / 2
        let start = startRow * cols + startCol
        isLand[start] <- true
        land.Add(startRow, startCol) |> ignore

        if map[startRow][startCol] <> '1' then
            eachNeighbour start (fun neighbour ->
                neighbourCounts[neighbour] <- neighbourCounts[neighbour] + 1
            )

        let enqueue index =
            if not isLand[index] && not isQueued[index] then
                isQueued[index] <- true
                frontier.Enqueue index

        eachNeighbour start enqueue

        while frontier.Count > 0 do
            let current = frontier.Dequeue()
            isQueued[current] <- false

            if not isLand[current] && neighbourCounts[current] >= 5 then
                let row = current / cols
                let col = current % cols
                isLand[current] <- true
                land.Add(row, col) |> ignore

                eachNeighbour current (fun neighbour ->
                    if map[row][col] <> '1' then
                        neighbourCounts[neighbour] <- neighbourCounts[neighbour] + 1
                    enqueue neighbour
                )

        land
