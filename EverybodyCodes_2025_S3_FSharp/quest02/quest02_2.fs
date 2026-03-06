module quest02_2

open EverybodyCodes_2025_S3_FSharp.Modules

//let path = "quest02/test_input_02.txt"
let path = "quest02/quest02_input_02.txt"

type Pos = {
    Row: int
    Col: int
}

let mutable soundSource = { Row = 0; Col = 0 }
let mutable vocalBone = { Row = 0; Col = 0 }
let visitedSet = System.Collections.Generic.HashSet<Pos>()

let mutable bbMinRow = System.Int32.MaxValue
let mutable bbMaxRow = System.Int32.MinValue
let mutable bbMinCol = System.Int32.MaxValue
let mutable bbMaxCol = System.Int32.MinValue

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

let MoveSequence = [| UP; RIGHT; DOWN; LEFT |]

let parseContent(lines: string array) =
    for rIdx, row in lines |> Array.indexed do
        for cIdx, col in row |> Seq.indexed do
            match col with
            | '#' -> vocalBone <- { Row = rIdx; Col = cIdx }
            | '@' -> soundSource <- { Row = rIdx; Col = cIdx }
            | _ -> ignore()
    vocalBone <- { Row = vocalBone.Row - soundSource.Row; Col = vocalBone.Col - soundSource.Col }
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
            elif vocalBone = pos then
                printf "%c" '#'
            elif visitedSet.Contains(pos) then
                printf "%c" '+'
            else printf "%c" '.'
        printfn "%s" System.Environment.NewLine

let fillEnclosedSpaces() =
    if visitedSet.Count < 4 then ()
    else
    let minRow = bbMinRow - 1
    let maxRow = bbMaxRow + 1
    let minCol = bbMinCol - 1
    let maxCol = bbMaxCol + 1

    let outside = System.Collections.Generic.HashSet<Pos>()
    let queue = System.Collections.Generic.Queue<Pos>()

    for r in minRow..maxRow do
        for c in minCol..maxCol do
            if r = minRow || r = maxRow || c = minCol || c = maxCol then
                let pos = { Row = r; Col = c }
                if not (visitedSet.Contains(pos)) && pos <> vocalBone then
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
                if not (visitedSet.Contains(n)) && n <> vocalBone then
                    if outside.Add(n) then
                        queue.Enqueue(n)

    let toFill = ResizeArray<Pos>()
    for r in (minRow + 1)..(maxRow - 1) do
        for c in (minCol + 1)..(maxCol - 1) do
            let pos = { Row = r; Col = c }
            if not (outside.Contains(pos)) && not (visitedSet.Contains(pos)) && pos <> vocalBone then
                toFill.Add(pos)

    for pos in toFill do
        addWave pos

let step (currentStep: int) (moveCount: int) =
    let rec findNextSounce (count: int) =
        let nextMove = MoveSequence[count % MoveSequence.Length]
    
        let possiblePos =
            match nextMove with
            | UP -> { soundSource with Row = soundSource.Row - 1 }
            | DOWN -> { soundSource with Row = soundSource.Row + 1 }
            | LEFT -> { soundSource with Col = soundSource.Col - 1 }
            | RIGHT -> { soundSource with Col = soundSource.Col + 1 }

        if visitedSet.Contains(possiblePos) || vocalBone = possiblePos then
            findNextSounce (count + 1)
        else
            addWave possiblePos
            fillEnclosedSpaces()
            soundSource <- possiblePos

            count + 1

    findNextSounce moveCount



let isSurrounded() =
    visitedSet.Contains({ vocalBone with Row = vocalBone.Row - 1 }) &&
    visitedSet.Contains({ vocalBone with Row = vocalBone.Row + 1 }) &&
    visitedSet.Contains({ vocalBone with Col = vocalBone.Col - 1 }) &&
    visitedSet.Contains({ vocalBone with Col = vocalBone.Col + 1 })

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