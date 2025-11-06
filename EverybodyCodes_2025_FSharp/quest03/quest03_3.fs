module quest03_part03

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest03/test_input_03.txt"
let path = "quest03/quest03_input_03.txt"

let parseContent (lines: string array) =
    lines[0].Split(",") |> Array.map int

let buildPacks (crates: int array) =
    let rec addCrate(remaining: int list) (packs: ResizeArray<int list>) =
        match remaining with
        | [] -> packs
        | crate :: rem ->
            if packs.Count = 0 then
                packs.Add([crate]) |> ignore
            else
                let mutable doContinue = true
                let mutable idx = 0
                while doContinue do                    
                    if packs[idx] |> List.last > crate then
                        packs[idx] <- packs[idx] @ [crate]
                        doContinue <- false
                    else
                        if idx = packs.Count - 1 then
                            packs.Add([crate]) |> ignore
                            doContinue <- false
                        else
                            idx <- idx + 1
            addCrate rem packs            
    let packs = ResizeArray<int list>()
    let packaging = addCrate (crates |> Array.sortDescending |> List.ofArray) (packs)
    packaging.Count

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let crates = parseContent lines
    buildPacks crates
