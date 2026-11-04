module quest07_3

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open System.Text.RegularExpressions

//let path = "quest07/test_input_03.txt"
//let path = "quest07/test_input_03b.txt"
let path = "quest07/quest07_input_03.txt"

type Cloud = {
    Id: int
    Energy: int64
    Connections: int array
}
let clouds = Dictionary<int, Cloud>()

let parseContent (lines: string array) =
    clouds.Clear()

    let parseLine (line: string) =
        let numbers = Regex.Matches(line, @"\d+") |> Seq.map(fun m -> int(m.Value)) |> Array.ofSeq
        clouds.Add(numbers[0], { Id = numbers[0]; Energy = numbers[1]; Connections = numbers[2..] })

    lines |> Seq.iter parseLine

let explosionThreshold = 1_000_000L

type CloudCache = {
    Ids: int array
    InitialEnergies: int64 array
    Connections: int array array
}

let buildCache () =
    let allClouds = clouds.Values |> Seq.toArray
    let idToIndex = Dictionary<int, int>(allClouds.Length)

    allClouds
    |> Array.iteri (fun idx cloud -> idToIndex[cloud.Id] <- idx)

    {
        Ids = allClouds |> Array.map _.Id
        InitialEnergies = allClouds |> Array.map _.Energy
        Connections =
            allClouds
            |> Array.map (fun cloud -> cloud.Connections |> Array.map (fun childId -> idToIndex[childId]))
    }

let aggregate (transmissions: (int * int * int) list) =
    let grouped = Dictionary<struct (int * int), int>()

    for (targetIdx, strength, times) in transmissions do
        if strength > 0 && times > 0 then
            let key = struct (targetIdx, strength)
            match grouped.TryGetValue key with
            | true, previousTimes -> grouped[key] <- previousTimes + times
            | _ -> grouped[key] <- times

    [ for KeyValue(struct (targetIdx, strength), times) in grouped do
          yield targetIdx, strength, times ]

let propagate (cache: CloudCache) (rootIdx: int) (initial: int) =
    let energies = Array.copy cache.InitialEnergies
    let exploded = Array.zeroCreate<bool> cache.Ids.Length
    let mutable remainingEnergy = energies |> Array.sum

    let rec transmit (totransmit: (int * int * int) list) =
        let activeTransmissions =
            totransmit
            |> List.filter (fun (targetIdx, strength, times) -> strength > 0 && times > 0 && not exploded[targetIdx])

        if List.isEmpty activeTransmissions then
            remainingEnergy
        else
            for (targetIdx, strength, times) in activeTransmissions do
                let delta = int64 strength * int64 times
                energies[targetIdx] <- energies[targetIdx] + delta
                remainingEnergy <- remainingEnergy + delta

            let propagated =
                activeTransmissions
                |> List.collect (fun (sourceIdx, strength, times) ->
                    if strength > 1 then
                        cache.Connections[sourceIdx]
                        |> Array.map (fun childIdx -> childIdx, strength - 1, times)
                        |> Array.toList
                    else
                        [])

            let explodingNow =
                [ for idx in 0 .. cache.Ids.Length - 1 do
                      if not exploded[idx] && energies[idx] >= explosionThreshold then
                          yield idx ]

            for idx in explodingNow do
                exploded[idx] <- true
                remainingEnergy <- remainingEnergy - energies[idx]

            let explosionBursts =
                explodingNow
                |> List.collect (fun sourceIdx ->
                    cache.Connections[sourceIdx]
                    |> Array.map (fun childIdx -> childIdx, 10, 1)
                    |> Array.toList)

            let nextWave =
                propagated @ explosionBursts
                |> aggregate
                |> List.filter (fun (targetIdx, _, _) -> not exploded[targetIdx])

            transmit nextWave

    transmit [ (rootIdx, initial, 1) ]

let execute() =
    let lines = LocalHelper.GetContent path 2026 7 3
    parseContent lines

    let cache = buildCache ()

    cache.Ids
    |> Array.mapi (fun idx _ -> propagate cache idx 10)
    |> Array.max
