module quest10_1

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest10/test_input_01.txt"
let path = "quest10/quest10_input_01.txt"

type CellType =
    | Empty
    | Sheep
    | Dragon

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
    Characters: Character list
}

let parseContent(lines: string array) =
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
                | _ -> None
            )
            |> Array.choose id
        )
        |> Array.concat
        |> Array.toList
    {
        Width = maxX;
        Height = maxY;
        Characters = characters
    }

let printFieldMap(fieldMap: FieldMap) =
    for y in 0 .. fieldMap.Height - 1 do
        let line =
            [ for x in 0 .. fieldMap.Width - 1 do
                let ch =
                    match fieldMap.Characters |> List.tryFind (fun c -> c.Pos.X = x && c.Pos.Y = y) with
                    | Some character ->
                        match character.Kind with
                        | Sheep -> 'S'
                        | Dragon -> 'D'
                    | None -> '.'
                yield ch
            ] |> System.String.Concat
        printfn "%s" line

let findNewPositions(pos: Position, fieldMap: FieldMap): Position list =
    let deltas = [(-2, -1); (-2, 1); (2, -1); (2, 1); (-1, -2); (-1, 2); (1, -2); (1, 2)];
    let positions =
        deltas
        |> List.map (fun (dx, dy) -> { X = pos.X + dx; Y = pos.Y + dy })
        |> List.filter (fun pos ->
            pos.X >= 0 && pos.X < fieldMap.Width &&
            pos.Y >= 0 && pos.Y < fieldMap.Height
        )
    positions

let move(fieldMap: FieldMap) (numOfMoves: int): Position list =
    let rec moveCharacter (positions: Position list, movesLeft: int) (visited: Position list): Position list =
        if movesLeft = 0 then
            visited
        else
            let newPositions = 
                positions
                |> List.collect (fun pos -> findNewPositions(pos, fieldMap))
                |> List.filter (fun pos -> not (List.contains pos visited))
            moveCharacter(newPositions, movesLeft - 1) (visited @ newPositions)
    let dragon = fieldMap.Characters |> List.find (fun c -> c.Kind = Dragon)
    let positions = moveCharacter([dragon.Pos], numOfMoves) []
    positions

let haunt(fieldMap: FieldMap) (dragonPositions: Position list) : int =
    let sheepPositions =
        fieldMap.Characters
        |> List.filter (fun c -> c.Kind = Sheep)
        |> List.map (fun c -> c.Pos)
        |> Set.ofList
    let setDragonPositions = Set.ofList dragonPositions
    (Set.intersect sheepPositions setDragonPositions).Count

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let fieldMap = parseContent(lines)
    let dragonPositions = move fieldMap 4
    haunt fieldMap dragonPositions
    //printFieldMap fieldMap
    //fieldMap.Characters.Length
