module quest07_2

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open System.Text.RegularExpressions

//let path = "quest07/test_input_02.txt"
//let path = "quest07/test_input_02b.txt"
let path = "quest07/quest07_input_02.txt"

type Cloud = {
    Id: int
    Energy: int64
    Connections: int array
}
let clouds = Dictionary<int, Cloud>()

let parseContent (lines: string array) =
    let parseLine (line: string) =
        let numbers = Regex.Matches(line, @"\d+") |> Seq.map(fun m -> int(m.Value)) |> Array.ofSeq
        clouds.Add(numbers[0], { Id = numbers[0]; Energy = numbers[1]; Connections = numbers[2..] })

    lines |> Seq.iter parseLine

let propagate (root: Cloud) (initial: int)=
    let rec transmit (totransmit: (int*int) list) (currentEnergy: int) =
        //printfn "transmiting %d to %d clouds" currentEnergy totransmit.Length
        if currentEnergy = 0 then
            clouds.Values |> Seq.sumBy _.Energy
        else
            for (id, times) in totransmit do
                let current = clouds[id]
                let old = current.Energy
                let energy = current.Energy + int64(times * currentEnergy)
                //printfn "increasing cloud %d with energy %d, %d times. from %d to : %d" id currentEnergy times old energy
                clouds[id] <- { current with Energy = energy }

            let newclouds =
                totransmit
                |> List.collect (fun (c, times) ->
                    clouds[c].Connections
                    |> Array.map (fun childId -> childId, times)
                    |> Array.toList)
                |> List.groupBy fst
                |> List.map (fun (childId, items) ->
                    childId, (items |> List.sumBy snd))

            transmit newclouds (currentEnergy - 1)
    transmit [(root.Id, 1)] initial


let execute() =
    let lines = LocalHelper.GetContent path 2026 7 2
    parseContent lines
    let firstCloud = clouds.Values |> Seq.maxBy _.Connections.Length
    propagate firstCloud 10