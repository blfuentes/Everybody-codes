module quest01_1

open EverybodyCodes_2025_S3_FSharp.Modules

let path = "quest01/test_input_01.txt"
// let path = "quest01/quest01_input_01.txt"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    lines.Length
