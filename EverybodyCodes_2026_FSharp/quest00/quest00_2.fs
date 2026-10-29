module quest00_2

open EverybodyCodes_2026_FSharp.Modules

let path = "quest00/test_input_02.txt"
//let path = "quest00/quest00_input_02.txt"

let execute() =
    let lines = LocalHelper.GetContent path 2026 0 2
    0