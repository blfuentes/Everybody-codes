module quest07_1

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest07/test_input_01.txt"
let path = "quest07/quest07_input_01.txt"

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

let findValidNames(names: string array, mapping: Map<string, string array>) =
    let isValid(name: string) =
        name.ToCharArray()
        |> Array.map string
        |> Array.pairwise
        |> Array.forall(fun (o, d) ->
            Array.contains d mapping[o]
        )
    names |> Array.filter isValid

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (names, mapping) = parseContent(lines)
    let validNames = findValidNames(names, mapping)
    validNames
