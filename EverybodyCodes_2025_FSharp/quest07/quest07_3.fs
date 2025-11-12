module quest07_part03

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest07/test_input_03.txt"
let path = "quest07/quest07_input_03.txt"

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

let buildTails (from: string) (mapping: Map<string, string array>) (maxSize: int) : string array =
    let rec build (current: string) (acc: string list) =
        if acc.Length = maxSize then
            acc |> List.toArray
        else
            match mapping.TryFind(current) with
            | Some nexts ->
                nexts
                |> Array.collect(fun next -> build next (acc @ [next]))
            | None -> acc |> List.toArray
    build from []

let calculateValidNames(names: string array, mapping: Map<string, string array>) =
    let minLen = 7
    let maxLen = 11
    let isValid(name: string) =
        name.ToCharArray()
        |> Array.map string
        |> Array.pairwise
        |> Array.forall(fun (o, d) ->
            match mapping.TryFind(o) with
            | Some arr -> Array.contains d arr
            | None -> false)

    let rec buildNames (current: string) : string list =
        if current.Length >= maxLen then 
            [current]
        else
            let lastChar = current.[current.Length - 1].ToString()
            match mapping.TryFind(lastChar) with
            | Some nexts ->
                nexts
                |> Array.toList
                |> List.collect (fun next -> [current] @ buildNames (current + next))
            | None -> [current]

    names
    |> Array.filter isValid
    |> Array.toList
    |> List.collect buildNames
    |> List.filter (fun n -> n.Length >= minLen && n.Length <= maxLen)
    |> List.distinct
    |> List.length

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (names, mapping) = parseContent(lines)
    calculateValidNames(names, mapping)  
