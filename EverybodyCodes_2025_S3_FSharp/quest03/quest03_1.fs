module quest03_1

open EverybodyCodes_2025_S3_FSharp.Modules
open System.Text.RegularExpressions
open System.IO
open System

let path = "quest03/test_input_01.txt"
// let path = "quest03/quest03_input_01.txt"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    lines.Length
