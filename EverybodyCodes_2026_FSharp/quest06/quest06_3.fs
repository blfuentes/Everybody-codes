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

let compositionOf (root: string) =
    let composition = Stack<string>()
    composition.Push root
    let visited = HashSet<string>()

    let mutable current = ""
    while composition.TryPop(&current) do
        visited.Add current |> ignore
        ingredients[current].Composition
        |> List.iter (fun i ->
            if not (visited.Contains i) then
                composition.Push i
        )
    visited      

let execute() =
    let lines = LocalHelper.GetContent path 2026 6 3
    parseContent lines
    let allcompo = 
        ingredients.Keys |> Seq.map compositionOf
    let possibleNulls = ingredients.Keys |> Seq.filter _.StartsWith("NULLv")
    let pairs = seq {
        for n1 in possibleNulls do
            for n2 in possibleNulls do
                if n1 <> n2 then
                    yield (n1, n2)
    }
    
    allcompo
    |> Seq.filter(fun c ->
        pairs |> Seq.exists(fun (n1, n2) -> c.Contains n1 && c.Contains n2)
    ) |> Seq.length