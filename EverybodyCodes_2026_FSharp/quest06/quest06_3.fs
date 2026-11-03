module quest06_3

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open System

//let path = "quest06/test_input_03.txt"
let path = "quest06/quest06_input_03.txt"

let ingredients = Dictionary<string, ResizeArray<string>>()

let ensureNode (id: string) =
    if not (ingredients.ContainsKey id) then
        ingredients[id] <- ResizeArray<string>()

let parseContent (lines: string array) =
    ingredients.Clear()

    for line in lines do
        let sep = line.IndexOf(':')
        let id = line.Substring(0, sep)
        let children = line.Substring(sep + 1).Split(',', StringSplitOptions.RemoveEmptyEntries)

        ensureNode id

        for child in children do
            ensureNode child
            ingredients[id].Add(child)

let buildReverseGraph () =
    let reverse = Dictionary<string, ResizeArray<string>>()

    let ensureNode key =
        if not (reverse.ContainsKey key) then
            reverse[key] <- ResizeArray<string>()

    for kvp in ingredients do
        let parent = kvp.Key
        ensureNode parent

        for child in kvp.Value do
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