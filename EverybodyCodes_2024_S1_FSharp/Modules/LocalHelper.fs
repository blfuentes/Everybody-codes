namespace EverybodyCodes_2024_S1_FSharp.Modules

open System.IO
open EverybodyCodes_Utils

[<AutoOpen>]
module LocalHelper =

    let getLines (text: string) : string list =
        use sr = new StringReader(text)
        let rec loop acc =
            match sr.ReadLine() with
            | null -> List.rev acc
            | line -> loop (line :: acc)
        loop []

    let GetContent (path: string) (eventStory: int) (quest: int) (part: int) =
        let sourcepath = __SOURCE_DIRECTORY__ + @"../../" + path
        let (retrieveContent, content) =
            if File.Exists sourcepath then
                let content = File.ReadAllText sourcepath
                if content.Length = 0 then
                    (true, content)
                else
                    (false, content)
            else
                (true, "")
        let content =
            if retrieveContent then
                let c = EverybodyCodes_Utils.retrieveContent eventStory quest part
                File.WriteAllText(sourcepath, c)
                c
            else
                content
        getLines content |> Array.ofList

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