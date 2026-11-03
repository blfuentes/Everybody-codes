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
    //let ingredients = Dictionary<string, Ingredient>()
    let parseLine (line: string) =
        (line.Split(':')[0], (line.Split(':')[1]).Split(','))
    lines
    |> Seq.iter(fun l ->
        let(id, ings) = parseLine l
        ings |> Seq.iter(fun i -> ingredients.TryAdd(i, { Id = i; Composition = []}) |> ignore)

        if ingredients.ContainsKey(id) then
            ingredients[id] <-  { ingredients[id] with Composition = (List.concat[ ingredients[id].Composition; ings |> List.ofArray]) }
        else
            ingredients.Add(id, { Id = id; Composition = (ings |> List.ofArray) })
    )


let compositionOf (root: string) =
    let composition = Stack<string>()
    composition.Push root
    let visited = HashSet<string>()
    
    let mutable current = ""
    while composition.TryPop(&current) do
        visited.Add current |> ignore
        ingredients[current].Composition
        |> List.iter(fun i ->
            if  not (visited.Contains i) then
                composition.Push i
        )
    visited        

let execute() =
    let lines = LocalHelper.GetContent path 2026 6 2
    parseContent lines
    //let composition = compositionOf "NULL"
    //composition.Count - 1
    let allcompo = 
        ingredients.Keys |> Seq.map compositionOf
    (allcompo |> Seq.filter _.Contains("NULL") |> Seq.length) - 1