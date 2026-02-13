module quest01_2

open EverybodyCodes_2025_S3_FSharp.Modules

let path = "quest01/test_input_02.txt"
// let path = "quest01/quest01_input_02.txt"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    lines.Length
