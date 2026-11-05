module quest08_utilities

open System.Collections.Generic

// doubled hex coordinates, the neighbours of (r, c) are (r, c±2) and (r +/- 1, c +/- 1)
let directions = [| (1, 1); (1, -1); (0, -2); (-1, -1); (-1, 1); (0, 2) |] // ↘ ↙ ← ↖ ↗ →

let parseField (lines: string array) =
    let field = Dictionary<int*int, int>()
    for r in 0 .. lines.Length - 1 do
        for c in 0 .. lines[r].Length - 1 do
            if lines[r][c] <> '.' then
                field[(r, c)] <- int (lines[r][c]) - int '0'
    field

// light paths for one direction, in travel order. light enters at hexes with no hex behind them
let rays (field: Dictionary<int*int, int>) (dr, dc) =
    let walk (r, c) = if field.ContainsKey((r, c)) then Some((r, c), (r + dr, c + dc)) else None
    [| for (r, c) in field.Keys do
         if not (field.ContainsKey((r - dr, c - dc))) then Array.unfold walk (r, c) |]

// seeds (height <= 0) take no energy and have no shadow, but every hex still lowers shadow
let litCells (field: Dictionary<int*int, int>) (rays: (int*int) array array) =
    let lit = ResizeArray<int*int>()
    for ray in rays do
        let mutable shadow = 0
        for cell in ray do
            let height = max 0 field.[cell]
            if height > shadow then lit.Add cell
            shadow <- max 0 (max shadow height - 1)
    lit

// original implementation... hand picked start cells and step counts per direction
module Previous =
    let mutable (maxRow, maxCol) = (0, 0)
    let mutable rowMinColByRow: int array = [||]
    let mutable rowMaxColByRow: int array = [||]
    let mutable entSlots = HashSet<int*int>()

    let parseContent(lines: string array) =
        let ents = Dictionary<int*int, int>()
        maxRow <- lines.Length - 1
        maxCol <- lines[0].Length - 1
        rowMinColByRow <- Array.create (maxRow + 1) -1
        rowMaxColByRow <- Array.create (maxRow + 1) -1
        entSlots <- HashSet<int*int>()

        for rIdx in 0..maxRow do
            let mutable minCol = -1
            let mutable maxColForRow = -1

            for cIdx in 0..maxCol do
                let c = lines[rIdx][cIdx]
                if c <> '.' then
                    if minCol = -1 then
                        minCol <- cIdx
                    maxColForRow <- cIdx
                    entSlots.Add((rIdx, cIdx)) |> ignore
                    ents.Add((rIdx, cIdx), int(c) - int('0'))

            rowMinColByRow[rIdx] <- minCol
            rowMaxColByRow[rIdx] <- maxColForRow

        ents

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
        if not (entSlots.Contains(key)) then
            currentShadow
        elif ents.ContainsKey(key) then
            let entSize = safeEntValue ents key
            if currentShadow > entSize then
                ents.Remove(key) |> ignore
                currentShadow - 1
            else
                entSize
        else
            max 0 (currentShadow - 1)

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

        for dc in 1..initRow do
            let start = secondPhaseStart dc
            applyShadowAlongPath ents start (initRow * 2 - (dc - 1)) direction

    let applyHorizontalShadow (ents: Dictionary<int*int, int>) (firstColByRow: int array) colStep =
        for r in 0..maxRow do
            let firstCol = firstColByRow.[r]
            let mutable currentShadow = safeEntValue ents (r, firstCol)
            for c in 0..maxCol do
                let key = (r, firstCol + (colStep * c))
                currentShadow <- updateShadowAtKey ents currentShadow key

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
        applyHorizontalShadow ents rowMaxColByRow -1

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
        applyHorizontalShadow ents rowMinColByRow 1

        ents.Values.Count

    let applyDirectionShadow directionIndex (ents: Dictionary<int*int, int>) =
        match directionIndex with
        | 0 -> shadowLefTop ents
        | 1 -> shadowRightTop ents
        | 2 -> shadowRight ents
        | 3 -> shadowRightBottom ents
        | 4 -> shadowLeftBottom ents
        | 5 -> shadowLeft ents
        | _ -> invalidArg "directionIndex" "Direction index must be in range 0..5"

    let energizedEnts directionIndex (activeEnts: Dictionary<int*int, int>) =
        let receivingEnts = clone activeEnts
        applyDirectionShadow directionIndex receivingEnts |> ignore
        receivingEnts

    let resolveSeeds (activeEnts: Dictionary<int*int, int>) (seedTimers: Dictionary<int*int, int>) =
        let keys = seedTimers.Keys |> Seq.toArray
        for key in keys do
            let remainingRounds = seedTimers[key]
            if remainingRounds = 0 then
                seedTimers.Remove(key) |> ignore
                activeEnts[key] <- 1
            else
                seedTimers[key] <- remainingRounds - 1

    let growArmy targetWarriors (initialEnts: Dictionary<int*int, int>) =
        let activeEnts = clone initialEnts
        let seedTimers = Dictionary<int*int, int>()

        let mutable warriors = 0
        let mutable totalEnergy = 0L
        let mutable round = 0

        while warriors < targetWarriors do
            resolveSeeds activeEnts seedTimers

            let directionIndex = round % 6
            let receivingEnts = energizedEnts directionIndex activeEnts
            let receivingKeys = receivingEnts.Keys |> Seq.toArray

            totalEnergy <- totalEnergy + int64 receivingKeys.Length

            for key in receivingKeys do
                let entHeight = activeEnts[key]
                if entHeight = 9 then
                    activeEnts.Remove(key) |> ignore
                    seedTimers[key] <- 3
                    warriors <- warriors + 1
                else
                    activeEnts[key] <- entHeight + 1

            round <- round + 1

        totalEnergy

