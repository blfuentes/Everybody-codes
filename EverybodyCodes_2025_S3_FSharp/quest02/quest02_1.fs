module quest02_1

open EverybodyCodes_2025_S3_FSharp.Modules

//let path = "quest02/test_input_01.txt"
let path = "quest02/quest02_input_01.txt"

type Pos = {
    Row: int
    Col: int
}

let mutable soundSource = { Row = 0; Col = 0 }
let vocalBones = new ResizeArray<Pos>()
let visitedWaves = new ResizeArray<Pos>()

let mutable maxRows = 0
let mutable maxCols = 0

type MoveType =
    | UP
    | DOWN
    | LEFT
    | RIGHT

let MoveSequence = [| UP; RIGHT; DOWN; LEFT |]

let parseContent(lines: string array) =
    maxRows <- lines.Length
    maxCols <- lines[0].Length
    for rIdx, row in lines |> Array.indexed do
        for cIdx, col in row |> Seq.indexed do
            match col with
            | '#' -> vocalBones.Add({Row = rIdx; Col = cIdx })
            | '@' -> soundSource <- { soundSource with Row = rIdx; Col = cIdx }
            | _ -> ignore()

let step (moveCount: int) =
    let rec findNextSounce (count: int) =
        let nextMove = MoveSequence[count % MoveSequence.Length]
    
        let possiblePos =
            match nextMove with
            | UP -> { soundSource with Row = soundSource.Row - 1 }
            | DOWN -> { soundSource with Row = soundSource.Row + 1 }
            | LEFT -> { soundSource with Col = soundSource.Col - 1 }
            | RIGHT -> { soundSource with Col = soundSource.Col + 1 }
        if visitedWaves.Contains(possiblePos) then
            findNextSounce (count + 1)
        else
            visitedWaves.Add(possiblePos)
            soundSource <- possiblePos
            count + 1
    findNextSounce moveCount

let printMap() =
    for rIdx in 0..maxRows do
        for cIdx in 0..maxCols do
            let pos = { Row = rIdx; Col = cIdx }
            if soundSource.Row = rIdx && soundSource.Col = cIdx then
                printf "%c" '@'
            elif visitedWaves.Contains(pos) then
                printf "%c" '+'
            elif vocalBones.Contains(pos) then
                printf "%c" '#'
            else printf "%c" '.'
        printfn "%s" System.Environment.NewLine

let runVocal() =
    let mutable steps = 0
    let mutable counter = 0
    visitedWaves.Add(soundSource)
    while not (vocalBones.Contains(soundSource)) do
        counter <- step counter
        steps <- steps + 1
        //printfn "Step %d" steps
        //printMap()
    steps

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent lines |> ignore
    runVocal()