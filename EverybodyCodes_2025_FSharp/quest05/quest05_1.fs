module quest05_part01

open System.Collections.Generic
open EverybodyCodes_2025_FSharp.Modules

//let path = "quest05/test_input_01.txt"
let path = "quest05/quest05_input_01.txt"

type Segment = {
    Root: int;
    Left: int option;
    Right: int option;
    Next: Segment option;
}

type Side = 
    | LEFT
    | RIGHT
    | ROOT

let parseContent (lines: string array) =
    let (id, segments) = 
        (int(lines[0].Split(":")[0]),
            (lines[0].Split(":")[1]).Split(",") |> Array.map int)
    (id, segments)



let buildFishbone((id, segments): (int*int array)) =
    let fishbone = ResizeArray<Segment>()
    let findPlace(value: int) =
        let mutable idx = 0
        let mutable doContinue = true
        let mutable side = ROOT
        if fishbone.Count = 0 then
            (idx, side)
        else
            while doContinue do
                let toCheck = fishbone[idx]
                match toCheck with
                | { Root = _; Left = Some l; Right = Some r; } ->
                    idx <- idx + 1
                | { Root = _; Left = _; Right = _ } when value = toCheck.Root ->
                    idx <- idx + 1
                | { Root = _; Left = None; Right = _; } when value < toCheck.Root ->
                    doContinue <- false
                    side <- LEFT
                | { Root = _; Left = Some l; Right = _; } when value <= toCheck.Root ->
                    idx <- idx + 1
                | { Root = _; Left = _; Right = None; } when value > toCheck.Root ->
                    doContinue <- false
                    side <- RIGHT
                | { Root = _; Left = _; Right = Some r; } when value >= toCheck.Root ->
                    idx <- idx + 1
                if idx = fishbone.Count then
                    doContinue <- false
                    side <- ROOT
            (idx, side)
    segments |> Array.iter(fun s ->
        let (idx, side) = findPlace s
        match side with
        | LEFT -> fishbone[idx] <- { fishbone[idx] with Left = Some s }
        | RIGHT -> fishbone[idx] <- { fishbone[idx] with Right = Some s }
        | ROOT -> 
            fishbone.Add( { Root = s; Left = None; Right = None; Next = None })
            if fishbone.Count > 1 then
                fishbone[idx-1] <- { fishbone[idx-1] with Next = Some fishbone[idx] }
    )
    fishbone

let display (fishbone: Segment seq) =
    let part = (String.concat"" (fishbone |> Seq.map (fun f -> f.Root.ToString())))
    sprintf "%s" part

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (id, segments) = parseContent lines
    let fishbone = buildFishbone (id, segments)
    display fishbone