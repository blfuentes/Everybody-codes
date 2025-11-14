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

let buildFamilies(scales: Map<int, char array>) =
    let groupIntersectingSets (sets: Set<int> list) =
        if sets.IsEmpty then []
        else
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

    let dnaCache = scales |> Map.map (fun _ dna -> dna)
    
    // Helper to check if childId is a valid child of the two parent IDs
    let isValidFamily (childId: int) (parent1Id: int) (parent2Id: int) =
        let childDna = dnaCache[childId]
        let parent1Dna = dnaCache[parent1Id]
        let parent2Dna = dnaCache[parent2Id]
        
        let mutable isValid = true
        let mutable i = 0
        while isValid && i < childDna.Length do
            let c = childDna[i]
            if c <> parent1Dna[i] && c <> parent2Dna[i] then
                isValid <- false
            i <- i + 1
        isValid

    let keys = scales |> Map.keys |> Seq.toArray
    let keyCount = keys.Length
    
    let familySet = HashSet<Set<int>>()
    
    // faster than combinations...
    for i in 0 .. keyCount - 1 do
        for j in i + 1 .. keyCount - 1 do
            for k in j + 1 .. keyCount - 1 do
                let a = keys[i]
                let b = keys[j]
                let c = keys[k]
                
                let family = 
                    if isValidFamily a b c then
                        Some (Set.ofList [a; b; c])
                    elif isValidFamily b a c then
                        Some (Set.ofList [a; b; c])
                    elif isValidFamily c a b then
                        Some (Set.ofList [a; b; c])
                    else
                        None
                
                match family with
                | Some f -> familySet.Add(f) |> ignore
                | None -> ()

    let families = familySet |> Seq.toList
    let groupedFamilies = groupIntersectingSets families
    groupedFamilies
    |> List.maxBy _.Count
    |> Seq.sum

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let scales = parseContent lines
    buildFamilies scales
