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


// Without resize array
//let buildFishbone (id, segments: int array) =
//    let findPlace value fishbone =
//        let rec loop acc remaining =
//            match remaining with
//            | [] -> (List.rev acc, None, ROOT)
//            | seg :: rest ->
//                match seg with
//                | { Root = r; Left = None; Right = _ } when value < r -> (List.rev acc, Some seg, LEFT)
//                | { Root = r; Left = _; Right = None } when value > r -> (List.rev acc, Some seg, RIGHT)
//                | _ -> loop (seg :: acc) rest
//        loop [] fishbone

//    let updateSegment seg side value =
//        match side with
//        | LEFT -> { seg with Left = Some value }
//        | RIGHT -> { seg with Right = Some value }
//        | ROOT -> seg

//    let (_, fishbone) =
//        segments
//        |> Array.fold (fun (prev, acc) s ->
//            let (before, targetOpt, side) = findPlace s acc
//            match side, targetOpt with
//            | ROOT, _ ->
//                let newSeg = { Root = s; Left = None; Right = None; Next = None }
//                let updatedPrev =
//                    match prev with
//                    | Some p -> { p with Next = Some newSeg }
//                    | None -> newSeg
//                (Some newSeg, acc @ [newSeg])
//            | LEFT, Some target ->
//                let updated = updateSegment target LEFT s
//                let updatedList = before @ [updated] @ (List.tail acc |> List.skip (List.length before))
//                (prev, updatedList)
//            | RIGHT, Some target ->
//                let updated = updateSegment target RIGHT s
//                let updatedList = before @ [updated] @ (List.tail acc |> List.skip (List.length before))
//                (prev, updatedList)
//            | _, None -> (prev, acc)
//        ) (None, [])

//    fishbone

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

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (id, segments) = parseContent lines
    let fishbone = buildFishbone (id, segments)
    display fishbone