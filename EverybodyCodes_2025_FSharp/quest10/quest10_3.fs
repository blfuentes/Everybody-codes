module quest10_3

open EverybodyCodes_2025_FSharp.Modules

let path = "quest10/test_input_03.txt"
//let path = "quest10/quest10_input_03.txt"

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
    Dragons: Set<Position>;
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
        Dragons = characters |> List.filter (fun c -> c.Kind = Dragon) |> List.map (fun c -> c.Pos) |> Set.ofList;
        Sheeps = characters |> List.filter (fun c -> c.Kind = Sheep) |> List.map (fun c -> c.Pos) |> Set.ofList;
        Hideouts = characters |> List.filter (fun c -> c.Kind = Hideout) |> List.map (fun c -> c.Pos) |> Set.ofList;
    }

let printFieldMap (fieldMap: FieldMap) =
    for y in 0 .. fieldMap.Height - 1 do
        let line =
            [ for x in 0 .. fieldMap.Width - 1 do
                let ch =
                    if fieldMap.Dragons |> Set.contains { X = x; Y = y } then
                        'D'
                    elif fieldMap.Sheeps |> Set.contains { X = x; Y = y } then
                        'S'
                    elif fieldMap.Hideouts |> Set.contains { X = x; Y = y } then
                        'H'
                    else
                        '#'
                yield ch
            ] |> System.String.Concat
        printfn "%s" line

let findNewPositions (character: Character) : Position list =
    let deltas = 
        if character.Kind.IsDragon then
            [(-2, -1); (-2, 1); (2, -1); (2, 1); (-1, -2); (-1, 2); (1, -2); (1, 2)]
        else
            [(0, 1)]            
    
    deltas
    |> List.map (fun (dx, dy) -> { X = character.Pos.X + dx; Y = character.Pos.Y + dy })
    

let haunt (fieldMap: FieldMap): Set<Position> =
    Set.difference (Set.intersect fieldMap.Dragons fieldMap.Sheeps) fieldMap.Hideouts 

let moveAndHaunt (fieldMap: FieldMap) (numOfMoves: int): int =
    let rec moveCharacter (currentMap: FieldMap) (movesLeft: int) (haunted: int) : int =
        if movesLeft = 0 then
            haunted
        else
            // find new positions of the dragon
            let newDragonPositions = 
                currentMap.Dragons
                |> Seq.collect (fun pos -> 
                    findNewPositions({ Pos = pos; Kind = Dragon })
                    |> List.filter (fun newPos -> 
                        newPos.X >= 0 && newPos.X < currentMap.Width &&
                        newPos.Y >= 0 && newPos.Y < currentMap.Height
                    )
                ) |> Set.ofSeq
            
            let dragonMap = { currentMap with Dragons = newDragonPositions }
            
            // find haunted sheeps on dragon movement
            let hauntedByDragon = haunt dragonMap
            
            let sheepsAfterDragonHaunt =
                Set.difference currentMap.Sheeps hauntedByDragon
            
            let sheepMap = { dragonMap with Sheeps = sheepsAfterDragonHaunt }

            // find new positions of the sheeps
            let newSheepPositions =
                sheepMap.Sheeps
                |> Seq.collect (fun pos ->
                    findNewPositions({ Pos = pos; Kind = Sheep })
                    |> List.filter (fun newPos ->
                        newPos.X >= 0 && newPos.X < sheepMap.Width &&
                        newPos.Y >= 0 && newPos.Y < sheepMap.Height
                    )
                ) |> Set.ofSeq
            let sheepMapAfterMove = { sheepMap with Sheeps = newSheepPositions }

            // find haunted sheeps on sheep movement
            let hauntedBySheep = haunt sheepMapAfterMove
            let totalHaunted = haunted + hauntedByDragon.Count + hauntedBySheep.Count

            let sheepsAfterSheepHaunt =
                Set.difference sheepMapAfterMove.Sheeps hauntedBySheep

            let newMap = { sheepMapAfterMove with Sheeps = sheepsAfterSheepHaunt }
            moveCharacter newMap (movesLeft - 1) totalHaunted            

    let positions = moveCharacter fieldMap numOfMoves 0
    positions


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let fieldMap = parseContent(lines)
    moveAndHaunt fieldMap 20
