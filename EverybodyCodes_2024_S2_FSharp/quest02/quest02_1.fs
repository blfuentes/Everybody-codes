module quest02_part01

open EverybodyCodes_2024_S2_FSharp.Modules

//let path = "quest02/test_input_01.txt"
let path = "quest02/quest02_input_01.txt"

// Convert balloon colour character to an integer
let colourToInt (colour: char) : int =
    match colour with
    | 'R' -> 0
    | 'G' -> 1
    | 'B' -> 2
    | _   -> failwithf "Unexpected colour: %c" colour

// Recursive function to compute number of arrows
let rec getNumArrows (arrowColour: int) (balloons: int list) : int =
    match balloons with
    | [] -> 1
    | [hitBalloon] -> 1
    | hitBalloon :: otherBalloons ->
        if arrowColour = hitBalloon then
            getNumArrows arrowColour otherBalloons
        else
            1 + getNumArrows ((arrowColour + 1) % 3) otherBalloons

let execute() =
    let balloonStr = LocalHelper.GetLinesFromFile(path)[0]
    let balloonColours =
        balloonStr
        |> Seq.map colourToInt
        |> Seq.toList

    getNumArrows 0 balloonColours
