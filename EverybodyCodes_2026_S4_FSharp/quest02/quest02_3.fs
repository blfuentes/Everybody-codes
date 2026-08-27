module quest02_3

open EverybodyCodes_2026_S4_FSharp.Modules
open System.Text.RegularExpressions
open System.Collections.Generic

//let path = "quest02/test_input_03.txt"
//let path = "quest02/test_input_03b.txt"
let path = "quest02/quest02_input_03.txt"

type Beacon = {
    Id: char
    X: int
    Y: int
}

let parseContent (lines: string array) =
    let startval = Regex.Matches(lines[0], $"\d+") |> Seq.map(fun m -> (int)m.Value) |> Seq.toArray
    let start = (startval[0], startval[1])

    let beacons =
        [for beaconline in lines[1..] do
            if beaconline.Contains("=") then
                let beaconval = Regex.Matches(beaconline, $"\d+") |> Seq.map(fun m -> (int)m.Value) |> Seq.toArray
                yield { Id = beaconline[0]; X = beaconval[0]; Y = beaconval[1] }]

    (start, beacons)

// floodfill to find all possible visited points
let exploreAll (start: int*int) (beacons: Beacon list) =
    let lighted = new HashSet<int*int>()
    let queue = new Queue<int*int>()
    lighted.Add(start) |> ignore
    queue.Enqueue(start)
    while queue.Count > 0 do
        let (sx, sy) = queue.Dequeue()
        for beacon in beacons do
            let next = (abs(beacon.X + sx) / 2, abs(beacon.Y + sy) / 2)
            if lighted.Add(next) then
                queue.Enqueue(next)
    lighted

let fillFireflies (lighted: HashSet<int*int>) =
    let fireflies = new HashSet<int*int>()
    lighted
    |> Seq.iter(fun (lx, ly) -> 
        fireflies.Add(lx - 1, ly) |> ignore
        fireflies.Add(lx + 1, ly) |> ignore
        fireflies.Add(lx, ly - 1) |> ignore
        fireflies.Add(lx, ly + 1) |> ignore
    )
    Set.difference (fireflies |> Set.ofSeq) (lighted |> Set.ofSeq) 

let execute() =
    let (start, beacons) = parseContent <| ((ReadLines path) |> Seq.toArray)
    let lights = exploreAll start beacons
    let fireflies = fillFireflies lights
    fireflies.Count