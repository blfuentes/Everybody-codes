module quest08_part01

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest08/test_input_01.txt"
let path = "quest08/quest08_input_01.txt"

let parseContent (lines: string) =
    lines.Split(",") |> Array.map int

let findCenterTimes (nails: int array) (numOfNails: int) =
    nails
    |> Array.pairwise
    |> Array.sumBy(fun (a, b) ->
        if abs(a - b) = numOfNails / 2 then 1
        else 0
    )

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let nails = parseContent lines
    //let numOfNails = 8
    let numOfNails = 32
    findCenterTimes nails numOfNails
