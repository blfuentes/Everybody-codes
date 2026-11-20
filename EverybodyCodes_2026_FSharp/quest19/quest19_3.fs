module quest19_3

open EverybodyCodes_2026_FSharp.Modules

// let path = "quest19/test_input_03.txt"
let path = "quest19/quest19_input_03.txt"

let parseContent (lines: string[]) =
    Array2D.init lines.Length lines[0].Length (fun row col -> lines[row][col])

let mixColor ((layer1, layer2): char * char) =
    match (layer1, layer2) with
    | ('K', _) -> 'K'
    | (_, 'K') -> 'K'
    | ('C', 'M')
    | ('M', 'C') -> 'B'
    | ('C', 'Y')
    | ('Y', 'C') -> 'G'
    | ('M', 'Y')
    | ('Y', 'M') -> 'R'
    | (l1, l2) when l1 = l2 -> l1
    | _ -> failwith "invalid combination of layers"

let product (counts: int[]) =
    counts
    |> Array.map bigint
    |> Array.reduce (*)

let applySplit (counts: int[]) (amount, first, second) split =
    let updated = Array.copy counts
    updated[first] <- updated[first] + split
    updated[second] <- updated[second] + amount - split
    updated

let bestSplitProduct (counts: int[]) (amount, first, second) =
    let numerator = counts[second] + amount - counts[first]
    let lower = max 0 (min amount (numerator / 2))
    let upper = max 0 (min amount (numerator / 2 + 1))
    let remaining = 3 - first - second

    let score split =
        bigint (counts[first] + split)
        * bigint (counts[second] + amount - split)
        * bigint counts[remaining]

    [| lower; upper |]
    |> Array.distinct
    |> Array.map score
    |> Array.max

let private bestFlexibleProduct (counts: int[]) amount =
    let balanced = Array.copy counts

    for _ in 1..amount do
        let smallestIndex =
            [| 0 .. balanced.Length - 1 |]
            |> Array.minBy (fun index -> balanced[index])

        balanced[smallestIndex] <- balanced[smallestIndex] + 1

    product balanced

let bestBlueGreenRedProduct (sourceCounts: int[]) =
    let cyan = sourceCounts[0]
    let magenta = sourceCounts[1]
    let yellow = sourceCounts[2]
    let total = cyan + magenta + yellow
    let mutable best = 0I

    for blue in 0..cyan + magenta do
        let remaining = total - blue
        let minGreen = max 0 (cyan - blue)
        let maxGreen = cyan - max 0 (blue - magenta) + yellow
        let balancedGreen = remaining / 2
        let candidates =
            [| max minGreen (min maxGreen balancedGreen)
               max minGreen (min maxGreen (balancedGreen + 1)) |]

        for green in candidates do
            let score = bigint blue * bigint green * bigint (remaining - green)
            if score > best then best <- score

    best

let bestForColorTriple (sourceCounts: int[]) (targetColors: char[]) =
    let counts = Array.zeroCreate 3
    let splitGroups = ResizeArray<int * int * int>()
    let mutable flexibleGroup = None

    for (sourceColor, sourceIndex) in [| ('C', 0); ('M', 1); ('Y', 2) |] do
        let amount = sourceCounts[sourceIndex]

        let targets =
            [| 'C'; 'M'; 'Y' |]
            |> Array.map (fun secondColor -> mixColor (sourceColor, secondColor))
            |> Array.choose (fun resultColor ->
                targetColors
                |> Array.tryFindIndex ((=) resultColor))
            |> Array.distinct

        match targets with
        | [||] -> ()
        | [| only |] -> counts[only] <- counts[only] + amount
        | [| first; second |] -> splitGroups.Add((amount, first, second))
        | [| first; second; third |] -> flexibleGroup <- Some(amount, first, second, third)
        | _ -> failwith "unexpected number of available result colors"

    match targetColors |> Array.tryFindIndex ((=) 'K') with
    | Some blackIndex -> counts[blackIndex] <- counts[blackIndex] + sourceCounts[3]
    | None -> ()

    match flexibleGroup with
    | Some(amount, _, _, _) -> bestFlexibleProduct counts amount
    | None ->
        match splitGroups.Count with
        | 0 -> product counts
        | 1 -> bestSplitProduct counts splitGroups[0]
        | 2 ->
            let firstAmount, _, _ = splitGroups[0]
            let secondAmount, _, _ = splitGroups[1]
            let firstGroup, secondGroup =
                if firstAmount <= secondAmount then splitGroups[0], splitGroups[1]
                else splitGroups[1], splitGroups[0]

            let amount, _, _ = firstGroup
            let mutable best = 0I

            for split in 0..amount do
                let updated = applySplit counts firstGroup split
                let candidate = bestSplitProduct updated secondGroup
                if candidate > best then best <- candidate

            best
        | 3 when targetColors = [| 'B'; 'G'; 'R' |] -> bestBlueGreenRedProduct sourceCounts
        | 3 -> failwith "unexpected triple of split groups"
        | _ -> failwith "unexpected number of split groups"

let findMaxCrystalCubes (layer: char array2d) =
    let sourceCounts = Array.zeroCreate 4

    for row in 0 .. layer.GetLength(0) - 1 do
        for col in 0 .. layer.GetLength(1) - 1 do
            match layer[row, col] with
            | 'C' -> sourceCounts[0] <- sourceCounts[0] + 1
            | 'M' -> sourceCounts[1] <- sourceCounts[1] + 1
            | 'Y' -> sourceCounts[2] <- sourceCounts[2] + 1
            | 'K' -> sourceCounts[3] <- sourceCounts[3] + 1
            | color -> failwithf "invalid input color: %c" color

    let colors = [| 'C'; 'M'; 'Y'; 'K'; 'B'; 'G'; 'R' |]

    [| for first in 0 .. colors.Length - 3 do
           for second in first + 1 .. colors.Length - 2 do
               for third in second + 1 .. colors.Length - 1 do
                   yield
                       bestForColorTriple sourceCounts
                           [| colors[first]; colors[second]; colors[third] |] |]
    |> Array.max

let execute () =
    let lines = LocalHelper.GetContent path 2026 19 3
    lines |> parseContent |> findMaxCrystalCubes
