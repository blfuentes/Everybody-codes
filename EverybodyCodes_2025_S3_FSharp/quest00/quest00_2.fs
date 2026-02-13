module quest00_2

open EverybodyCodes_2025_S3_FSharp.Modules

let path = "quest00/test_input_02.txt"
//let path = "quest00/quest00_input_02.txt"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    0