module quest12_1

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest12/test_input_01.txt"
let path = "quest12/quest12_input_01.txt"

type Position = {
    Row: int
    Col: int
}

type Segment = {
    Name: string
    Ranking: int
    Pos: Position
}

type Tower = {
    Segments: Segment list
}

let towers = new Dictionary<int, Tower>()

let parseContent(lines: string array) =
    let maxRows = lines.Length
    let rankings = ['A'..'Z'] |> Seq.mapi (fun idx ch -> (string ch, idx + 1)) |> dict
    let targets = 
        seq {
            for rIdx, line in Array.indexed lines do
                for cIdx, ch  in Array.indexed (line.ToCharArray()) do
                    if ch = 'T' then
                        yield {Row = rIdx; Col = cIdx}
                    else if ch <> '.' && ch <> '=' then
                        let segment = {
                            Name = ch.ToString();
                            Ranking = rankings[ch.ToString()];
                            Pos = {Row = rIdx; Col = cIdx}
                         }
                        if towers.ContainsKey(cIdx) then
                            let tower = towers[cIdx]
                            towers[cIdx] <- {Segments = segment :: tower.Segments}
                        else
                            towers.Add(cIdx, {Segments = [segment]})
        } |> Seq.toList
    targets

let possibleHits (tower: Tower) (power: int) (floorline: int) =
    let hitForSegment (segment: Segment) =
        let goingUp = 
            [1..power]
            |> List.map (fun p -> { Row = segment.Pos.Row - p; Col = segment.Pos.Col + p})
        let middle = 
            [1..power]
            |> List.map (fun p -> { Row = segment.Pos.Row - power; Col = segment.Pos.Col + p + power})
        let mutable height = segment.Pos.Row - power
        let mutable column = 1
        let goingDown =
            seq {
                while height < floorline do
                    yield { Row = height + 1; Col = segment.Pos.Col + (2 * power) + column}
                    height <- height + 1
                    column <- column + 1
            } |> Seq.toList
        (segment, goingUp @ middle @ goingDown)
    tower.Segments
    |> List.map hitForSegment

let destroyTargets (targets: Position list) =
    let tower = towers.Values |> Seq.head
    let floorline = (tower.Segments |> List.head).Pos.Row + 1
    let rec destroy (toDestroy: Position list) (value: int) =
        match toDestroy with
        | [] ->
            value
        | head :: tail ->
            let mutable power = 1
            let mutable destroyed = false
            let mutable newValue = value
            while not destroyed do
                let hits = possibleHits tower power floorline
                let found = hits |> List.tryFind (fun (_, positions) -> positions |> List.contains head)
                match found with
                | None -> power <- power + 1
                | Some (segment, _) -> 
                    newValue <- newValue + (segment.Ranking * power); 
                    destroyed <- true

            destroy tail newValue

    destroy targets 0

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let targets = parseContent lines |> List.sortBy (fun pos -> pos.Row, pos.Col)
    destroyTargets targets