module quest05_part03

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest05/test_input_03.txt"
let path = "quest05/quest05_input_03.txt"

type Segment = {
    Root: int;
    Left: int option;
    Right: int option;
    Next: Segment option;
}

type Fishbone = {
    Id: int;
    Spine: Segment seq
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
    { Id = id; Spine = fishbone }

let display (fishbone: Segment seq) =
    let part = (String.concat"" (fishbone |> Seq.map (fun f -> f.Root.ToString())))
    sprintf "%s" part

let valueOfSpine(fishbone: Fishbone) =
    bigint.Parse((String.concat"" (fishbone.Spine |> Seq.map (fun f -> f.Root.ToString()))))

let valueOfLevel(segment: Segment) =
    match (segment.Left, segment.Root, segment.Right) with
    | (Some l, ro, Some r) -> bigint.Parse(string(l) + string(ro) + string(r))
    | (Some l, ro, None) -> bigint.Parse(string(l) + string(ro))
    | (None, ro, Some r) -> bigint.Parse(string(ro) + string(r))
    | _ -> bigint(segment.Root)

let compareFishbones(f1: Fishbone) (f2: Fishbone) =
    let rec compareSpine (lvl: int) =
        if lvl = (f1.Spine |> Seq.length) then
            0
        else
            let (lvl1, lvl2) = (
                valueOfLevel (f1.Spine |> Seq.item(lvl)),
                valueOfLevel(f2.Spine |> Seq.item(lvl))
            )
            if lvl1 > lvl2 then 1
            elif lvl1 < lvl2 then -1
            else
                compareSpine (lvl + 1)
    let (valueOf1, valueOf2) = (valueOfSpine f1, valueOfSpine f2)
    if valueOf1 > valueOf2 then
        1
    elif valueOf1 < valueOf2 then
        -1
    else
        match compareSpine 0 with
        | 0 -> if f1.Id > f2.Id then 1 else -1
        | 1 -> 1
        | -1 -> -1
        | _ -> failwith "error"        

let calculateChecksum (fishbones: Fishbone seq) =
    let sorted = fishbones |> Seq.sortWith compareFishbones |> Seq.rev
    sorted
    |> Seq.mapi(fun i v -> (i+1)*v.Id)
    |> Seq.sum

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let definitions = parseContent lines
    let fishbones = definitions |> Seq.map buildFishbone
    calculateChecksum fishbones
