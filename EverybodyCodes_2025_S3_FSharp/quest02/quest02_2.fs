module quest02_2

open System.Numerics
open EverybodyCodes_2025_S3_FSharp.Modules

let path = "quest02/test_input_02.txt"
//let path = "quest02/quest02_input_02.txt"

type Pos = {
    Row: int
    Col: int
}

let mutable soundSource = { Row = 0; Col = 0 }
let mutable vocalBone = { Row = 0; Col = 0 }
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
            | '#' -> vocalBone <- { Row = rIdx; Col = cIdx }
            | '@' -> soundSource <- { soundSource with Row = rIdx; Col = cIdx }
            | _ -> ignore()
    if vocalBone.Row < soundSource.Row then
        vocalBone <- { vocalBone with Row = -vocalBone.Row - 1 }
    elif vocalBone.Row > soundSource.Row then
        vocalBone <- { vocalBone with Row = vocalBone.Row - soundSource.Row + 1}
    else
        vocalBone <- { vocalBone with Row = 0 }
    if vocalBone.Col < soundSource.Col then
        vocalBone <- { vocalBone with Col = -vocalBone.Col - 1 }
    elif vocalBone.Col > soundSource.Col then
        vocalBone <- { vocalBone with Col = vocalBone.Col - soundSource.Col + 1}
    else
        vocalBone <- { vocalBone with Col = 0 }

    soundSource <- { Row = 0; Col = 0 }

let step (moveCount: int) =
    let rec findNextSounce (count: int) =
        let nextMove = MoveSequence[count % MoveSequence.Length]
    
        let possiblePos =
            match nextMove with
            | UP -> { soundSource with Row = soundSource.Row - 1 }
            | DOWN -> { soundSource with Row = soundSource.Row + 1 }
            | LEFT -> { soundSource with Col = soundSource.Col - 1 }
            | RIGHT -> { soundSource with Col = soundSource.Col + 1 }
        if visitedWaves.Contains(possiblePos) || vocalBone = possiblePos then
            findNextSounce (count + 1)
        else
            visitedWaves.Add(possiblePos)
            soundSource <- possiblePos
            
            match nextMove with
            | UP -> printf "[^]"
            | DOWN -> printf "[v]"
            | LEFT -> printf "[<]"
            | RIGHT -> printf "[>]"

            count + 1
    findNextSounce moveCount

let printMap() =
    let minRow = min -5 (visitedWaves |> Seq.map _.Row |> Seq.min)
    let maxRow = max 5 (visitedWaves |> Seq.map _.Row |> Seq.max)
    let minCol = min -5 (visitedWaves |> Seq.map _.Col |> Seq.min)
    let maxCol = max 5 (visitedWaves |> Seq.map _.Col |> Seq.max)

    for rIdx in minRow..maxRow do
        for cIdx in minCol..maxCol do
            let pos = { Row = rIdx; Col = cIdx }
            if soundSource.Row = rIdx && soundSource.Col = cIdx then
                printf "%c" '@'
            elif vocalBone = pos then
                printf "%c" '#'
            elif visitedWaves.Contains(pos) then
                printf "%c" '+'
            else printf "%c" '.'
        printfn "%s" System.Environment.NewLine

let isSurrounded =
    visitedWaves.Contains({ vocalBone with Row = vocalBone.Row - 1}) &&
    visitedWaves.Contains({ vocalBone with Row = vocalBone.Row + 1}) &&
    visitedWaves.Contains({ vocalBone with Col = vocalBone.Col - 1}) &&
    visitedWaves.Contains({ vocalBone with Col = vocalBone.Col + 1})

let runVocal() =
    let mutable steps = 0
    let mutable counter = 0
    visitedWaves.Add(soundSource)
    printfn "Step %d" steps
    printMap()
    while not isSurrounded do
        counter <- step counter
        printfn "Step %d" steps
        steps <- steps + 1
        printMap()
    steps

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent lines |> ignore
    runVocal()