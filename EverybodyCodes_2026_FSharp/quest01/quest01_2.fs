module quest01_2

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open System

//let path = "quest01/test_input_02.txt"
let path = "quest01/quest01_input_02.txt"

let builds size =
    2 * size + (2 * (size - 2))

let minSize pines =
    (pines + 3) / 4

let size (pines: int) (minsize: int) =  
    Seq.initInfinite (fun i -> i + minsize)
    |> Seq.skipWhile(fun p ->
        let size = builds p
        size < pines
    )
    |> Seq.head

let cache = Dictionary<int, int>()


let execute() =
    let content = LocalHelper.GetContentFromFile(path)
    let remaining = content.Split(',') |> Seq.map int
    let minPines = builds 3
    remaining
    |> Seq.sumBy(fun r ->
        if r < minPines then
            minPines - r
        else
            let minsize = minSize r
            let canbuildsize = size r minsize
            (builds canbuildsize) - r
    )
