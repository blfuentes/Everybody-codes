module quest01_2

open EverybodyCodes_2025_S3_FSharp.Modules

//let path = "quest01/test_input_02.txt"
let path = "quest01/quest01_input_02.txt"

let parseContent(lines: string seq) =
    let inline convertToNum(value: string) =
        value |> Seq.fold (fun acc v -> (acc <<< 1) ||| (if v > 'Z' then 0 else 1)) 0

    lines
    |> Seq.toArray
    |> Array.map(fun l ->
        let parts = l.Split(':')
        let dnas  = parts.[1].Split(' ')
        (parts.[0] |> int),
        convertToNum dnas.[0] + convertToNum dnas.[1] + convertToNum dnas.[2],
        convertToNum dnas.[3]
    )


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let dnas = parseContent lines
    let (a, _, _) =
        dnas
        |> Array.groupBy(fun (_, _, s) -> s)
        |> Array.maxBy fst
        |> snd
        |> Array.minBy(fun (_, v, _) -> v)
    a

    