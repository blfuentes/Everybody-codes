module quest19_1_visualization

open System.IO
open EverybodyCodes_2026_FSharp.Modules
open quest19_utilities

let path = "quest19/quest19_input_01.txt"
let outputFileName = "quest19_1_visualization.bmp"

let render (lines: string[]) outputPath =
    let firstLayer, secondLayer = quest19_1.parseContent lines
    let mixedLayer, score = quest19_1.mixLayerWithScore firstLayer secondLayer
    writeBitmap outputPath mixedLayer
    score

let execute () =
    let lines = LocalHelper.GetContent path 2026 19 1
    render lines (Path.Combine(VisualizationFolder, outputFileName))
