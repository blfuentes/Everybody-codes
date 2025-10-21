module quest11_part01

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest11/test_input_01.txt"
let path = "quest11/quest11_input_01.txt"

let transformations = new Dictionary<string, string list>()

let parseContent(lines: string array) =
    lines
    |> Array.iter(fun line ->
        let origin, target = line.Split(":")[0], (line.Split(":")[1]).Split(",")
        transformations.Add(origin, target |> Array.toList)
    )

let rec transformation (current: int) (target: int) (status: string list) =
    if current = target then
        let result = String.concat "" status
        result.Length
    else
        let newStatus =
            status |> List.collect(fun s ->
                if transformations.ContainsKey(s) then
                    transformations.[s]
                else
                    [s]
            )
        transformation (current + 1) target newStatus
    

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent(lines)
    transformation 0 4 ["A"]
