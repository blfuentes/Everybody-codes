module quest02_part03

open EverybodyCodes_2024_S2_FSharp.Modules

//let path = "quest02/test_input_03.txt"
let path = "quest02/quest02_input_03.txt"

let numRepeats = 100000

let colourToInt colour =
    match colour with
    | 'R' -> 0
    | 'G' -> 1
    | 'B' -> 2
    | _ -> failwith "Invalid colour character"

let repeatList (items: 'a list) (n: int) : 'a list =
    if n <= 0 then []
    else
        Seq.replicate n items  // Lazily create n copies of the list
        |> Seq.collect id       // Flatten the sequence of lists
        |> List.ofSeq           // Convert back to a single list

let getMidInx items =
    (List.length items - 1) / 2

let getFrontHalf items =
    List.take (getMidInx items + 1) items

let getBackHalf items =
    List.skip (getMidInx items + 1) items

let hitsDouble arrowColour frontBalloon =
    arrowColour = frontBalloon

let getSecondHalf' arrowColour frontHalf backHalf =
    let rec loop acc arrowColour frontHalf backHalf =
        match frontHalf, backHalf with
        | [], bh -> (List.rev acc) @ bh // Prepend reversed accumulator to rest of backHalf
        | _, [] -> List.rev acc         // Return reversed accumulator

        | f::fs, b::bs ->
            if hitsDouble arrowColour f then
                loop acc ((arrowColour + 1) % 3) fs bs
            else
                match fs with
                | [] ->
                    loop (b::acc) ((arrowColour + 2) % 3) [] bs
                | _::fs' ->
                    loop (b::acc) ((arrowColour + 2) % 3) fs' bs

    loop [] arrowColour frontHalf backHalf

let getSecondHalf balloonColours arrowColour =
    getSecondHalf' arrowColour (getFrontHalf balloonColours) (getBackHalf balloonColours)

let divideAndConquer balloonColours arrowColour : bigint =
    let rec loop (acc: bigint) arrowColour balloonColours =
        match balloonColours with
        | [] -> acc // Base case: return final count
        | _ ->
            let len = List.length balloonColours
            if len % 2 = 0 then
                let lenFront = len / 2
                let newBalloons = getSecondHalf balloonColours arrowColour
                let newArrowColour = (arrowColour + lenFront) % 3
                loop (acc + (bigint lenFront)) newArrowColour newBalloons
            else
                let newBalloons = List.tail balloonColours
                let newArrowColour = (arrowColour + 1) % 3
                loop (acc + 1I) newArrowColour newBalloons

    loop 0I arrowColour balloonColours

let execute() =
    let balloonStr = LocalHelper.GetLinesFromFile(path)[0]
    let balloonSequence =
        balloonStr
        |> Seq.map colourToInt
        |> List.ofSeq
    let balloonColours = repeatList balloonSequence numRepeats

    divideAndConquer balloonColours 0