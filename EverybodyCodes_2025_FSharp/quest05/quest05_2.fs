module quest05_part02

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest05/test_input_02.txt"
let path = "quest05/quest05_input_02.txt"

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
    let definitions = 
        seq {
            for line in lines do
                let (id, segments) = 
                    (int(line.Split(":")[0]),
                        (line.Split(":")[1]).Split(",") |> Array.map int)
                yield (id, segments)
        }
    definitions

let buildFishbone((id, segments): (int*int array)) =
    let fishbone = ResizeArray<Segment>()
    let findPlace (value: int) =
        let decision segment =
            match segment with
            | { Root = r; Left = None; Right = _ } when value < r -> Some LEFT
            | { Root = r; Left = _; Right = None } when value > r -> Some RIGHT
            | _ -> None
    
        match fishbone |> Seq.tryFindIndex (fun seg -> decision seg |> Option.isSome) with
        | Some idx ->
            let side = decision fishbone[idx] |> Option.defaultValue ROOT
            (idx, side)
        | None ->
            (fishbone.Count, ROOT)
    
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

let valueOf(fishbone: Segment seq) =
    bigint.Parse((String.concat"" (fishbone |> Seq.map (fun f -> f.Root.ToString()))))

let findDiff(values: bigint seq) =
    let v' =
        values
        |> Seq.sortDescending
    (v' |> Seq.head) - (v' |> Seq.last)
    

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let definitions = parseContent lines
    let fishbones = definitions |> Seq.map buildFishbone |> Seq.map valueOf
    findDiff fishbones
