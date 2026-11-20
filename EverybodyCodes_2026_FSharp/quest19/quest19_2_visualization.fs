module quest19_2_visualization

open System.IO
open EverybodyCodes_2026_FSharp.Modules
open quest19_utilities

let path = "quest19/quest19_input_02.txt"
let outputFileName = "quest19_2_visualization.bmp"

let render (lines: string[]) (outputPath: string) =
    let firstLayer, secondLayer = quest19_2.parseContent lines
    let mixedLayer, score = quest19_2.findBestMixedLayer firstLayer secondLayer
    writeBitmapWithCellSize 1 outputPath mixedLayer
    score

let execute () =
    let lines = LocalHelper.GetContent path 2026 19 2
    render lines (Path.Combine(VisualizationFolder, outputFileName))
