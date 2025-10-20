module quest10_part01

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

//let path = "quest10/test_input_01.txt"
let path = "quest10/quest10_input_01.txt"

let shrineMap = Array2D.create 8 8 '.'
let shrineSolved = Array2D.create 8 8 '.'
let runesByRow = new Dictionary<int, Set<char>>()
let runesByCol = new Dictionary<int, Set<char>>()

let parseContent (lines: string[]) =
    let toFill =     
        seq {
            for row, line in Array.indexed lines do
                for col, char in Array.indexed (line.ToCharArray()) do
                    shrineMap[row, col] <- char
                    shrineSolved[row, col] <- char                
                    if char = '.' then
                        yield (row, col)
        } |> Seq.toList
    for row in 0 .. shrineMap.GetLength(0) - 1 do
        let rowRunes = 
            shrineMap[row, *] 
            |> Seq.filter (fun c -> c <> '.') 
            |> Set.ofSeq
        runesByRow.Add(row, rowRunes)
    for col in 0 .. shrineMap.GetLength(1) - 1 do
        let colRunes = 
            shrineMap[*, col] 
            |> Seq.filter (fun c -> c <> '.') 
            |> Set.ofSeq
        runesByCol.Add(col, colRunes)

    toFill

let printMap(themap: char[,]) =
    let rows = themap.GetLength(0)
    let cols = themap.GetLength(1)
    for r in 0 .. rows - 1 do
        for c in 0 .. cols - 1 do
            printf "%c" themap[r, c]
        printfn ""

let findRune (pos: int*int) =
    let rowRunes = runesByRow[fst pos]
    let colRunes = runesByCol[snd pos]
    let rune = Set.intersect rowRunes colRunes |> Set.minElement
    shrineSolved[fst pos, snd pos] <- rune
    rune

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let toFill = parseContent lines
    printMap shrineMap
    let runes = 
        toFill
        |> Seq.map (fun pos -> findRune pos)        

    printMap shrineSolved
    
    let resultString = new string(Seq.toArray runes)
    resultString
