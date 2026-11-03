module quest06_3

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

//let path = "quest06/test_input_03.txt"
let path = "quest06/quest06_input_03.txt"

type Ingredient = {
    Id: string
    Composition: string list
}

let ingredients = Dictionary<string, Ingredient>()

let parseContent (lines: string array) =
    ingredients.Clear()

    let parseLine (line: string) =
        let parts = line.Split(':')
        let id = parts[0]
        let ings = parts[1].Split(',')
        (id, ings)

    lines
    |> Seq.iter (fun l ->
        let (id, ings) = parseLine l
        ings |> Seq.iter (fun i -> ingredients.TryAdd(i, { Id = i; Composition = [] }) |> ignore)

        if ingredients.ContainsKey(id) then
            ingredients[id] <- { ingredients[id] with Composition = List.concat [ ingredients[id].Composition; List.ofArray ings ] }
        else
            ingredients.Add(id, { Id = id; Composition = List.ofArray ings })
    )

let buildReverseGraph () =
    let reverse = Dictionary<string, ResizeArray<string>>()

    let ensureNode key =
        if not (reverse.ContainsKey key) then
            reverse[key] <- ResizeArray<string>()

    for kvp in ingredients do
        let parent = kvp.Key
        ensureNode parent

        for child in kvp.Value.Composition do
            ensureNode child
            reverse[child].Add(parent)

    reverse

let execute() =
    let lines = LocalHelper.GetContent path 2026 6 3
    parseContent lines

    let reverse = buildReverseGraph()
    let possibleNulls = ingredients.Keys |> Seq.filter (fun k -> k.StartsWith("NULLv")) |> Seq.toArray

    let reachableNullCount = Dictionary<string, int>()

    let reachMemo = Dictionary<string, HashSet<string>>()

    let getReachableFromNull (nullRoot: string) =
        match reachMemo.TryGetValue nullRoot with
        | true, cached -> cached
        | _ ->
            let visited = HashSet<string>()
            let stack = Stack<string>()
            stack.Push(nullRoot)

            let mutable current = ""
            while stack.TryPop(&current) do
                if visited.Add(current) then
                    if reverse.ContainsKey(current) then
                        reverse[current]
                        |> Seq.iter (fun parent ->
                            if not (visited.Contains(parent)) then
                                stack.Push(parent)
                        )

            reachMemo[nullRoot] <- visited
            visited

    for n in possibleNulls do
        let reached = getReachableFromNull n
        for node in reached do
            if reachableNullCount.ContainsKey(node) then
                reachableNullCount[node] <- reachableNullCount[node] + 1
            else
                reachableNullCount[node] <- 1

    reachableNullCount.Values
    |> Seq.filter (fun c -> c >= 2)
    |> Seq.length