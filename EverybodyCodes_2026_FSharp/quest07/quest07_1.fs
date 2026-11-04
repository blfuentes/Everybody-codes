module quest07_1

open System.Text.RegularExpressions
open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic

//let path = "quest07/test_input_01.txt"
let path = "quest07/quest07_input_01.txt"

type Cloud = {
    Id: int
    Energy: int
    Connections: int array
}
let clouds = Dictionary<int, Cloud>()

let parseContent (lines: string array) =
    let parseLine (line: string) =
        let numbers = Regex.Matches(line, @"\d+") |> Seq.map(fun m -> int(m.Value)) |> Array.ofSeq
        clouds.Add(numbers[0], { Id = numbers[0]; Energy = numbers[1]; Connections = numbers[2..] })

    lines |> Seq.iter parseLine

let propagate (root: Cloud) (initial: int)=
    let rec transmit (totransmit: Cloud list) (currentEnergy: int) =
        //printfn "transmiting %d to %d clouds" currentEnergy totransmit.Length
        if currentEnergy = 0 then
            clouds.Values |> Seq.sumBy _.Energy
        else
            for cloud in totransmit do
                let current = clouds[cloud.Id]
                clouds[cloud.Id] <- { current with Energy = current.Energy + currentEnergy }

            let newclouds = 
                totransmit 
                |> List.collect (fun c -> c.Connections |> Array.map (fun cc -> clouds[cc]) |> List.ofArray)
            transmit newclouds (currentEnergy - 1)
    transmit [root] initial


let execute() =
    let lines = LocalHelper.GetContent path 2026 7 1
    parseContent lines
    let firstCloud = clouds.Values |> Seq.maxBy _.Connections.Length
    propagate firstCloud 5
