module quest01_2

open EverybodyCodes_2025_S3_FSharp.Modules

//let path = "quest01/test_input_02.txt"
let path = "quest01/quest01_input_02.txt"

let parseContent(lines: string seq) =
    let inline convertToNum(value: string) =
        let v' = 
            String.concat ""(value.ToCharArray()
            |> Array.map(fun v -> if v > 'Z' then 0 else 1)
            |> Array.map string)
        System.Convert.ToInt32(v', 2)
        
    lines
    |> Seq.map(fun l ->
        (
            l.Split(":")[0] |> int),
            convertToNum((l.Split(":")[1]).Split(" ")[0])+
            convertToNum((l.Split(":")[1]).Split(" ")[1])+
            convertToNum((l.Split(":")[1]).Split(" ")[2]),
            convertToNum((l.Split(":")[1]).Split(" ")[3])
    )


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let dnas = parseContent lines
    let (a, _, _) =
        dnas
        |> Seq.groupBy(fun (_, _, s) -> s)
        |> Seq.sortByDescending fst
        |> Seq.map snd   
        |> Seq.head
        |> Seq.minBy(fun (_, v, _) -> v)
    a

    