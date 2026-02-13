module quest01_3

open EverybodyCodes_2025_S3_FSharp.Modules
open System.Collections.Generic

//let path = "quest01/test_input_03.txt"
let path = "quest01/quest01_input_03.txt"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    lines.Length
