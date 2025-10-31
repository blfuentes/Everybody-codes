module quest03_part01

open EverybodyCodes_2024_S1_FSharp.Modules

//let path = "quest03/test_input_01.txt"
let path = "quest03/quest03_input_01.txt"

type Snail = {
    X: int;
    Y: int;
}

let parseContent (lines: string array) =
    let snails =
        seq {
            for line in lines do
                { 
                    X = int((line.Split(" ")[0]).Split("=")[1]);
                    Y = int((line.Split(" ")[1]).Split("=")[1]);
                }    
        } |> Seq.toList
    snails

let newPosition (snail: Snail) (numOfDays: int) =
    let cycle = snail.X + snail.Y - 1
    let newX = (snail.X + numOfDays - 1) % cycle + 1
    let newY = cycle + 1 - newX

    { X = newX; Y = newY }

let moveSnail (snails: Snail list) (numOfDays) =
    snails
    |> List.map(fun snail ->
        newPosition snail numOfDays
        //printfn "Snail moved from (%d, %d) to (%d, %d)" snail.X snail.Y newSnail.X newSnail.Y
    )

let calculate (snail: Snail) =
    snail.X + (100 * snail.Y)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let snails = parseContent lines
    moveSnail snails 100
    |> List.sumBy calculate

