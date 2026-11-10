module quest11_3

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open System.Text.RegularExpressions
open System

//let path = "quest11/test_input_03.txt"
let path = "quest11/quest11_input_03.txt"

let ensureNode key value (connections: Dictionary<int, ResizeArray<int>>) =
    if not (connections.ContainsKey(key)) then
        connections.Add(key, ResizeArray<int>())
    if not (connections[key].Contains(value)) then
        connections[key].Add(value)

let addConnection left right connections =
    ensureNode left right connections
    ensureNode right left connections

let parseContent (lines: string array) =
    let numOfPortals = int(Regex.Match(lines[0], @"\d+").Value)
    let shortcuts =
        lines
        |> Seq.skip(2)
        |> Seq.map(fun line ->
        let numbers = Regex.Matches(line, @"\d+") |> Seq.map (fun m -> int(m.Value)) |> Array.ofSeq
        numbers[0], numbers[1])
        |> Array.ofSeq

    numOfPortals, shortcuts

let buildConnections portals shortcuts =
    let connections = Dictionary<int, ResizeArray<int>>()
    for portal in 1..portals do
        connections.Add(portal, ResizeArray<int>())

    for portal in 1..portals do
        for offset in 1..3 do
            let clockwise = (portal - 1 + offset) % portals + 1
            let counterclockwise = ((portal - 1 - offset) % portals + portals) % portals + 1
            addConnection portal clockwise connections
            addConnection portal counterclockwise connections

    for left, right in shortcuts do
        addConnection left right connections

    connections

let totalStrain portals (connections: Dictionary<int, ResizeArray<int>>) =
    let distances = Array.create (portals + 1) -1
    let toVisit = Queue<int>()
    let mutable total = 0

    for start in 1..portals do
        Array.fill distances 0 distances.Length -1
        toVisit.Clear()
        distances[start] <- 0
        toVisit.Enqueue(start)

        while toVisit.Count > 0 do
            let current = toVisit.Dequeue()
            for neighbor in connections[current] do
                if distances[neighbor] = -1 then
                    distances[neighbor] <- distances[current] + 1
                    total <- total + distances[neighbor]
                    toVisit.Enqueue(neighbor)

    total

let rec pairings crystals =
    match crystals with
    | [] -> [ [] ]
    | first :: rest ->
        [ for partner in rest do
            for pairs in pairings (rest |> List.filter ((<>) partner)) do
                yield (first, partner) :: pairs ]

let execute() =
    let lines = LocalHelper.GetContent path 2026 11 3
    let numOfPortals, shortcuts = parseContent lines
    let initialStrain = buildConnections numOfPortals [||] |> totalStrain numOfPortals
    let spacing = numOfPortals / 8

    let bestFinalStrain =
        seq {
            for first in 1..spacing do
                let crystals = [ for i in 0..7 -> first + i * spacing ]
                for pairs in pairings crystals do
                    let candidateShortcuts = Array.append shortcuts (Array.ofList pairs)
                    let connections = buildConnections numOfPortals candidateShortcuts
                    yield totalStrain numOfPortals connections
        }
        |> Seq.min

    initialStrain - bestFinalStrain
