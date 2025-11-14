module quest03_1

open EverybodyCodes_2024_S2_FSharp.Modules
open System.Text.RegularExpressions
open System.IO
open System

//let path = "quest03/test_input_01.txt"
let path = "quest03/quest03_input_01.txt"

type Dice(id:int, faces:int list, seed:int) =
    let mutable pulse = seed
    let mutable faceId = 0
    let mutable rollNumber = 1
    let mutable spin = 0
    member _.Id = id
    member _.Faces = faces
    member _.Seed = seed
    member this.Roll() : int =
        spin <- rollNumber * pulse
        faceId <- (faceId + spin) % faces.Length
        pulse <- pulse + spin
        pulse <- pulse % seed
        pulse <- pulse + 1 + rollNumber + seed
        rollNumber <- rollNumber + 1
        faces.[faceId]


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)

    // Parse each line into a Dice
    let dices =
        [ for line in lines ->
            let parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            let id = int (parts.[0].TrimEnd(':'))
            let faces =
                parts.[1].Split('=').[1].Trim()
                |> fun s -> s.Trim([|'[';']'|])
                |> fun s -> s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                |> Array.map int
                |> Array.toList
            let seed = int (parts.[2].Split('=').[1])
            Dice(id, faces, seed) ]

    // Loop until total >= 10_000
    let mutable total = 0
    let mutable counter = 0
    while total < 10_000 do
        let diceTotal = dices |> List.sumBy (fun d -> d.Roll())
        total <- total + diceTotal
        counter <- counter + 1
    counter
