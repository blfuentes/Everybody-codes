module quest02_3

open EverybodyCodes_2025_S3_FSharp.Modules

//let path = "quest02/test_input_03.txt"
let path = "quest02/quest02_input_03.txt"

type Pos = {
    Row: int
    Col: int
}

let mutable soundSource = { Row = 0; Col = 0 }
let vocalBones = System.Collections.Generic.HashSet<Pos>()
let visitedSet = System.Collections.Generic.HashSet<Pos>()

let mutable bbMinRow = System.Int32.MaxValue
let mutable bbMaxRow = System.Int32.MinValue
let mutable bbMinCol = System.Int32.MaxValue
let mutable bbMaxCol = System.Int32.MinValue

let mutable boneMinRow = System.Int32.MaxValue
let mutable boneMaxRow = System.Int32.MinValue
let mutable boneMinCol = System.Int32.MaxValue
let mutable boneMaxCol = System.Int32.MinValue

let addWave (pos: Pos) =
    if visitedSet.Add(pos) then
        bbMinRow <- min bbMinRow pos.Row
        bbMaxRow <- max bbMaxRow pos.Row
        bbMinCol <- min bbMinCol pos.Col
        bbMaxCol <- max bbMaxCol pos.Col

type MoveType =
    | UP
    | DOWN
    | LEFT
    | RIGHT

let MoveSequence = [| UP; UP; UP; RIGHT; RIGHT; RIGHT; DOWN; DOWN; DOWN; LEFT; LEFT; LEFT |]

let parseContent(lines: string array) =
    let rawBones = ResizeArray<Pos>()
    for rIdx, row in lines |> Array.indexed do
        for cIdx, col in row |> Seq.indexed do
            match col with
            | '#' -> rawBones.Add({ Row = rIdx; Col = cIdx })
            | '@' -> soundSource <- { Row = rIdx; Col = cIdx }
            | _ -> ignore()
    for bone in rawBones do
        let normalized = { Row = bone.Row - soundSource.Row; Col = bone.Col - soundSource.Col }
        vocalBones.Add(normalized) |> ignore
        boneMinRow <- min boneMinRow normalized.Row
        boneMaxRow <- max boneMaxRow normalized.Row
        boneMinCol <- min boneMinCol normalized.Col
        boneMaxCol <- max boneMaxCol normalized.Col
    soundSource <- { Row = 0; Col = 0 }

let printMap() =
    let minRow = min -5 bbMinRow
    let maxRow = max 5 bbMaxRow
    let minCol = min -5 bbMinCol
    let maxCol = max 5 bbMaxCol

    for rIdx in minRow..maxRow do
        for cIdx in minCol..maxCol do
            let pos = { Row = rIdx; Col = cIdx }
            if soundSource.Row = rIdx && soundSource.Col = cIdx then
                printf "%c" '@'
            elif vocalBones.Contains(pos) then
                printf "%c" '#'
            elif visitedSet.Contains(pos) then
                printf "%c" '+'
            else printf "%c" '.'
        printfn "%s" System.Environment.NewLine

let fillEnclosedSpaces() =
    if visitedSet.Count < 4 then ()
    else
    let minRow = (min bbMinRow boneMinRow) - 1
    let maxRow = (max bbMaxRow boneMaxRow) + 1
    let minCol = (min bbMinCol boneMinCol) - 1
    let maxCol = (max bbMaxCol boneMaxCol) + 1

    let outside = System.Collections.Generic.HashSet<Pos>()
    let queue = System.Collections.Generic.Queue<Pos>()

    for r in minRow..maxRow do
        for c in minCol..maxCol do
            if r = minRow || r = maxRow || c = minCol || c = maxCol then
                let pos = { Row = r; Col = c }
                if not (visitedSet.Contains(pos)) && not (vocalBones.Contains(pos)) then
                    if outside.Add(pos) then
                        queue.Enqueue(pos)

    while queue.Count > 0 do
        let current = queue.Dequeue()
        let neighbors = [|
            { current with Row = current.Row - 1 }
            { current with Row = current.Row + 1 }
            { current with Col = current.Col - 1 }
            { current with Col = current.Col + 1 }
        |]
        for n in neighbors do
            if n.Row >= minRow && n.Row <= maxRow && n.Col >= minCol && n.Col <= maxCol then
                if not (visitedSet.Contains(n)) && not (vocalBones.Contains(n)) then
                    if outside.Add(n) then
                        queue.Enqueue(n)

    let toFill = ResizeArray<Pos>()
    for r in (minRow + 1)..(maxRow - 1) do
        for c in (minCol + 1)..(maxCol - 1) do
            let pos = { Row = r; Col = c }
            if not (outside.Contains(pos)) && not (visitedSet.Contains(pos)) && not (vocalBones.Contains(pos)) then
                toFill.Add(pos)

    for pos in toFill do
        addWave pos

let step (currentStep: int) (moveCount: int) =
    let moveToPos (possiblePos: Pos) =
        addWave possiblePos
        fillEnclosedSpaces()
        soundSource <- possiblePos

    let rec findNextSounce (count: int) =
        if count >= moveCount + MoveSequence.Length then
            let rec findThroughBone (count2: int) =
                let nextMove = MoveSequence[count2 % MoveSequence.Length]
                let possiblePos =
                    match nextMove with
                    | UP -> { soundSource with Row = soundSource.Row - 1 }
                    | DOWN -> { soundSource with Row = soundSource.Row + 1 }
                    | LEFT -> { soundSource with Col = soundSource.Col - 1 }
                    | RIGHT -> { soundSource with Col = soundSource.Col + 1 }
                if visitedSet.Contains(possiblePos) then
                    findThroughBone (count2 + 1)
                else
                    moveToPos possiblePos
                    count2 + 1
            findThroughBone moveCount
        else
            let nextMove = MoveSequence[count % MoveSequence.Length]
            let possiblePos =
                match nextMove with
                | UP -> { soundSource with Row = soundSource.Row - 1 }
                | DOWN -> { soundSource with Row = soundSource.Row + 1 }
                | LEFT -> { soundSource with Col = soundSource.Col - 1 }
                | RIGHT -> { soundSource with Col = soundSource.Col + 1 }

            if visitedSet.Contains(possiblePos) || vocalBones.Contains(possiblePos) then
                findNextSounce (count + 1)
            else
                moveToPos possiblePos
                count + 1

    findNextSounce moveCount



let isNeighborCovered (pos: Pos) =
    visitedSet.Contains(pos) || vocalBones.Contains(pos)

let isSurrounded() =
    vocalBones |> Seq.forall (fun bone ->
        isNeighborCovered { bone with Row = bone.Row - 1 } &&
        isNeighborCovered { bone with Row = bone.Row + 1 } &&
        isNeighborCovered { bone with Col = bone.Col - 1 } &&
        isNeighborCovered { bone with Col = bone.Col + 1 })

let runVocal() =
    let mutable steps = 0
    let mutable counter = 0
    addWave soundSource
    while not (isSurrounded()) do
        counter <- step steps counter
        steps <- steps + 1
    //printMap()
    steps

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent lines |> ignore
    runVocal()