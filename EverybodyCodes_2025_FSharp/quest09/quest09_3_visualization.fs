module quest09_part03_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading.Tasks
open System.Drawing
open System.Drawing.Imaging

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
            let adjacency = Dictionary<int, HashSet<int>>()
            for i in 0 .. sets.Length - 1 do
                adjacency[i] <- HashSet<int>()
            for i in 0 .. sets.Length - 1 do
                for j in i+1 .. sets.Length - 1 do
                    if not (Set.isEmpty (Set.intersect sets[i] sets[j])) then
                        adjacency[i].Add(j) |> ignore
                        adjacency[j].Add(i) |> ignore

            // DFS to find connected components
            let visited = HashSet<int>()
            let rec dfs node (component: ResizeArray<int>) =
                visited.Add(node) |> ignore
                component.Add(node)
                for neighbor in adjacency[node] do
                    if not (visited.Contains(neighbor)) then
                        dfs neighbor component

            let mutable groups = ResizeArray<Set<int>>()
            for i in 0 .. sets.Length - 1 do
                if not (visited.Contains(i)) then
                    let component = ResizeArray<int>()
                    dfs i component
                    let merged = component |> Seq.map (fun idx -> sets[idx]) |> Set.unionMany
                    groups.Add(merged)
            groups |> Seq.toList

    let keys = scales |> Map.keys |> Seq.toArray
    let keyCount = keys.Length
    
    let maxKey = keys |> Array.max
    let dnaArray = Array.create (maxKey + 1) [||]
    scales |> Map.iter (fun id dna -> dnaArray[id] <- dna)
    
    let isValidFamily (childId: int) (parent1Id: int) (parent2Id: int) =
        let childDna = dnaArray[childId]
        let parent1Dna = dnaArray[parent1Id]
        let parent2Dna = dnaArray[parent2Id]
        
        let mutable isValid = true
        let mutable i = 0
        while isValid && i < childDna.Length do
            let c = childDna[i]
            if c <> parent1Dna[i] && c <> parent2Dna[i] then
                isValid <- false
            i <- i + 1
        isValid

    let exportFamiliesToBitmap (groupedFamilies: Set<int> list) (allTriplets: Set<int> list) (filename: string) =
        let nodeRadius = 30
        let hSpacing = 100
        let vSpacing = 100
        let familySpacing = 200
        let font = new Font("Arial", 16.0f)
        let pen = new Pen(Color.LightGray, 3.0f)
        let brush = new SolidBrush(Color.Black)
        let textBrush = new SolidBrush(Color.White)

        // Layout each family in its own grid region
        let familyLayouts =
            groupedFamilies
            |> List.map (fun fam ->
                let allIds = fam |> Set.toList
                // Find parent nodes (nodes that are parents in any triplet in this family)
                let parentIds =
                    allTriplets
                    |> List.collect (fun triplet ->
                        let ids = triplet |> Seq.toList
                        match ids with
                        | [a; b; c] when fam.Contains(a) && fam.Contains(b) && fam.Contains(c) -> [b; c]
                        | [a; b; c] when fam.Contains(b) && fam.Contains(a) && fam.Contains(c) -> [a; c]
                        | [a; b; c] when fam.Contains(c) && fam.Contains(a) && fam.Contains(b) -> [a; b]
                        | _ -> []
                    )
                    |> Set.ofList
                let leafIds = allIds |> List.filter (fun id -> not (parentIds.Contains(id)))
                let nonLeafIds = allIds |> List.filter (fun id -> parentIds.Contains(id))
                let nodeIds = nonLeafIds @ leafIds // non-leaf nodes first, leaf nodes last
                let gridCols = int (ceil (sqrt (float nodeIds.Length)))
                let gridRows = int (ceil (float nodeIds.Length / float gridCols))
                let famWidth = gridCols * hSpacing + hSpacing
                let famHeight = gridRows * vSpacing + vSpacing
                let nodePositions =
                    nodeIds
                    |> List.mapi (fun idx id ->
                        let col = idx % gridCols
                        let row = idx / gridCols
                        let x = hSpacing + col * hSpacing
                        let y = vSpacing + row * vSpacing
                        (id, (x, y))
                    )
                    |> dict
                (nodePositions, famWidth, famHeight, fam)
            )

        // Calculate total image size
        let totalWidth =
            (familyLayouts
            |> List.map (fun (_, famWidth, _, _) -> famWidth)
            |> List.sum)
            + (familyLayouts.Length + 1) * familySpacing
        let maxHeight =
            (familyLayouts
            |> List.map (fun (_, _, famHeight, _) -> famHeight)
            |> List.max)
            + vSpacing

        use bmp = new Bitmap(totalWidth, maxHeight)
        use g = Graphics.FromImage(bmp)
        g.Clear(Color.FromArgb(30, 30, 30))

        // Draw each family group with extra space between
        let mutable xOffset = familySpacing
        for (nodePositions, famWidth, famHeight, fam) in familyLayouts do
            // Draw connections for all triplets in this family
            let famTriplets =
                allTriplets |> List.filter (fun triplet ->
                    let ids = triplet |> Seq.toList
                    ids |> List.forall (fun id -> fam.Contains(id))
                )
            for triplet in famTriplets do
                let ids = triplet |> Seq.toList
                match ids with
                | [a; b; c] ->
                    let positions = [a; b; c] |> List.choose (fun id ->
                        match nodePositions.TryGetValue(id) with
                        | true, pos -> Some pos
                        | _ -> None)
                    match positions with
                    | [pa; pb; pc] ->
                        g.DrawLine(pen, xOffset + fst pa, snd pa, xOffset + fst pb, snd pb)
                        g.DrawLine(pen, xOffset + fst pa, snd pa, xOffset + fst pc, snd pc)
                        g.DrawLine(pen, xOffset + fst pb, snd pb, xOffset + fst pc, snd pc)
                    | _ -> ()
                | _ -> ()
            // Draw nodes
            for KeyValue(id, (x, y)) in nodePositions do
                g.FillEllipse(brush, xOffset + x - nodeRadius, y - nodeRadius, nodeRadius * 2, nodeRadius * 2)
                g.DrawEllipse(pen, xOffset + x - nodeRadius, y - nodeRadius, nodeRadius * 2, nodeRadius * 2)
                let str = id.ToString()
                let sz = g.MeasureString(str, font)
                g.DrawString(str, font, textBrush, float32(xOffset + x) - float32(sz.Width) / 2.0f, float32(y) - float32(sz.Height) / 2.0f)
            xOffset <- xOffset + famWidth + familySpacing
        bmp.Save(filename, ImageFormat.Png)
    
    let familyBag = ConcurrentBag<Set<int>>()
    
    Parallel.For(0, keyCount, fun i ->
        for j in i + 1 .. keyCount - 1 do
            for k in j + 1 .. keyCount - 1 do
                let a = keys[i]
                let b = keys[j]
                let c = keys[k]
                
                if isValidFamily a b c || isValidFamily b a c || isValidFamily c a b then
                    familyBag.Add(Set.ofList [a; b; c])
    ) |> ignore

    let families = familyBag |> Seq.distinct |> Seq.toList
    let groupedFamilies = groupIntersectingSets families
    exportFamiliesToBitmap groupedFamilies families "quest09_part03_visualization.png"
    groupedFamilies
    |> List.maxBy _.Count
    |> Seq.sum

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let scales = parseContent lines
    buildFamilies scales