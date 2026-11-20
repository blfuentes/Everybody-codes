module quest19_2

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

// let path = "quest19/test_input_02.txt"
let path = "quest19/quest19_input_02.txt"

let parseContent (lines: string[]) =
    let (maxRow, maxCol) = (lines.Length / 2, lines[0].Length)
    // let firstLayer = Array2D.create<char> (lines.Length / 2) lines[0].Length
    // let secondLayer = Array2D.create<char> (lines.Length / 2) lines[0].Length
    let firstLayer = Array2D.init  maxRow maxCol (fun r j -> lines[r][j])
    let secondLayer = 
        Array2D.init maxRow maxCol (
            fun r j -> 
                // lines[r + maxRow + 1][j]
                let c = lines[r + maxRow + 1][j]
                c
                // match c with
                // | '1' -> 'M'
                // | '2' -> 'C'
                // | '3' -> 'Y'
                // | '4' -> 'K'
                // | _ -> failwith "invlid color assigment"
        )
    (firstLayer, secondLayer)

let mixColor((layer1, layer2): (char*char))=
    match (layer1, layer2) with
    | ('K', _) -> 'K'
    | (_, 'K') -> 'K'
    | ('C', 'M') -> 'B'
    | ('M', 'C') -> 'B'
    | ('C', 'Y') -> 'G'
    | ('Y', 'C') -> 'G'
    | ('M', 'Y') -> 'R'
    | ('Y', 'M') -> 'R'
    | (l1,l2) when l1 = l2 -> l1
    | _ -> failwith "invalid combination of layers"

let mixLayer(layer1: char array2d) (layer2: char array2d) (comb: Dictionary<char, char>) =
    let counterMix = Dictionary<char, bigint>()
    let mixedLayer = 
        Array2D.init (layer1.GetLength(0)) (layer1.GetLength(1)) 
            (fun r c ->
                let color = mixColor (layer1[r, c], comb[layer2[r, c]])
                match counterMix.TryAdd (color, 1I) with
                | true -> color
                | false -> 
                    counterMix[color] <- counterMix[color] + 1I
                    color
    )   
    counterMix.Values
    |> Seq.sortDescending
    |> Seq.take 3
    |> Seq.reduce ((*))

let findMaxComb (layer1: char array2d) (layer2: char array2d) =
    let comb = Dictionary<char, char>()
    let range = ['1'..'4']
    let areunique(a, b, c, d) =
        [a; b; c; d]
        |> Seq.distinct
        |> Seq.length = 4
    [for m in range do
        for c in range do
            for y in range do
                for k in range do
                    if areunique(m, c, y, k) then
                        comb.Clear()
                        comb.Add(m, 'M')
                        comb.Add(c, 'C')
                        comb.Add(y, 'Y')
                        comb.Add(k, 'K')
                        yield mixLayer layer1 layer2 comb
    ] |> Seq.max


let execute() =
    let lines = LocalHelper.GetContent path 2026 19 2
    let (firstLayer, secondLayer) = parseContent lines
    findMaxComb firstLayer secondLayer
