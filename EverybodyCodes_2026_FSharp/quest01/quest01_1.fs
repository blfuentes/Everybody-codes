module quest01_1

open EverybodyCodes_2026_FSharp.Modules
open System

//let path = "quest01/test_input_01.txt"
let path = "quest01/quest01_input_01.txt"

let additionalPines (pines: int) =
    let size = int (Math.Ceiling(Math.Sqrt(pines)))
    int (size * size - int pines)

let execute() =
    let content = (LocalHelper.GetContent path 2026 1 1)[0]
    content.Split(',')
    |> Array.sumBy (fun value -> additionalPines (int value))
