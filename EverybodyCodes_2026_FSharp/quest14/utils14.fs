module quest14_utilities

open System.IO

let private cellSize = 8

let private colorForSymbol (symbol: char) =
    let value = int symbol
    let mutable red = 0
    let mutable green = 0
    let mutable blue = 0

    // Spread the character bits across the most significant RGB bits first.
    for bit in 0 .. 15 do
        let componentBit = 7 - bit / 3
        let componentValue = ((value >>> bit) &&& 1) <<< componentBit
        match bit % 3 with
        | 0 -> red <- red ||| componentValue
        | 1 -> green <- green ||| componentValue
        | _ -> blue <- blue ||| componentValue

    byte red, byte green, byte blue

let writeBitmap (filePath: string) (grid: string array) =
    if grid.Length = 0 || grid[0].Length = 0 then
        invalidArg "grid" "The grid must not be empty."
    if grid |> Array.exists (fun row -> row.Length <> grid[0].Length) then
        invalidArg "grid" "The grid must be rectangular."

    let width = grid[0].Length * cellSize
    let height = grid.Length * cellSize
    let rowSize = ((width * 3 + 3) / 4) * 4
    let imageSize = rowSize * height

    use stream = File.Create filePath
    use writer = new BinaryWriter(stream)
    writer.Write [| byte 'B'; byte 'M' |]
    writer.Write (54 + imageSize)
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
    for row in grid.Length - 1 .. -1 .. 0 do
        for column in 0 .. grid[row].Length - 1 do
            let red, green, blue = colorForSymbol (grid[row][column])
            for x in 0 .. cellSize - 1 do
                let offset = (column * cellSize + x) * 3
                pixelRow[offset] <- blue
                pixelRow[offset + 1] <- green
                pixelRow[offset + 2] <- red
        for _ in 1 .. cellSize do
            writer.Write pixelRow