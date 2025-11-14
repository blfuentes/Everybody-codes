module quest10_3

open EverybodyCodes_2024_FSharp.Modules

//let path = "quest10/test_input_03.txt"
let path = "quest10/quest10_input_03.txt"

let parseContent (lines: string array) =
    lines |> Array.map (fun line -> line.ToCharArray())

let calculateValue (s: string) =
    s.ToCharArray()
    |> Array.mapi (fun i c -> (int c - int 'A' + 1) * (i + 1))
    |> Array.sum

let extractMiddle (arr: char[][]) =
    arr
    |> Array.skip 2
    |> Array.take 4
    |> Array.collect (fun row -> row |> Array.skip 2 |> Array.take 4)

let isSolved (flatGrid: char seq) =
    not (flatGrid |> Seq.contains '.')

let extractCard (mainGrid: char[][]) (col: int) (row: int) (spanX: int) (spanY: int) =
    Array.init 8 (fun y ->
        Array.init 8 (fun x ->
            mainGrid[row * spanY + y][col * spanX + x]
        )
    )

let updateCard (mainGrid: char[][]) (col: int) (row: int) (card: char[][]) (spanX: int) (spanY: int) =
    for y in 0..7 do
        for x in 0..7 do
            if card[y][x] <> '?' then
                mainGrid[row * spanY + y][col * spanX + x] <- card[y][x]

let solve (card: char[][]) =
    for y in 0..7 do
        for x in 0..7 do
            if card[y][x] = '.' then
                let rowChars = card[y] |> Seq.filter (fun c -> c <> '.') |> Set.ofSeq
                let colChars = card |> Seq.map (fun r -> r[x]) |> Seq.filter (fun c -> c <> '.') |> Set.ofSeq
                let intersection = Set.intersect rowChars colChars
                if intersection.Count = 1 then
                    card[y][x] <- Set.minElement intersection
    for y in 0..7 do
        for x in 0..7 do
            if card[y][x] = '.' then
                let row = card[y]
                let col = card |> Array.map (fun r -> r[x])
                if row |> Array.contains '?' || col |> Array.contains '?' then
                    let letters =
                        Seq.concat [row; col]
                        |> Seq.filter (fun c -> not (['?'; '.'] |> List.contains c))
                        |> Seq.countBy id
                        |> Seq.filter (fun (_, count) -> count = 1)
                        |> Seq.map fst
                        |> Seq.toList

                    if letters.Length = 1 then
                        let missingLetter = letters.Head
                        card[y][x] <- missingLetter
                        // Fill in question marks in the same row/column
                        for i in 0..7 do
                            if card[y][i] = '?' then card[y][i] <- missingLetter
                            if card[i][x] = '?' then card[i][x] <- missingLetter
    card

let run (mainGrid: char[][]) (cols: int) (rows: int) =
    let mutable res = 0
    let mutable prevRes = -1 // Ensure the loop runs at least once
    let spanX, spanY = 6, 6

    while prevRes <> res do
        prevRes <- res
        res <- 0

        for y in 0..rows - 1 do
            for x in 0..cols - 1 do
                let card = extractCard mainGrid x y spanX spanY
                let solvedCard = solve card
                
                if solvedCard |> Array.concat |> isSolved then
                    updateCard mainGrid x y solvedCard spanX spanY
                    let middleString = extractMiddle solvedCard |> System.String
                    res <- res + calculateValue middleString
    res

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let mainGrid = parseContent lines
    run mainGrid 20 10