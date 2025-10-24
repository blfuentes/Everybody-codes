module quest14_part02

open EverybodyCodes_2024_FSharp.Modules

//let path = "quest14/test_input_02.txt"
let path = "quest14/quest14_input_02.txt"

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

let walkPath (operations: Operation array) =
    let mutable position: int*int*int = (0,0,0) // x,y,z
    let mutable visited = Set.empty<int*int*int>
    operations
    |> Array.iter (fun op ->
        match op with
        | { Action = F; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx, cy, cz + 1)
                visited <- visited.Add(position)
        | { Action = B; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx, cy, cz - 1)
                visited <- visited.Add(position)
        | { Action = D; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx, cy - 1, cz)
                visited <- visited.Add(position)
        | { Action = U; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx, cy + 1, cz)
                visited <- visited.Add(position)
        | { Action = L; Value = v } -> 
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx - 1, cy, cz)
                visited <- visited.Add(position)
        | { Action = R; Value = v } ->
            for _i in 1 .. v do
                let (cx, cy, cz) = position
                position <- (cx + 1, cy, cz)
                visited <- visited.Add(position)
    )
    visited    

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let operationsCollection = lines |> Array.map parseContent
    let allpoints =         
        operationsCollection
            |> Array.map walkPath
            |> Array.reduce (fun acc set -> Set.union acc set)
    allpoints.Count
