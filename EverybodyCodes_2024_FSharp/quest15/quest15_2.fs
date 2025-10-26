module quest15_part02

open EverybodyCodes_2024_FSharp.Modules
open System
open System.Collections.Generic

//let path = "quest15/test_input_02.txt"
let path = "quest15/quest15_input_02.txt"

type Coord = int * int
type Path = Dictionary<Coord,string>
type POI = Dictionary<string, HashSet<Coord>>

let parseContent (data: string list) : POI * Path =
    let path = Dictionary<Coord,string>()
    let poi = Dictionary<string, HashSet<Coord>>()
    let addPoi key coord =
        if not (poi.ContainsKey key) then poi[key] <- HashSet()
        poi[key].Add coord |> ignore
    for y, line in Seq.indexed data do
        for x, c in Seq.indexed line do
            match c with
            | '#' | '~' -> ()
            | '.' ->
                path[(y,x)] <- "."
                if y = 0 then
                    addPoi "Start" (y,x)
                    path[(y,x)] <- "Start"
            | _ ->
                let s = string c
                path[(y,x)] <- s
                addPoi s (y,x)
    poi, path

// BFS distances
let calculateDistances (path: Path) (src: Coord) (poi: POI) =
    let distances = Dictionary<(string*Coord), int>()
    let mutable boundary = HashSet([src])
    let seen = HashSet<Coord>()
    let mutable dist = 0
    while boundary.Count > 0 do
        let newBoundary = HashSet<Coord>()
        for (y,x) in boundary do
            if path.ContainsKey (y,x) then
                let label = path[(y,x)]
                if poi.ContainsKey label then
                    distances[(label,(y,x))] <- dist
            seen.Add (y,x) |> ignore
            for (dy,dx) in [(-1,0);(1,0);(0,-1);(0,1)] do
                let ny, nx = y+dy, x+dx
                if not (seen.Contains (ny,nx)) && path.ContainsKey (ny,nx) then
                    newBoundary.Add (ny,nx) |> ignore
        dist <- dist + 1
        boundary <- newBoundary
    distances

// follow route
let followRoute (route: string list) (distances: Dictionary<(string*Coord), Dictionary<(string*Coord),int>>) =
    let cur =
        distances
        |> Seq.choose (fun kvp ->
            let (c,loc) = kvp.Key
            if c = route.Head then Some((c,loc),0) else None)
        |> dict
    let mutable cur = Dictionary cur
    for target in route.Tail do
        let dests =
            distances
            |> Seq.choose (fun kvp ->
                let (c,loc) = kvp.Key
                if c = target then Some((c,loc), Int32.MaxValue) else None)
            |> dict
        let dests = Dictionary dests
        for KeyValue(src,curDist) in cur do
            for KeyValue(dest,_) in dests do
                let d = distances[src][dest]
                dests[dest] <- min dests[dest] (curDist + d)
        cur <- dests
    cur.Values |> Seq.min

let rec permutations lst =
    seq {
        match lst with
        | [] -> yield []
        | _ ->
            for i in 0 .. lst.Length-1 do
                let x = lst[i]
                let rest = lst[0..i-1] @ lst[i+1..]
                for perm in permutations rest do
                    yield x::perm
    }

let findBestRoute (data: string list) (start: string) =
    let poi, path = parseContent data

    // compute distances
    let distances =
        dict [
            for KeyValue(c,locs) in poi do
                for loc in locs ->
                    (c,loc), calculateDistances path loc poi
        ]

    // brute force permutations
    let targets = poi.Keys |> Seq.filter ((<>) start) |> Seq.toList
    let possible = permutations targets |> List.ofSeq
    possible
    |> Seq.map (fun route -> followRoute (start::route@[start]) (Dictionary distances))
    |> Seq.min

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path) |> List.ofArray
    findBestRoute lines "Start"


