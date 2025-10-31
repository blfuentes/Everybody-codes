module quest02_part02

open System.Numerics
open EverybodyCodes_2024_S2_FSharp.Modules

//let path = "quest02/test_input_02.txt"
let path = "quest02/quest02_input_02.txt"

let colourToInt colour =
    match colour with
    | 'R' -> 0
    | 'G' -> 1
    | 'B' -> 2
    | _ -> failwith "Invalid colour character"

let repeatList (items: 'a list) (n: int) : 'a list =
    if n <= 0 then []
    else
        Seq.replicate n items  // Lazily create a sequence of 'n' copies of the list
        |> Seq.collect id       // Flatten the sequence of lists into one big sequence
        |> List.ofSeq           // Convert the final sequence back into a list

let getMidInx items =
    (List.length items - 1) / 2

let removeInx inx items =
    (List.take inx items) @ (List.skip (inx + 1) items)

let removeMid items =
    removeInx (getMidInx items) items

let fireArrow arrowColour (hitBalloon::otherBalloons) =
    if arrowColour <> hitBalloon || (List.length otherBalloons) % 2 = 0 then
        otherBalloons
    else
        removeMid otherBalloons

let getNumArrows arrowColour balloonColours : bigint =
    let rec loop acc arrowColour balloonColours =
        match balloonColours with
        | [] -> acc // Base case: return the final accumulated count
        | _ ->
            let nextArrowColour = (arrowColour + 1) % 3
            let remainingBalloons = fireArrow arrowColour balloonColours
            loop (acc + 1I) nextArrowColour remainingBalloons
            
    loop 0I arrowColour balloonColours

let execute() =
    let balloonStr = LocalHelper.GetLinesFromFile(path)[0]

    let balloonColoursList =
        balloonStr
        |> Seq.map colourToInt
        |> Seq.toList

    let balloonColours = repeatList balloonColoursList 100
    getNumArrows 0 balloonColours

