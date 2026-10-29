module quest02_3

open EverybodyCodes_2026_FSharp.Modules

//let path = "quest02/test_input_03.txt"
let path = "quest02/quest02_input_03.txt"

let parseContent(lines: string seq) =
    lines
    |> Seq.map(fun line ->
        let pairs = line.Split(' ')
        pairs
        |> Seq.map(fun pair ->
            match pair[1] with
            | '>' -> (pair[2], pair[0])
            | '<' -> (pair[0], pair[2])
            | _ -> failwith "incorrect comparison"
        )
    )

let sortPairs(pairs: seq<char*char>) : string =
    let sources = pairs |> Seq.map fst |> Set
    let targets = pairs |> Seq.map snd |> Set
    let nodes =  Set.union sources targets
    let adj0 =
        nodes |> Seq.map (fun n -> n, Set.empty<char>) |> Map.ofSeq

    let indegree0 =
        nodes |> Seq.map (fun n -> n, 0) |> Map.ofSeq

    let adj, indegree =
        ((adj0, indegree0), pairs |> List.ofSeq)
        ||> List.fold (fun (adj, indegree) (u, v) ->
            let currentNeighbors = Map.find u adj

            if Set.contains v currentNeighbors then
                (adj, indegree)
            else
                let adj' = Map.add u (Set.add v currentNeighbors) adj
                let indegree' = Map.add v (Map.find v indegree + 1) indegree
                (adj', indegree')
        )

    // Kahn’s algorithm
    // https://en.wikipedia.org/wiki/Topological_sorting#Kahn's_algorithm
    // Step 1: initialize ready set, all nodes with indegree to 0
    let ready0 =
        indegree
        |> Map.toSeq
        |> Seq.choose (fun (n, d) -> if d = 0 then Some n else None)
        |> Set.ofSeq

    // Step 2: process until no ready nodes remain
    let rec loop (ready: Set<char>) (indeg: Map<char, int>) (outputRev: char list) =
        if Set.isEmpty ready then
            outputRev, indeg
        else
            // deterministic choice: smallest ready element
            let u = Set.minElement ready
            let ready' = Set.remove u ready

            // Step 3: remove u by decrementing indegree of each outgoing neighbor
            let neighbors = Map.find u adj
            let indeg', ready'' =
                ((indeg, ready'), neighbors)
                ||> Set.fold (fun (ind, r) v ->
                    let d' = (Map.find v ind) - 1
                    let ind' = Map.add v d' ind
                    let r' = if d' = 0 then Set.add v r else r
                    (ind', r')
                )

            loop ready'' indeg' (u :: outputRev)

    let ordered, _ = loop ready0 indegree []

    // Step 4: cycle check
    if List.length ordered <> Map.count adj then
        failwith "Cycle detected!"

    ordered |> Seq.map string |> String.concat ""

let execute() =
    let lines = LocalHelper.GetContent path 2026 2 3
    let pairs = parseContent lines
    pairs
    |> Seq.toArray
    |> Array.Parallel.map (sortPairs >> int64)
    |> Array.sum
