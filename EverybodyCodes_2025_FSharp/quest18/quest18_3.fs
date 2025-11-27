module quest18_3

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Text.RegularExpressions

//let path = "quest18/test_input_03.txt"
let path = "quest18/quest18_input_03.txt"

type BranchDefinition = {
    Id: int Option;
    Thickness: bigint;
}

type PlantDefinition = {
    Id: int
    Thickness: bigint
    Branches: BranchDefinition[]
}

type TestCase = {
    Id: int;
    Brightness: Map<int, bigint>;
}

let plantDefinitions = new Dictionary<int, PlantDefinition>()
let branchThicknessCache = new Dictionary<int, int list>()
let parseContent (content: string) =
    let definitions = content.Split($"{System.Environment.NewLine}{System.Environment.NewLine}{System.Environment.NewLine}")
    let parts = definitions[0].Split($"{System.Environment.NewLine}{System.Environment.NewLine}")
    // regular expression to match all numbers in a string
    let numberRegex = Regex(@"-?\d+")
    parts
    |> Seq.iter(fun p ->

        let (header, body) = (
            p.Split($"{System.Environment.NewLine}")[0],
            (p.Split($"{System.Environment.NewLine}")[1..])
        )
        let id = numberRegex.Match(header).Value |> int
        let thickness = bigint.Parse(numberRegex.Matches(header).[1].Value)
        let branches =
            body
            |> Seq.map(fun b ->
                let matches = numberRegex.Matches(b)
                let branchId =
                    if matches.Count > 1 then
                        Some (matches[0].Value |> int)
                    else
                        None
                match branchId with
                | Some id ->
                    if not (branchThicknessCache.ContainsKey(id)) then
                        branchThicknessCache.Add(id, [])
                    branchThicknessCache[id] <- branchThicknessCache[id] @ [int matches[1].Value]
                | None -> ()
                let branchThickness =
                    if matches.Count > 1 then
                        bigint.Parse(matches[1].Value)
                    else
                        bigint.Parse(matches[0].Value)
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
    let testCases =
        definitions[1].Split($"{System.Environment.NewLine}{System.Environment.NewLine}")
        |> Seq.collect(fun t ->
            t.Split($"{System.Environment.NewLine}")
            |> Seq.mapi(fun idx line ->
                let brightnessMap =
                    line.Split(" ")
                    |> Seq.map(fun n -> bigint.Parse(n))
                    |> Seq.mapi(fun i v -> (i + 1, v))
                    |> Map.ofSeq
                {    
                    Id = idx + 1;
                    Brightness = brightnessMap
                }
            )
        )
    testCases

let calculateBrightness (testCase: TestCase) =
    let plantEnergies = Array.zeroCreate<bigint> (plantDefinitions.Count)
    for pIdx in 1..plantDefinitions.Count do
        let plant = plantDefinitions[pIdx]
        if testCase.Brightness.ContainsKey(plant.Id) && testCase.Brightness[plant.Id] = 0I then
            plantEnergies[pIdx-1] <- 0I
        else
            let mutable energy = 0I
            for branch in plant.Branches do
                match branch.Id with
                | Some bId ->
                    energy <- energy + (plantEnergies[bId-1] * branch.Thickness)
                | None ->
                    energy <- energy + 1I
            plantEnergies[pIdx-1] <- if plant.Thickness > energy then 0I else energy
    plantEnergies |> Array.last

let sumDiffs (bestCase: TestCase) (testCases: seq<TestCase>) =
    let maxBrightness = calculateBrightness(bestCase)
    testCases
    |> Seq.sumBy(fun tc ->
        let brightness = calculateBrightness(tc)
        if brightness > 0I then
            maxBrightness - brightness
        else
            0I
    )

// This solution doesn't work with test case :)
// Analyzing the input, you can see that the best case is to have all branches with thickness > 0
let findBestCase() =
    let keys = branchThicknessCache.Keys |> Seq.filter(fun id -> id <= 81) |> Seq.toArray
    let brightness =
        seq {
            for key in keys do
                if branchThicknessCache[key] |> List.forall(fun t -> t > 0) then
                    yield 1
                else
                    yield 0
        }
    { Id = -1; Brightness = brightness |> Seq.mapi(fun i v -> (i + 1, bigint v)) |> Map.ofSeq }

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let testCases = parseContent lines
    let bestCase = findBestCase()
    sumDiffs bestCase testCases
