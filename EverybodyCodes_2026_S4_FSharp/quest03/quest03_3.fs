module quest03_3

open EverybodyCodes_2026_S4_FSharp.Modules
open System

//let path = "quest03/test_input_03.txt"
let path = "quest03/quest03_input_03.txt"

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

// Main idea for huge input:
// The stitch behaviour repeats (is periodic), so we do not scan every tile.
// We only scan one period in X and one period in Y, then multiply by
// how many real tiles map to each (rx, ry) class.
let solve (path: string) =
    let (width, height, hOffsets, vOffsets) = parseContent <| ((ReadLines path) |> Seq.toArray)

    // Period size of all stitch conditions.
    // x parity + vertical offset cycle => 2 * |vOffsets|
    // y parity + horizontal offset cycle => 2 * |hOffsets|
    let px = 2 * vOffsets.Length
    let py = 2 * hOffsets.Length
    //printfn "  floor %d x %d -> periods %d x %d (%d classes)" width height px py (px * py)

    // aPar[y]: colour flip parity contributed by moving vertically down to row y.
    // Crossing a stitched boundary flips colour; non-stitched does not.
    let aPar = Array.zeroCreate py
    for y in 1 .. py - 1 do
        aPar[y] <- (aPar[y - 1] + (if hOffsets[y % hOffsets.Length] = 0 then 1 else 0)) % 2

    // bPar[c][x]: colour flip parity from moving horizontally to column x,
    // for row parity c (because vertical stitches depend on y parity).
    let bPar = Array.init 2 (fun c ->
        let acc = Array.zeroCreate px
        for x in 1 .. px - 1 do
            acc[x] <- (acc[x - 1] + (if vOffsets[x % vOffsets.Length] = c then 1 else 0)) % 2
        acc)

    // countX[rx] = number of real columns with x % px = rx.
    // countY[ry] = number of real rows with y % py = ry.
    // This is how we scale one periodic class to the full huge floor.
    let multiplicities (size: int) (period: int) =
        Array.init period (fun r -> if r >= size then 0L else int64 ((size - r - 1) / period) + 1L)
    let countX = multiplicities width px
    let countY = multiplicities height py

    let mutable group0 = 0L
    let mutable group1 = 0L

    // Iterate all residue classes (rx, ry) instead of all real tiles.
    for ry in 0 .. py - 1 do
        if countY[ry] > 0L then
            for rx in 0 .. px - 1 do
                // Tile is isolated if all four sides have stitches.
                if countX[rx] > 0L
                   && stitch hOffsets ry rx
                   && stitch hOffsets (ry + 1) rx
                   && stitch vOffsets rx ry
                   && stitch vOffsets (rx + 1) ry then
                    // Region colour is binary (0/1). We combine vertical and horizontal parity.
                    let colour = (aPar[ry] + bPar[ry % 2][rx]) % 2

                    // Number of real tiles represented by this residue class.
                    let amount = countX[rx] * countY[ry]
                    if colour = 0 then group0 <- group0 + amount else group1 <- group1 + amount
    //    if (ry + 1) % 20 = 0 || ry = py - 1 then
    //        printfn "  row classes %d/%d - groups so far: %d / %d" (ry + 1) py group0 group1
    //printfn "  groups: %d / %d (total %d)" group0 group1 (group0 + group1)
    // Part 2/3 asks for the larger colour group among isolated tiles.
    max group0 group1

let execute() =
    solve path
