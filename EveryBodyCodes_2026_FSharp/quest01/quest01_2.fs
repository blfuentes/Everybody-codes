module quest01_2

open EverybodyCodes_2026_FSharp.Modules

//let path = "quest01/test_input_02.txt"
let path = "quest01/quest01_input_02.txt"

let builds size =
    2 * size + (2 * (size - 2))

let size pines =
    Seq.initInfinite id
    |> Seq.skip(3)
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
            let canbuildsize = size r
            (builds canbuildsize) - r
    )
