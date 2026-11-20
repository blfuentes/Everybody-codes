module quest19_utilities

open System.IO

let private colorForSymbol = function
    | 'C' -> 0uy, 255uy, 255uy
    | 'M' -> 255uy, 0uy, 255uy
    | 'Y' -> 255uy, 255uy, 0uy
    | 'K' -> 0uy, 0uy, 0uy
    | 'B' -> 0uy, 0uy, 255uy
    | 'G' -> 0uy, 255uy, 0uy
    | 'R' -> 255uy, 0uy, 0uy
    | symbol -> failwithf "invalid crystal color: %c" symbol

let writeBitmapWithCellSize (cellSize: int) (filePath: string) (grid: char[,]) =
    if cellSize < 1 then
        invalidArg (nameof cellSize) "The cell size must be positive."

    let rows = grid.GetLength 0
    let columns = grid.GetLength 1

    if rows = 0 || columns = 0 then
        invalidArg (nameof grid) "The crystal grid must not be empty."

    let width = columns * cellSize
    let height = rows * cellSize
    let rowSize = ((width * 3 + 3) / 4) * 4
    let imageSize = rowSize * height
    let directory = Path.GetDirectoryName filePath

    if not (System.String.IsNullOrEmpty directory) then
        Directory.CreateDirectory directory |> ignore

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

    for bitmapRow in height - 1 .. -1 .. 0 do
        System.Array.Clear(pixelRow, 0, pixelRow.Length)
        let gridRow = bitmapRow / cellSize

        for column in 0 .. columns - 1 do
            let red, green, blue = colorForSymbol grid[gridRow, column]

            for x in 0 .. cellSize - 1 do
                let offset = (column * cellSize + x) * 3
                pixelRow[offset] <- blue
                pixelRow[offset + 1] <- green
                pixelRow[offset + 2] <- red

        writer.Write pixelRow

let writeBitmap filePath grid =
    writeBitmapWithCellSize 8 filePath grid

