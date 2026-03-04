module quest01_1

open EverybodyCodes_2025_S3_FSharp.Modules

//let path = "quest01/test_input_01.txt"
let path = "quest01/quest01_input_01.txt"

let parseContent(lines: string seq) =
    let inline convertToNum(value: string) =
        value |> Seq.fold (fun acc v -> (acc <<< 1) ||| (if v > 'Z' then 0 else 1)) 0
        
    lines
    |> Seq.toArray
    |> Array.map(fun l ->
        let parts = l.Split(':')
        let dnas  = parts.[1].Split(' ')
        (parts.[0] |> int),
        convertToNum dnas.[0],
        convertToNum dnas.[1],
        convertToNum dnas.[2]
    )


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let dnas = parseContent lines
    dnas
    |> Array.sumBy(fun (a,b,c,d) ->
        if c > b && c > d then a else 0
    )
