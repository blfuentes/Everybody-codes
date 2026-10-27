module quest01_3

open EverybodyCodes_2026_FSharp.Modules
open System

//let path = "quest01/test_input_03.txt"
let path = "quest01/quest01_input_03.txt"

let builds size =
    size * size - 1

let minSize pines =
    let size = int(Math.Sqrt(pines + 1.))
    if size % 2 = 0 then size + 1 else size

let size (pines: int) (minsize: int) =
    Seq.initInfinite (fun i -> 2 * i + minsize)
    |> Seq.skipWhile(fun p ->
        let size = builds p
        size < pines
    )
    |> Seq.head

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
