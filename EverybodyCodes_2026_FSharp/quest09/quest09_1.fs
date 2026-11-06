module quest09_1

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

// let path = "quest09/test_input_01.txt"
let path = "quest09/quest09_input_01.txt"

type PlantDef = {
    Id: int
    Growth: int
    Limit: int
}

let parseContent (lines: string array) : PlantDef seq =
    lines |> Seq.map(fun line -> 
        let numbers = Regex.Matches(line, @"\d+")
        { 
            Id = int(numbers[0].Value);
            Growth = int(numbers[1].Value);
            Limit = int(numbers[2].Value)
        }
    )

let growPlant (plant: PlantDef) =
    let halfGrowth = (plant.Growth - 1) / 2
    let rec grow seconds size branches =
        if size >= plant.Limit then
            seconds
        else
            let nextBranches, splits =
                List.foldBack
                    (fun length (nextBranches, splitCount) ->
                        let grownLength = length + 1
                        if grownLength = plant.Growth then
                            [halfGrowth; halfGrowth; 1] @ nextBranches, splitCount + 1
                        else
                            grownLength :: nextBranches, splitCount)
                    branches
                    ([], 0)

            grow (seconds + 1) (size + List.length branches + splits) nextBranches

    grow 1 1 [1]

let execute() =
    let lines = LocalHelper.GetContent path 2026 9 1
    let plants = parseContent lines
    plants |> Seq.sumBy growPlant
