module quest05_3_visualization

open System.IO
open EverybodyCodes_2026_FSharp.Modules
open quest05_utilities

let path = "quest05/quest05_input_03.txt"
let outputFileName = "quest05_3_visualize.bmp"

let private cellWidth = 8
let private cellHeight = 9

let private colors = function
    | '0' -> (45uy, 140uy, 210uy)
    | '#' -> (139uy, 90uy, 43uy)
    | '=' -> (240uy, 210uy, 70uy)
    | '@' -> (45uy, 130uy, 55uy)
    | _ -> (0uy, 0uy, 0uy)

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

        for col in 0 .. cols - 1 do
            let symbol = map[mapRow][col]
            let red, green, blue = colors symbol

            for x in 0 .. cellWidth - 1 do
                let pixelOffset = col * cellWidth * 3 + x * 3
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
            |> Array.mapi (fun col _ ->
                if trees.Contains(row, col) then '@'
                elif sand.Contains(row, col) then '='
                elif land.Contains(row, col) then '#'
                else '0'
            )
        )

    let outputPath = Path.Combine(VisualizationFolder, outputFileName)
    writeBitmap outputPath finalMap
    outputPath
