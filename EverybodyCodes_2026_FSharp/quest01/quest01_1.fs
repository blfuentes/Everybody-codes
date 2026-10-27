module quest01_1

open EverybodyCodes_2026_FSharp.Modules
open System

//let path = "quest01/test_input_01.txt"
let path = "quest01/quest01_input_01.txt"

let execute() =
    let content = LocalHelper.GetContentFromFile(path)
    let remaining = content.Split(',') |> Seq.map float
    remaining
    |> Seq.sumBy(fun r ->
        let cando = Math.Sqrt(r)
        let withr = Math.Pow(int(cando), 2)
        if withr < r then
            let next = int(cando) + 1
            int(Math.Pow(next, 2)) - int(r)
        else 
            int(0)
    )
