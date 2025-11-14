module quest00_1

open EverybodyCodes_2024_FSharp.Modules

let path = "quest00/test_input_01.txt"
//let path = "quest00/quest00_input_01.txt"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    0