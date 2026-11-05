module quest08_2

open EverybodyCodes_2026_FSharp.Modules
open quest08_utilities

//let path = "quest08/test_input_02.txt"
let path = "quest08/quest08_input_02.txt"

let execute() =
    let field = LocalHelper.GetContent path 2026 8 2 |> parseField
    directions |> Array.sumBy (fun dir -> (litCells field (rays field dir)).Count)
