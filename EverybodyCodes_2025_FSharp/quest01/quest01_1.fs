module quest01_1

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest01/test_input_01.txt"
let path = "quest01/quest01_input_01.txt"

let parseContent (lines: string array) =
    let names = lines[0].Split(",")
    let movements = 
        lines[2].Split(",") 
        |> Seq.map (fun mov -> ((mov.Substring(0,1), int(mov.Substring(1)))))
    (names, movements)

let findName (names: string array) (movements: (string*int) seq) =
    let namesLength = names.Length
    let finalIndex =
        Seq.fold (fun acc (direction, steps) ->
            match direction with
            | "R" -> min (acc + steps) (namesLength - 1)
            | "L" -> max (acc - steps) 0
            | _ -> acc 
        ) 0 movements
    names[finalIndex]
    
let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (names, movements) = parseContent lines
    findName names movements