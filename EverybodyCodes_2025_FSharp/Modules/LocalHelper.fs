namespace EverybodyCodes_2025_FSharp.Modules

open System.IO

[<AutoOpen>]
module LocalHelper =

    let VisualizationFolder = 
        if not (Directory.Exists(__SOURCE_DIRECTORY__ + @"../../visualizations/")) then
            Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"../../visualizations/") |> ignore
        __SOURCE_DIRECTORY__ + @"../../visualizations/"

    let GetLinesFromFile(path: string) =
        File.ReadAllLines(__SOURCE_DIRECTORY__ + @"../../" + path)

    let GetContentFromFile(path: string) =
        File.ReadAllText(__SOURCE_DIRECTORY__ + @"../../" + path)

    let ReadLines(path: string) =
        File.ReadLines(__SOURCE_DIRECTORY__ + @"../../" + path)

    ///////////////////////////////////////////////////////////////////////////////////
    /// Returns duration of the execution of the function
    let duration f = 
        let timer = new System.Diagnostics.Stopwatch()
        timer.Start()
        let returnValue = f()
        //printfn "Elapsed Time: %i" timer.ElapsedMilliseconds
        (returnValue, timer.ElapsedTicks)
    ///////////////////////////////////////////////////////////////////////////////////   