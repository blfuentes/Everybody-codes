module quest12_part02

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest12/test_input_02.txt"
let path = "quest12/quest12_input_02.txt"

type Position = {
    Row: int
    Col: int
}

type Target = {
    Life: int
    Pos: Position
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
                    match ch with
                    | 'T' -> 
                        yield {
                            Life = 1;
                            Pos= {Row = rIdx; Col = cIdx}
                        }
                    | 'H' -> 
                        yield {
                            Life = 2;
                            Pos= {Row = rIdx; Col = cIdx}
                        }
                    | c when c <> '.' && c <> '=' ->
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
                    | _ ->
                        // do nothing
                        ()
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

let destroyTargets (targets: Target list) =
    let tower = towers.Values |> Seq.head
    let floorline = (tower.Segments |> List.head).Pos.Row + 1
    let rec destroy (toDestroy: Target list) (value: int) =
        match toDestroy with
        | [] ->
            value
        | head :: tail ->
            let mutable power = 1
            let mutable destroyed = false
            let mutable newValue = value
            let mutable life = head.Life
            while not destroyed do
                let hits = possibleHits tower power floorline
                let found = hits |> List.tryFind (fun (_, positions) -> positions |> List.contains head.Pos)
                match found with
                | None -> power <- power + 1
                | Some (segment, _) -> 
                    newValue <- newValue + (segment.Ranking * power); 
                    life <- life - 1
                    destroyed <- life = 0

            destroy tail newValue

    destroy targets 0

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let targets = parseContent lines |> List.sortBy (fun tar -> tar.Pos.Row, tar.Pos.Col)
    destroyTargets targets
