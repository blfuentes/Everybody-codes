module quest05_3_visualization

open System.IO
open EverybodyCodes_2026_FSharp.Modules
open quest05_utilities

let path = "quest05/quest05_input_03.txt"
let outputFileName = "quest05_3_visualize.bmp"

let private cellWidth = 8
let private cellHeight = 9

let private glyph = function
    | '0' ->
        [| "01110"
           "10001"
           "10011"
           "10101"
           "11001"
           "10001"
           "01110" |]
    | '#' ->
        [| "01010"
           "01010"
           "11111"
           "01010"
           "11111"
           "01010"
           "01010" |]
    | '=' ->
        [| "00000"
           "11111"
           "00000"
           "11111"
           "00000"
           "00000"
           "00000" |]
    | '@' ->
        [| "01110"
           "10001"
           "10111"
           "10101"
           "10111"
           "10000"
           "01110" |]
    | _ ->
        [| "00000"
           "00000"
           "00000"
           "00000"
           "00000"
           "00000"
           "00000" |]

let private colors = function
    | '0' -> (190uy, 105uy, 35uy), (255uy, 255uy, 255uy)
    | '#' -> (55uy, 125uy, 65uy), (255uy, 255uy, 255uy)
    | '=' -> (90uy, 190uy, 220uy), (40uy, 35uy, 25uy)
    | '@' -> (35uy, 85uy, 45uy), (255uy, 255uy, 255uy)
    | _ -> (0uy, 0uy, 0uy), (255uy, 255uy, 255uy)

let private writeBitmap (filePath: string) (map: char array array) =
    let rows = map.Length
    let cols = if rows = 0 then 0 else map[0].Length
    let width = cols * cellWidth
    let height = rows * cellHeight
    let rowSize = ((width * 3 + 3) / 4) * 4
    let imageSize = rowSize * height
    let fileSize = 54 + imageSize

    use stream = File.Create filePath
    use writer = new BinaryWriter(stream)

    writer.Write [| byte 'B'; byte 'M' |]
    writer.Write fileSize
    writer.Write 0
    writer.Write 54
    writer.Write 40
    writer.Write width
    writer.Write height
    writer.Write 1us
    writer.Write 24us
    writer.Write 0
    writer.Write imageSize
    writer.Write 2835
    writer.Write 2835
    writer.Write 0
    writer.Write 0

    let pixelRow = Array.zeroCreate<byte> rowSize

    for bitmapRow in height - 1 .. -1 .. 0 do
        System.Array.Clear(pixelRow, 0, pixelRow.Length)
        let mapRow = bitmapRow / cellHeight
        let glyphRow = bitmapRow % cellHeight - 1

        for col in 0 .. cols - 1 do
            let symbol = map[mapRow][col]
            let background, foreground = colors symbol
            let pattern = glyph symbol

            for x in 0 .. cellWidth - 1 do
                let glyphCol = x - 1
                let color =
                    if glyphRow >= 0 && glyphRow < pattern.Length
                       && glyphCol >= 0 && glyphCol < pattern[glyphRow].Length
                       && pattern[glyphRow][glyphCol] = '1' then
                        foreground
                    else
                        background

                let pixelOffset = col * cellWidth * 3 + x * 3
                let red, green, blue = color
                pixelRow[pixelOffset] <- blue
                pixelRow[pixelOffset + 1] <- green
                pixelRow[pixelOffset + 2] <- red

        writer.Write pixelRow

let execute() =
    let map = LocalHelper.GetContent path 2026 5 3 |> Array.map _.ToCharArray()
    let land = travelMap map
    let sand = quest05_3.reconstructSand land map
    let trees = quest05_3.addStampedTrees land sand map
    let trees = quest05_3.growForest land sand map trees

    let finalMap =
        map
        |> Array.mapi (fun row cells ->
            cells
            |> Array.mapi (fun col water ->
                if trees.Contains(row, col) then '@'
                elif sand.Contains(row, col) then '='
                elif land.Contains(row, col) then '#'
                else water
            )
        )

    let outputPath = Path.Combine(VisualizationFolder, outputFileName)
    writeBitmap outputPath finalMap
    outputPath
