module quest03_2

open EverybodyCodes_2026_S4_FSharp.Modules
open System

//let path = "quest03/test_input_02.txt"
//let pathb = "quest03/test_input_02b.txt"
//let pathc = "quest03/test_input_02c.txt"
let path = "quest03/quest03_input_02.txt"

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

// two-colouring: crossing a stitched edge flips the colour, walking from (0,0)
let solve (path: string) =
    let (width, height, hOffsets, vOffsets) = parseContent <| ((ReadLines path) |> Seq.toArray)
    let mutable group0 = 0
    let mutable group1 = 0
    let mutable prevRow = Array.zeroCreate width
    let row = Array.zeroCreate width
    for y in 0 .. height - 1 do
        // colour of (0, y): from (0, y-1) crossing horizontal line y over column 0
        row[0] <- if y = 0 then 0
                  else (prevRow[0] + (if stitch hOffsets y 0 then 1 else 0)) % 2
        for x in 1 .. width - 1 do
            row[x] <- (row[x - 1] + (if stitch vOffsets x (y) then 1 else 0)) % 2
        for x in 0 .. width - 1 do
            if stitch hOffsets y x
               && stitch hOffsets (y + 1) x
               && stitch vOffsets x y
               && stitch vOffsets (x + 1) y then
                if row[x] = 0 then group0 <- group0 + 1 else group1 <- group1 + 1
        Array.blit row 0 prevRow 0 width
    //printfn "  groups: %d / %d (total %d)" group0 group1 (group0 + group1)
    max group0 group1

let execute() =
    //printfn "quest03 part2 %s: %d" pathb (solve pathb)
    //printfn "quest03 part2 %s: %d" pathc (solve pathc)
    solve path
