module quest01_2

open EverybodyCodes_2026_FSharp.Modules

//let path = "quest01/test_input_02.txt"
let path = "quest01/quest01_input_02.txt"

let additionalPines (pines: int) =
    if pines < 8 then
        8 - pines
    else
        (4 - pines % 4) % 4

let execute() =
    let content = (LocalHelper.GetContent path 2026 1 2)[0]
    content.Split(',')
    |> Array.sumBy (fun value -> additionalPines (int value))
