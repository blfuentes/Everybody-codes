module quest02_1

open EverybodyCodes_2026_FSharp.Modules

//let path = "quest02/test_input_01.txt"
let path = "quest02/quest02_input_01.txt"

let parseContent(lines: string seq) =
    lines
    |> Seq.map(fun line ->
        let pairs = line.Split(' ')
        pairs
        |> Seq.map(fun pair ->
            match pair[1] with
            | '>' -> (pair[2], pair[0])
            | '<' -> (pair[0], pair[2])
            | _ -> failwith "incorrect comparison"
        )
    )

let sortPairs (pairs: seq<char * char>) : string =
    let indegree =
        pairs
        |> Seq.countBy snd
        |> Map.ofSeq

    ['1' .. '5']
    |> List.sortByDescending (fun n -> Map.tryFind n indegree |> Option.defaultValue 0)
    |> List.map string
    |> String.concat ""

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let pairs = parseContent lines
    pairs |> Seq.sumBy (sortPairs >> int)
