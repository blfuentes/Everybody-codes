module quest08_1

open EverybodyCodes_2026_FSharp.Modules
open quest08_utilities

//let path = "quest08/test_input_01.txt"
let path = "quest08/quest08_input_01.txt"

let execute() =
    let field = LocalHelper.GetContent path 2026 8 1 |> parseField
    (litCells field (rays field directions.[0])).Count
