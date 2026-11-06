module quest09_3

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

// let path = "quest09/test_input_03.txt"
let path = "quest09/quest09_input_03.txt"

type PlantDef = {
    Id: int
    Growth: int64
    Limit: int64
}

let parseContent (lines: string array) : PlantDef seq =
    lines |> Seq.map(fun line -> 
        let numbers = Regex.Matches(line, @"\d+")
        { 
            Id = int(numbers[0].Value);
            Growth = int64(numbers[1].Value);
            Limit = int64(numbers[2].Value)
        }
    )

let growPlant (plant: PlantDef) =
    let shortDelay = (plant.Growth + 1L) / 2L
    let longDelay = plant.Growth - 1L

    let reachesLimit seconds =
        let remaining = seconds - plant.Growth

        // A split produces two branches with the short delay and one with the long delay
        // Paths with i short and j long delays occur 2^i * binomial(i+j, i) times
        let rec countShort i powerOfTwo leaves =
            if leaves >= plant.Limit then true
            elif i * shortDelay > remaining then false
            else
                let rec countLong j paths total =
                    if total >= plant.Limit || i * shortDelay + j * longDelay > remaining then
                        total
                    else
                        let nextTotal = total + paths
                        let nextPaths = paths * (i + j + 1L) / (j + 1L)
                        countLong (j + 1L) nextPaths nextTotal

                let nextLeaves = countLong 0L powerOfTwo leaves
                countShort (i + 1L) (powerOfTwo * 2L) nextLeaves

        countShort 0L 1L 1L

    if plant.Limit = 1L then
        1L
    else
        let rec findUpperBound seconds =
            if reachesLimit seconds then seconds
            else findUpperBound (seconds * 2L)

        let rec search low high =
            if low + 1L = high then high
            else
                let mid = (low + high) / 2L
                if reachesLimit mid then search low mid
                else search mid high

        search 1L (findUpperBound plant.Growth) |> int64

let execute() =
    let lines = LocalHelper.GetContent path 2026 9 3
    let plants = parseContent lines
    plants |> Seq.sumBy growPlant
