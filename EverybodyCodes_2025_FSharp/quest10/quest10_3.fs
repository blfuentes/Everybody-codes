module quest10_3

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Text
open System.Numerics

//let path = "quest10/test_input_03.txt"
let path = "quest10/quest10_input_03.txt"

type CellType =
    | Empty
    | Sheep
    | Dragon
    | Hideout

type Position = {
    X: int;
    Y: int
}

type Character = {
    Pos: Position;
    Kind: CellType
}

type FieldMap = {
    Width: int;
    Height: int;
    Dragon: Position;
    Sheeps: Set<Position>;
    Hideouts: Set<Position>;
}

let parseContent (lines: string array) =
    let maxX = lines[0].Length
    let maxY = lines.Length
    let characters =
        lines
        |> Array.mapi (fun y line ->
            line.ToCharArray()
            |> Array.mapi (fun x ch ->
                match ch with
                | '.' -> None
                | 'S' -> Some { Pos = { X = x; Y = y } ; Kind = Sheep }
                | 'D' -> Some { Pos = { X = x; Y = y } ; Kind = Dragon }
                | '#' -> Some { Pos = { X = x; Y = y } ; Kind = Hideout }
                | _ -> None
            )
            |> Array.choose id
        )
        |> Array.concat
        |> Array.toList
    {
        Width = maxX;
        Height = maxY;
        Dragon = characters |> List.find (fun c -> c.Kind = Dragon) |> fun c -> c.Pos;
        Sheeps = characters |> List.filter (fun c -> c.Kind = Sheep) |> List.map (fun c -> c.Pos) |> Set.ofList;
        Hideouts = characters |> List.filter (fun c -> c.Kind = Hideout) |> List.map (fun c -> c.Pos) |> Set.ofList;
    }

// for faster hashing
let inline encodePos (p: Position) : bigint = (bigint p.X <<< 32) ||| bigint (uint32 p.Y)

let turnsToEscape (field: FieldMap) : HashSet<bigint> =
    let hs = HashSet<bigint>()
    for x in 0 .. field.Width - 1 do
        // count trailing hideouts from bottom
        let rec countTrailing y acc =
            if y < 0 then acc
            else
                let pos = { X = x; Y = y }
                if field.Hideouts.Contains pos then countTrailing (y - 1) (acc + 1)
                else acc
        let trailing = countTrailing (field.Height - 1) 0
        let height = field.Height - trailing
        hs.Add (encodePos { X = x; Y = height }) |> ignore
    hs

let dragonPositions (field: FieldMap) (initialPost: Position) : Position[] =
    let x = initialPost.X
    let y = initialPost.Y
    let deltas = [|(-2, -1); (-2, 1); (2, -1); (2, 1); (-1, -2); (-1, 2); (1, -2); (1, 2)|]
    let buffer = ResizeArray<Position>(8)
    for (dx, dy) in deltas do
        let nx = x + dx
        let ny = y + dy
        if nx >= 0 && nx < field.Width && ny >= 0 && ny < field.Height then
            buffer.Add({ X = nx; Y = ny })
    buffer.ToArray()

let possibleSheepLocations (sheep: Position[]) (dragonOpt: Position option) : Position[][] =
    let n = sheep.Length
    if n = 0 then [||]
    else
        let results = ResizeArray<Position[]>(n)
        for i in 0 .. n - 1 do
            let shp = sheep[i]
            let moved = { X = shp.X; Y = shp.Y + 1 }
            match dragonOpt with
            | Some d when d = moved -> () // blocked
            | _ ->
                let arr = Array.zeroCreate<Position> n
                for j in 0 .. n - 1 do
                    arr[j] <- (if j = i then moved else sheep[j])
                results.Add arr
        results.ToArray()

let simulate (field: FieldMap) : BigInteger =
    let dragon = field.Dragon
    let sheepArr = field.Sheeps |> Set.toArray
    let hides = field.Hideouts

    // hideouts hashed
    let hidesEncoded = HashSet<bigint>()
    for h in hides do hidesEncoded.Add(encodePos h) |> ignore
    let bottomEscapes = turnsToEscape field

    // cache key of current state of sheeps and dragon
    let cacheKeyOf (sheepArr: Position[]) (dragonPos: Position) : string =
        let sb = StringBuilder(4 * (sheepArr.Length + 1))
        for i in 0 .. sheepArr.Length - 1 do
            let p = sheepArr[i]
            sb.Append(p.X) |> ignore
            sb.Append(',') |> ignore
            sb.Append(p.Y) |> ignore
            if i < sheepArr.Length - 1 then sb.Append('|') |> ignore
        sb.Append(';') |> ignore
        sb.Append(dragonPos.X) |> ignore
        sb.Append(',') |> ignore
        sb.Append(dragonPos.Y) |> ignore
        sb.ToString()

    let cache = Dictionary<string,BigInteger>()

    let rec movementTurn (sheepState: Position[]) (dragonPos: Position) : BigInteger =
        if sheepState.Length = 0 then BigInteger.One
        else
            let key = cacheKeyOf sheepState dragonPos
            match cache.TryGetValue key with
            | true, v -> v
            | _ ->
                let dragonOnHide = hides.Contains dragonPos
                let possible = possibleSheepLocations sheepState (if dragonOnHide then None else Some dragonPos)
                let allDoable =
                    // sheeps cannot move...
                    let states = if possible.Length = 0 then [| sheepState |] else possible
                    // filter out states where any sheep can escape
                    let buf = ResizeArray<Position[]>(states.Length)
                    for st in states do
                        let mutable ok = true
                        let mutable i = 0
                        while i < st.Length && ok do
                            if bottomEscapes.Contains(encodePos st[i]) then
                                ok <- false
                            i <- i + 1
                        if ok then buf.Add(st)
                    buf.ToArray()
                if allDoable.Length = 0 then
                    cache[key] <- BigInteger.Zero
                    BigInteger.Zero
                else
                    let allDragonMoves = dragonPositions field dragonPos
                    let mutable total = BigInteger.Zero
                    for si in 0 .. allDoable.Length - 1 do
                        let sheepAfterMove = allDoable[si]
                        for di in 0 .. allDragonMoves.Length - 1 do
                            let dragonAfterMove = allDragonMoves[di]
                            let dragonAfterEncoded = encodePos dragonAfterMove
                            // if dragon moved into a hideout or didn't land on a sheep, no sheep eaten
                            let sheepAfterEaten : Position[] =
                                if hidesEncoded.Contains dragonAfterEncoded then
                                    sheepAfterMove
                                else
                                    // sheeps eaten by dragon??
                                    let mutable found = false
                                    for k in 0 .. sheepAfterMove.Length - 1 do
                                        if sheepAfterMove[k] = dragonAfterMove then found <- true
                                    if not found then sheepAfterMove
                                    else
                                        // filter out eaten sheeps
                                        let n = sheepAfterMove.Length
                                        let arr = Array.zeroCreate<Position> (n - 1)
                                        let mutable idx = 0
                                        for k in 0 .. n - 1 do
                                            if not (sheepAfterMove[k] = dragonAfterMove) then
                                                arr[idx] <- sheepAfterMove[k]; idx <- idx + 1
                                        arr
                            total <- total + movementTurn sheepAfterEaten dragonAfterMove
                    cache[key] <- total
                    total
    movementTurn sheepArr dragon

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let fieldMap = parseContent(lines)
    simulate fieldMap    
