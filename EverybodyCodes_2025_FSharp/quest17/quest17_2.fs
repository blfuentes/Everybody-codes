module quest17_2

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest17/test_input_02.txt"
let path = "quest17/quest17_input_02.txt"

let mutable Xv, Yv = (0, 0)
let mutable maxX, maxY = (0, 0)

let parseContent (lines: string array) =
    maxX <- lines[0].ToCharArray() |> Array.length
    maxY <- lines.Length
    let field = Array2D.create maxX maxY 0
    for y in 0 .. lines.Length - 1 do
        for x in 0 .. lines[y].ToCharArray().Length - 1 do
            if lines[y][x] = '@' then
                Xv <- x
                Yv <- y
                field[x, y] <- 0
            else
                field[x, y] <- int((lines[y][x]).ToString())
    field

let isInRange (Xv, Yv) (Xc, Yc) R =
    (Xv - Xc) * (Xv - Xc) + (Yv - Yc) * (Yv - Yc) <= R * R

let printField (field: int[,]) =
    for y in 0 .. maxY - 1 do
        for x in 0 .. maxX - 1 do
            if (x, y) = (Xv, Yv) then
                printf "@"
            else
                printf "%s" (field[x, y].ToString())
        printfn ""

let calculateRange (field: int[,]) (radius: int) =
    let points = 
        seq {
            for y in 0 .. maxY - 1 do
                for x in 0 .. maxX - 1 do
                if isInRange (Xv, Yv) (x, y) radius then
                    yield field[x, y]
        }
    points |> Seq.sum

let getMaxRange (field: int[,]) =
    let maxRadius = field.GetLength(0) / 2
    let radiusEffects = Dictionary<int, int>()
    radiusEffects.Add(0, 0)
    let rec calculateRadiusEffect (remaining: int list) (prevSum: int) =
        match remaining with
        | [] -> 
            radiusEffects |> Seq.maxBy (fun kvp -> kvp.Value) |> (fun kvp -> (kvp.Key, kvp.Value))
        | r :: rs ->
            let currentSum = calculateRange field r
            radiusEffects.Add(r, currentSum - prevSum)
            calculateRadiusEffect rs currentSum
    let (radius, effect) = calculateRadiusEffect [1 .. maxRadius] 0
    radius * effect

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let field = parseContent lines
    getMaxRange field