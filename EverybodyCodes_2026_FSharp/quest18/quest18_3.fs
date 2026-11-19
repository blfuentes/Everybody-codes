module quest18_3

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

//let path = "quest18/test_input_03.txt"
let path = "quest18/quest18_input_03.txt"

let parseContent (lines: string[]) =
    [
        for line in lines do
            let numbers = 
                Regex.Matches(line, @"\d+") 
                |> Seq.map(fun m -> bigint.Parse(m.Value))
                |> Array.ofSeq
                |> fun xs ->(xs[0], xs[1], xs[2])
            yield (numbers)
    ]

let countFilling(stacks: (bigint*bigint*bigint) list) =
    let rectangles = List.toArray stacks
    let tallestRight = Array.zeroCreate<bigint> rectangles.Length
    let mutable rightHeight = 0I

    for i in rectangles.Length - 1 .. -1 .. 0 do
        let (_, height, _) = rectangles[i]
        rightHeight <- max rightHeight height
        tallestRight[i] <- rightHeight

    let mutable total = 0I
    let mutable leftHeight = 0I
    let mutable previousEnd = None

    for i in 0 .. rectangles.Length - 1 do
        let (x, height, width) = rectangles[i]
        match previousEnd with
        | Some endX ->
            total <- total + (x - endX) * min leftHeight tallestRight[i]
        | None -> ()

        leftHeight <- max leftHeight height
        total <- total + width * min leftHeight tallestRight[i]
        previousEnd <- Some (x + width)

    total


let execute() =
    let lines = LocalHelper.GetContent path 2026 18 3
    let stacks = parseContent lines
    countFilling stacks
