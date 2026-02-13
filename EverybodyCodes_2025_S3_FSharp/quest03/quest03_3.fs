module quest03_3

open EverybodyCodes_2025_S3_FSharp.Modules
open System

let path = "quest03/test_input_03.txt"
// let path = "quest03/quest03_input_03.txt"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    lines.Length
