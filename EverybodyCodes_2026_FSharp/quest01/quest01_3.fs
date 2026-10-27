module quest01_3

open EverybodyCodes_2026_FSharp.Modules
open System

//let path = "quest01/test_input_03.txt"
let path = "quest01/quest01_input_03.txt"

let additionalPines (pines: int) =
    let size = max 3 (int(Math.Ceiling(Math.Sqrt(float pines + 1.))))
    let oddSize = if size % 2 = 0 then size + 1 else size
    (oddSize * oddSize - 1 - pines)

let execute() =
    let content = LocalHelper.GetContentFromFile(path)
    content.Split(',')
    |> Array.sumBy (fun value -> additionalPines (int value))
