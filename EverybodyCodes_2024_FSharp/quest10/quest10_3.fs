module quest10_part03

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

let path = "quest10/test_input_03.txt"
//let path = "quest10/quest10_input_03.txt"

type Defintion = {
    ShrineMap: char[,]
    ShrineSolved: char[,]
    RunesByRow: Dictionary<int, Set<char>>
    RunesByCol: Dictionary<int, Set<char>>
    ToFill: (int * int) list
}

let parseFull (lines: string[]) =
    let step = 6
    let blockHeight = ((lines.Length - 8) / step) + 1
    let blockWidth = ((lines[0].Length - 8) / step) + 1

    let allRunes =
        seq {
            for row in 0..blockHeight - 1 do
                for col in 0..blockWidth - 1 do
                    let shrineMap = Array2D.create 8 8 '.'
                    let shrineSolved = Array2D.create 8 8 '.'
                    let runesByRow = new Dictionary<int, Set<char>>()
                    let runesByCol = new Dictionary<int, Set<char>>()
                    let toFill =
                        seq {
                            for rIdx in 0..7 do
                                for cIdx in 0..7 do
                                    let char = lines[rIdx + row * step][cIdx + col * step]
                                    shrineMap[rIdx, cIdx] <- char
                                    shrineSolved[rIdx, cIdx] <- char  
                                    if char = '.' then
                                        yield (rIdx, cIdx)
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
                    yield { 
                        ShrineMap = shrineMap;
                        ShrineSolved = shrineSolved;
                        RunesByRow = runesByRow;
                        RunesByCol = runesByCol;
                        ToFill = toFill 
                    }
        } |> Seq.toList
    allRunes

let printMap(themap: char[,]) =
    let rows = themap.GetLength(0)
    let cols = themap.GetLength(1)
    for r in 0 .. rows - 1 do
        for c in 0 .. cols - 1 do
            printf "%c" themap[r, c]
        printfn ""

let findRune (pos: int*int) (themap: char[,]) (runesByRow: Dictionary<int, Set<char>>) (runesByCol: Dictionary<int, Set<char>>)=
    let rowRunes = runesByRow[fst pos]
    let colRunes = runesByCol[snd pos]
    if rowRunes.Contains '?' || colRunes.Contains '?' then
        '?'
    else
        let rune = Set.intersect rowRunes colRunes |> Set.minElement
        themap[fst pos, snd pos] <- rune
        rune

let calculatePower (runes: char array) =
    let weights = [|'A'..'Z'|] |> Array.mapi (fun i c -> (c, i + 1)) |> dict
    runes 
    |> Array.mapi(fun i c -> 
        if c = '?' then 0 else weights[c] * (i + 1)) 
    |> Array.sum


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let allRunes = parseFull lines
    allRunes
    |> List.map(fun runeDef ->
        let toFill = runeDef.ToFill
        let shrineSolved = runeDef.ShrineSolved
        let runesByRow = runeDef.RunesByRow
        let runesByCol = runeDef.RunesByCol
        let findRuneLocal = findRune >> (fun f -> f shrineSolved runesByRow runesByCol)
        let runes = 
            toFill
            |> Seq.map (fun pos -> 
                let rune = findRuneLocal pos
                printMap (shrineSolved)
                rune
                )
            |> Seq.toArray
        calculatePower runes
    )
    |> Seq.sum
