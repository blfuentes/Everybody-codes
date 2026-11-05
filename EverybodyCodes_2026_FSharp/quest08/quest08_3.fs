module quest08_3

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open quest08_utilities

//let path = "quest08/test_input_03.txt"
let path = "quest08/quest08_input_03.txt"

// a warrior leaves a seed at -3. seeds grow each round and sprout on reaching 1
let growArmy targetWarriors (field: Dictionary<int*int, int>) =
    let raysByDirection = directions |> Array.map (rays field)
    let cells = Array.ofSeq field.Keys
    let mutable warriors = 0
    let mutable energy = 0L
    let mutable round = 0

    while warriors < targetWarriors do
        for cell in cells do
            if field[cell] <= 0 then field[cell] <- field[cell] + 1

        let lit = litCells field raysByDirection[round % 6]
        energy <- energy + int64 lit.Count

        for cell in lit do
            if field[cell] = 9 then
                field[cell] <- -3
                warriors <- warriors + 1
            else
                field[cell] <- field[cell] + 1

        //printfn "Round %d %c energy %d warriors %d" (round + 1) "↘↙←↖↗→"[round % 6] lit.Count warriors
        round <- round + 1

    energy

let execute() =
    let field = LocalHelper.GetContent path 2026 8 3 |> parseField
    growArmy 100000 field
