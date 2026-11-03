module quest06_2

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open System

//let path = "quest06/test_input_02.txt"
let path = "quest06/quest06_input_02.txt"

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
    let lines = LocalHelper.GetContent path 2026 6 2
    parseContent lines

    let reverse = buildReverseGraph()

    let reachableFromNull = HashSet<string>()
    let stack = Stack<string>()
    stack.Push("NULL")

    let mutable current = ""
    while stack.TryPop(&current) do
        if reachableFromNull.Add(current) then
            if reverse.ContainsKey(current) then
                reverse[current]
                |> Seq.iter (fun parent ->
                    if not (reachableFromNull.Contains(parent)) then
                        stack.Push(parent)
                )

    reachableFromNull.Remove("NULL") |> ignore
    reachableFromNull.Count