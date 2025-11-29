module quest20_1

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest20/test_input_01.txt"
let path = "quest20/quest20_input_01.txt"

type Facing =
    | Down
    | Up
type CellType =
    | Wall
    | None
    | Trampoling

type Trampoling = {
    Id: string
    Position: int * int
    Orientation: Facing
    Kind: CellType
}

let mutable maxRow = 0
let mutable maxCol = 0
let TrampolingCollection = new Dictionary<int*int, Trampoling>()

let parseContent (lines: string array) =
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    let trampoling = Array2D.init (maxRow + 1) (maxCol + 1) (fun r c ->
        let id = sprintf "%d,%d" r c
        match r % 2 with
        | 0 ->
            match c % 2 with
            | 0 -> match lines[r][c] with
                    | '.' -> { Id = id; Position = (r, c); Orientation = Down; Kind = None }
                    | 'T' -> 
                        let t = { Id = id; Position = (r, c); Orientation = Down; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | '#' -> { Id = id; Position = (r, c); Orientation = Down; Kind = Wall }
                    | _ -> failwith "Invalid character"
            | 1 -> match lines[r][c] with
                    | '.' -> { Id = id; Position = (r, c); Orientation = Up; Kind = None }
                    | 'T' -> 
                        let t = { Id = id; Position = (r, c); Orientation = Up; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | '#' -> { Id = id; Position = (r, c); Orientation = Up; Kind = Wall }
                    | _ -> failwith "Invalid character"
            | _ -> failwith "Invalid character"
        | 1 ->
            match c % 2 with
            | 0 -> match lines[r][c] with
                    | '.' -> { Id = id; Position = (r, c); Orientation = Up; Kind = None }
                    | 'T' -> 
                        let t = { Id = id; Position = (r, c); Orientation = Up; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | '#' -> { Id = id; Position = (r, c); Orientation = Up; Kind = Wall }
                    | _ -> failwith "Invalid character"
            | 1 -> match lines[r][c] with
                    | '.' -> { Id = id; Position = (r, c); Orientation = Down; Kind = None }
                    | 'T' -> 
                        let t = { Id = id; Position = (r, c); Orientation = Down; Kind = Trampoling }
                        TrampolingCollection.Add((r,c), t)
                        t
                    | '#' -> { Id = id; Position = (r, c); Orientation = Down; Kind = Wall }
                    | _ -> failwith "Invalid character"
            | _ -> failwith "Invalid character"
        | _ -> failwith "Invalid value"
    )
    trampoling

let printTrampoling (trampoling: Trampoling[,]) =
    for r in 0 .. maxRow do
        for c in 0 .. maxCol do
            match trampoling[r, c].Kind with
            | Wall -> printf "#"
            | None -> printf "."
            | Trampoling -> 
                match trampoling[r, c].Orientation with
                | Down -> 
                    // print triangle pointing down
                    printf "D"
                | Up -> 
                    // print triangle pointing up
                    printf "U"
        printfn ""

let countTouchingTrampoling (trampoling: Trampoling[,]) =
    let pairs = HashSet<Trampoling * Trampoling>()
    for kvp in TrampolingCollection do
        let (r, c) = kvp.Key
        let t1 = kvp.Value
        let neighbors = 
            [ (r-1, c); (r+1, c); (r, c-1); (r, c+1); ]
        for (nr, nc) in neighbors do
            if TrampolingCollection.ContainsKey((nr, nc)) then
                let t2 = TrampolingCollection[(nr, nc)]
                let add = 
                    if r = nr then
                        t1.Orientation <> t2.Orientation                        
                    elif r < nr then
                        t1.Orientation = Up && t2.Orientation = Down
                    else
                        t1.Orientation = Down && t2.Orientation = Up
                if add then
                    if not(pairs.Contains((t1, t2)) || pairs.Contains((t2, t1))) then
                            pairs.Add((t1, t2)) |> ignore
    pairs.Count
let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let trampolingMap = parseContent lines
    //printTrampoling trampolingMap
    countTouchingTrampoling trampolingMap
