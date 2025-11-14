module quest09_2

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest09/test_input_02.txt"
let path = "quest09/quest09_input_02.txt"

let parseContent (lines: string array) =
    let scales =
        lines
        |> Array.map(fun line ->
            let id = int(line.Split(":")[0])
            let dna = (line.Split(":")[1]).ToCharArray()
            (id, dna)
        )
    scales |> Map.ofArray

let isChild (childIdx: int) (scales: Map<int, char array>) =
    let child = scales[childIdx]
    let parentKeys = scales.Keys |> Seq.filter(fun k -> k <> childIdx)
    let rec matches(index: int) =
        if index = child.Length then
            true
        else
            let childChar = child[index]
            let parentChars =
                parentKeys
                |> Seq.map(fun k -> scales[k][index])
                |> Seq.toArray
            match Array.contains childChar parentChars with
            | true -> matches(index + 1)
            | false -> false
    matches 0

let buildFamily(scales: Map<int, char array>) =
    let child = 
        scales
        |> Map.keys
        |> Seq.tryFind(fun k -> isChild k scales)
    match child with
    | None -> (None, None)
    | Some child ->
        (Some (child, scales[child]), Some(scales |> Map.remove child))

let calculateSimilarity(childDna: char array) (parentDnas: Map<int, char array>) =
    let rec similarityHelper(index: int, parents: int array) =
        if index = childDna.Length then
            parents |> Array.fold(fun acc p -> acc * p) 1
        else
            parentDnas
            |> Map.toSeq
            |> Seq.iteri(fun i (_, parentDna) ->
                if childDna[index] = parentDna[index] then
                    parents[i] <- parents[i] + 1
            )
            similarityHelper(index + 1, parents)
    similarityHelper(0, [|0; 0|])

let rec comb n l =
    match (n,l) with
    | (0,_) -> [[]]
    | (_,[]) -> []
    | (n,x::xs) ->
        let useX = List.map (fun l -> x::l) (comb (n-1) xs)
        let noX = comb n xs
        (useX @ noX)

let buildFamilies(scales: Map<int, char array>) =
    let keys = scales |> Map.keys |> Seq.toArray
    let combinations = comb 3 (Array.toList keys)
    let families =
        combinations
        |> List.map(fun comb ->
            let possibleFamily = scales |> Map.filter(fun k _ -> List.contains k comb)
            let family = buildFamily possibleFamily
            match family with
            | (Some c, Some p) -> 
                (true, c, p)
            | _ -> 
                (false, (0,[||]), Map.empty)
        )
    families
    |> List.filter(fun (isFamily, _, _) -> isFamily)
    |> List.sumBy(fun f ->
        let (_, (_, child), parents) = f
        calculateSimilarity child parents
    )

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let scales = parseContent lines
    buildFamilies scales
