module quest06_1

open EverybodyCodes_2026_FSharp.Modules
open System
open System.Collections.Generic

//let path = "quest06/test_input_01.txt"
let path = "quest06/quest06_input_01.txt"

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

let reachableCountFrom (root: string) =
    let stack = Stack<string>()
    let visited = HashSet<string>()
    stack.Push(root)

    let mutable current = ""
    while stack.TryPop(&current) do
        if visited.Add(current) then
            let mutable children = Unchecked.defaultof<ResizeArray<string>>
            if ingredients.TryGetValue(current, &children) then
                for c in children do
                    stack.Push(c)

    visited.Count - 1

let execute() =
    let lines = LocalHelper.GetContent path 2026 6 1
    parseContent lines
    reachableCountFrom "NULL"
