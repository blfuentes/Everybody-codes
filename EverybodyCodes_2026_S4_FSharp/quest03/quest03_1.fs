module quest03_1

open EverybodyCodes_2026_S4_FSharp.Modules
open System.Text.RegularExpressions
open System.IO
open System

//let path = "quest03/test_input_01.txt"
let path = "quest03/quest03_input_01.txt"

let parseContent (lines: string array) =
    let value (key: string) : string =
        let line = lines |> Array.find (fun l -> l.StartsWith(key + "="))
        let parts = line.Split('=')
        parts[1].Trim()
    let width = int (value "width")
    let height = int (value "height")
    let hOffsets = (value "horizontal-offsets").ToCharArray() |> Array.map (fun c -> int c - int '0')
    let vOffsets = (value "vertical-offsets").ToCharArray() |> Array.map (fun c -> int c - int '0')
    (width, height, hOffsets, vOffsets)

let stitch (offsets: int array) (line: int) (index: int) =
    index % 2 = offsets[line % offsets.Length]

let execute() =
    let (width, height, hOffsets, vOffsets) = parseContent <| ((ReadLines path) |> Seq.toArray)
    seq {
        for y in 0 .. height - 1 do
            for x in 0 .. width - 1 do
                if stitch hOffsets y x
                   && stitch hOffsets (y + 1) x
                   && stitch vOffsets x y
                   && stitch vOffsets (x + 1) y then yield 1 }
    |> Seq.length
