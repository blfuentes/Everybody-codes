module quest07_2

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest07/test_input_02.txt"
let path = "quest07/quest07_input_02.txt"

let parseContent(lines: string array) =
    let names = lines[0].Split(",")

    let mapping =
        lines[2..]
        |> Array.map(fun line ->
            let init = line.Split(" > ")[0]
            let dest = (line.Split(" > ")[1]).Split(",")
            (init, dest)
        ) |> Map.ofArray
    (names, mapping)

let calculateValidNames(names: string array, mapping: Map<string, string array>) =
    let isValid(name: string) =
        name.ToCharArray()
        |> Array.map string
        |> Array.pairwise
        |> Array.forall(fun (o, d) ->
            Array.contains d mapping[o]
        )
    names 
    |> Array.mapi(fun index name -> (index+1, isValid name))
    |> Array.filter(fun (_, valid) -> valid)
    |> Array.sumBy(fun (index, _) -> index)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (names, mapping) = parseContent(lines)
    calculateValidNames(names, mapping)    
