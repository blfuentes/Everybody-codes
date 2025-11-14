module quest09_part03

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest09/test_input_03.txt"
let path = "quest09/quest09_input_03.txt"

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
    let groupIntersectingSets (sets: Set<int> list) =
        let adjacency = Dictionary<int, ResizeArray<int>>()
        for i in 0 .. sets.Length - 1 do
            adjacency[i] <- ResizeArray<int>()
        for i in 0 .. sets.Length - 1 do
            for j in i+1 .. sets.Length - 1 do
                if not (Set.isEmpty (Set.intersect sets[i] sets[j])) then
                    adjacency[i].Add(j)
                    adjacency[j].Add(i)

        // DFS to find connected components
        let visited = HashSet<int>()
        let rec dfs node (component: ResizeArray<int>) =
            visited.Add(node) |> ignore
            component.Add(node)
            for neighbor in adjacency.[node] do
                if not (visited.Contains(neighbor)) then
                    dfs neighbor component

        let groups = ResizeArray<Set<int>>()
        for i in 0 .. sets.Length - 1 do
            if not (visited.Contains(i)) then
                let component = ResizeArray<int>()
                dfs i component
                // Merge sets in this component
                let merged = component |> Seq.map (fun idx -> sets[idx]) |> Set.unionMany
                groups.Add(merged)
        groups |> Seq.toList


    let keys = scales |> Map.keys |> Seq.toArray
    let combinations = comb 3 (Array.toList keys)
    let families =
        combinations
        |> List.mapi(fun i comb ->
            //if i % 10000 = 0 then
            //    printfn "Family %i from %i." i (combinations.Length)
            let possibleFamily = scales |> Map.filter(fun k _ -> List.contains k comb)
            let family = buildFamily possibleFamily
            match family with
            | (Some c, Some p) -> 
                //printfn "Found Family at %i" i
                (true, c, p)
            | _ -> 
                (false, (0,[||]), Map.empty)
        )
        |> List.filter(fun (isFamily, _, _) -> isFamily)

    let familyIds =
        families
        |> List.map(fun f ->
            let (_, (cId, _), parents) = f
            [cId] @ (parents |> Map.keys |> Seq.toList) |> Set.ofList
        )
    let groupedFamilies = groupIntersectingSets familyIds
    groupedFamilies
    |> List.maxBy _.Count
    |> Seq.sum

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let scales = parseContent lines
    buildFamilies scales
