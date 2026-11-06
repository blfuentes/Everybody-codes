module quest09_2

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

// let path = "quest09/test_input_02.txt"
let path = "quest09/quest09_input_02.txt"

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
    let rec grow seconds leaves branches =
        if leaves >= plant.Limit then
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

            grow (seconds + 1) (leaves + splits) nextBranches

    grow 1 1 [1]

let execute() =
    let lines = LocalHelper.GetContent path 2026 9 2
    let plants = parseContent lines
    plants |> Seq.sumBy growPlant
