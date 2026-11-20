module quest19_3_visualization

open System.IO
open EverybodyCodes_2026_FSharp.Modules
open quest19_utilities

let path = "quest19/quest19_input_03.txt"
let outputFileName = "quest19_3_visualization.bmp"

let render (lines: string[]) outputPath =
    let inputLayer = quest19_3.parseContent lines
    let score = quest19_3.findMaxCrystalCubes inputLayer
    writeBitmap outputPath inputLayer
    score

let execute () =
    let lines = LocalHelper.GetContent path 2026 19 3
    render lines (Path.Combine(VisualizationFolder, outputFileName))
