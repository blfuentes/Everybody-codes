module quest18_1

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest18/test_input_01.txt"
let path = "quest18/quest18_input_01.txt"

type BranchDefinition = {
    Id: int Option;
    Thickness: int;
}

type PlantDefinition = {
    Id: int
    Thickness: int
    Branches: BranchDefinition[]
}

let plantDefinitions = new Dictionary<int, PlantDefinition>()

let parseContent (content: string) =
    let parts = content.Split($"{System.Environment.NewLine}{System.Environment.NewLine}")
    // regular expression to match all numbers in a string
    let numberRegex = System.Text.RegularExpressions.Regex(@"\d+")
    parts
    |> Seq.iter(fun p ->

        let (header, body) = (
            p.Split($"{System.Environment.NewLine}")[0],
            (p.Split($"{System.Environment.NewLine}")[1..])
        )
        let id = numberRegex.Match(header).Value |> int
        let thickness = numberRegex.Matches(header).[1].Value |> int
        let branches =
            body
            |> Seq.map(fun b ->
                let matches = numberRegex.Matches(b)
                let branchId =
                    if matches.Count > 1 then
                        Some (matches[0].Value |> int)
                    else
                        None
                let branchThickness =
                    if matches.Count > 1 then
                        matches[1].Value |> int
                    else
                        matches[0].Value |> int
                {
                    Id = branchId
                    Thickness = branchThickness
                }
            )
            |> Seq.toArray
        plantDefinitions.Add(id, {
            Id = id
            Thickness = thickness
            Branches = branches
        })
    )

let calculateBrightness () =
    let plantEnergies = Array.zeroCreate<int> (plantDefinitions.Count)
    for pIdx in 1..plantDefinitions.Count do
        let plant = plantDefinitions[pIdx]
        let mutable energy = 0
        for branch in plant.Branches do
            match branch.Id with
            | Some bId ->
                energy <- energy + (plantEnergies[bId-1] * branch.Thickness)
            | None ->
                energy <- energy + 1
        plantEnergies[pIdx-1] <- if plant.Thickness > energy then 0 else energy
    plantEnergies |> Array.last

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    parseContent lines
    calculateBrightness()
