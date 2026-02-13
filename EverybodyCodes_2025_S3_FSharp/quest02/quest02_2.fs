module quest02_2

open System.Numerics
open EverybodyCodes_2025_S3_FSharp.Modules

//let path = "quest02/test_input_02.txt"
let path = "quest02/quest02_input_02.txt"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)[0]
    lines.Length