module quest02_1

open EverybodyCodes_2025_S3_FSharp.Modules

let path = "quest02/test_input_01.txt"
// let path = "quest02/quest02_input_01.txt"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)[0]
    lines.Length