module quest01_2

open EverybodyCodes_2026_S4_FSharp.Modules
open System.Collections.Generic

//let path = "quest01/test_input_02.txt"
let path = "quest01/quest01_input_02.txt"

let parseContent (lines: string seq) =
    lines
    |> Seq.map(fun line -> line.Split(',') |> Array.map int)

let dance (steps: int array) =
    let visited = new HashSet<int>()
    visited.Add(0) |> ignore
    steps
    |> Array.fold (fun pos step -> 
        if (pos - step < 0) || (visited.Contains(pos - step)) then 
            let mutable newPos = pos + step
            while visited.Contains(newPos) do newPos <- newPos + 1
            visited.Add(newPos) |> ignore
            newPos
        else
            let newPos = pos - step
            visited.Add(newPos) |> ignore
            newPos
    ) 0
    

let execute() =
    let dances = parseContent (ReadLines path)
    dances |> Seq.sumBy dance