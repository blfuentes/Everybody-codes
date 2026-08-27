module quest02_1

open EverybodyCodes_2026_S4_FSharp.Modules
open System.Text.RegularExpressions
open System.Collections.Generic

//let path = "quest02/test_input_01.txt"
let path = "quest02/quest02_input_01.txt"

type Beacon = {
    Id: char
    X: int
    Y: int
}

let parseContent (lines: string array) =
    let startval = Regex.Matches(lines[0], $"\d+") |> Seq.map(fun m -> (int)m.Value) |> Seq.toArray
    let start = (startval[0], startval[1])

    let beacons =
        [for beaconline in lines[1..lines.Length - 2] do
            let beaconval = Regex.Matches(beaconline, $"\d+") |> Seq.map(fun m -> (int)m.Value) |> Seq.toArray
            yield { Id = beaconline[0]; X = beaconval[0]; Y = beaconval[1] }]
        |> List.map(fun b -> b.Id, b)
        |> Map.ofList
    let moves = (lines[lines.Length-1].Split("=")[1]).ToCharArray() |> List.ofArray |> List.map char

    (start, beacons, moves)

let rec consumeMoves (moves: char list) (swarm:int*int, beacons:Map<char, Beacon>) (lighted: HashSet<int*int>) =
    match moves with
    | [] -> 
        lighted
    | move::remaining ->
        let beacon = beacons[move]
        let newSwarmX = (abs(beacon.X + (fst swarm)) / 2)
        let newSwarmY = (abs(beacon.Y + (snd swarm)) / 2)
        lighted.Add(newSwarmX, newSwarmY) |> ignore
        consumeMoves remaining ((newSwarmX, newSwarmY), beacons) lighted

let execute() =
    let (start, beacons, moves) = parseContent <| ((ReadLines path) |> Seq.toArray)
    let lighted = new HashSet<int*int>()
    lighted.Add(start) |> ignore
    let lights = consumeMoves moves (start, beacons) lighted
    lights.Count
    