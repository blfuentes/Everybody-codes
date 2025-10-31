module quest03_part02

open EverybodyCodes_2024_S2_FSharp.Modules
open System

//let path = "quest03/test_input_02.txt"
let path = "quest03/quest03_input_02.txt"

type Dice(id:int, faces:int list, seed:int) =
    let mutable pulse = bigint seed
    let mutable faceId = bigint 0
    let mutable rollNumber = bigint 1
    let mutable spin = bigint 0
    let mutable target = 0

    member _.Id = id
    member _.Faces = faces
    member _.Seed = seed
    member _.Target with get() = target

    member this.Roll() : int =
        spin <- rollNumber * pulse
        faceId <- (faceId + spin) % (bigint faces.Length)
        // ensure non-negative index
        if faceId < 0I then faceId <- faceId + (bigint faces.Length)

        pulse <- pulse + spin
        pulse <- pulse % (bigint seed)
        pulse <- pulse + (bigint (1 + seed)) + rollNumber
        rollNumber <- rollNumber + 1I

        faces.[int faceId]

    member this.NextTarget(sequence:int list) : bool =
        if target >= sequence.Length - 1 then
            true
        else
            target <- target + 1
            false

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)    
    // Parse dice definitions until empty line
    let diceDefs =
        lines
        |> Array.takeWhile (fun line -> line <> "")
        |> Array.map (fun line ->
            let parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            let id = int (parts.[0].TrimEnd(':'))
            let faces =
                parts.[1].Split('=').[1].Trim()
                |> fun s -> s.Trim([|'[';']'|])
                |> fun s -> s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                |> Array.map int
                |> Array.toList
            let seed = int (parts.[2].Split('=').[1])
            Dice(id, faces, seed))
        |> Array.toList

    // Last line is the sequence
    let sequence =
        lines.[lines.Length - 1]
        |> Seq.map (fun ch -> int (string ch))
        |> Seq.toList

    // Mutable working list of dice
    let mutable dices = diceDefs
    let mutable finishers : int list = []

    // Loop until no dice remain
    while not dices.IsEmpty do
        let finishedIndices =
            dices
            |> List.mapi (fun i dice ->
                let result = dice.Roll()
                if result = sequence.[dice.Target] && dice.NextTarget(sequence) then
                    Some i else None)
            |> List.choose id

        // Remove finished dice in reverse order
        for i in List.rev finishedIndices do
            let finisher = List.item i dices
            finishers <- finishers @ [finisher.Id]
            dices <- dices |> List.mapi (fun j d -> if j = i then None else Some d) |> List.choose id

    sprintf "%s" (String.Join(",", finishers))
