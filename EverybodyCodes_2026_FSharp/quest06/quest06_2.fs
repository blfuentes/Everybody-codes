module quest06_2

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

//let path = "quest06/test_input_02.txt"
let path = "quest06/quest06_input_02.txt"

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