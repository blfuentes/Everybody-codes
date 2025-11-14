module quest14_1

open EverybodyCodes_2024_FSharp.Modules

//let path = "quest14/test_input_01.txt"
let path = "quest14/quest14_input_01.txt"

type ActionType =
    | U
    | D
    | L
    | R
    | F
    | B

type Operation = {
    Action: ActionType
    Value: int
}

let parseContent(lines: string) =
    let parts = lines.Split(',')
    parts
    |> Array.map (fun p ->
        { Action =
            match p[0] with
            | 'U' -> U
            | 'D' -> D
            | 'L' -> L
            | 'R' -> R
            | 'F' -> F
            | 'B' -> B
            | _ -> failwith "Unknown action"
          Value = int (p[1..]) }
    )

let calculateHeight (operations: Operation array) =
    let ops =
        operations
        |> Seq.filter (fun k  -> k.Action.IsU || k.Action.IsD)
    let mutable mix = 0
    let mutable maxHeight = 0
    ops
    |> Seq.iter (fun op ->
        match op with
        | { Action = U; Value = v } -> mix <- mix + v
        | { Action = D; Value = v } -> mix <-mix - v
        | _ -> ()
        if mix > maxHeight then
            maxHeight <- mix
    )
    maxHeight

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let operations = parseContent(lines)
    calculateHeight operations
