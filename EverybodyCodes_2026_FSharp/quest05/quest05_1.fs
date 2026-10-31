module quest05_1

open EverybodyCodes_2026_FSharp.Modules
open quest05_utilities
//let path = "quest05/test_input_01.txt"
let path = "quest05/quest05_input_01.txt"

let execute() =
    let map = LocalHelper.GetContent path 2026 5 1 |> Array.map _.ToCharArray()
    travelMap map |> fun land -> land.Count
