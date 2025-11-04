module quest01_part03

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest01/test_input_03.txt"
let path = "quest01/quest01_input_03.txt"

let parseContent (lines: string array) =
    let names = lines[0].Split(",")
    let movements = 
        lines[2].Split(",") 
        |> Seq.map (fun mov -> ((mov.Substring(0,1), int(mov.Substring(1)))))
    (names, movements)

let findName (names: string array) (movements: (string*int) seq) =
    let namesLength = names.Length
    movements
    |>
    Seq.iter(fun (direction, steps) ->
        let swapIdx =
            match direction with
            | "R" -> steps % namesLength
            | "L" -> modn (0 - steps) namesLength
            | _ -> failwith "unexpected direction"
        let (n1, n2) = (names[0], names[swapIdx])
        names[swapIdx] <- n1
        names[0] <- n2
    ) 
    names[0]
    
let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (names, movements) = parseContent lines
    findName names movements
