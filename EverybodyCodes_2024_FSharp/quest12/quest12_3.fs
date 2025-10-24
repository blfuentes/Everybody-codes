module quest12_part03

open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest12/test_input_03.txt"
let path = "quest12/quest12_input_03.txt"

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
    lines
    |> Array.map (fun line ->
        { Life = 1; Pos = { Row = int(line.Split(" ")[1]); Col = int(line.Split(" ")[0]) } }
    ) |> Array.toList
    
let positionsMemory = new Dictionary<int * int, Segment list>()


//let possibleHits (tower: Tower) (power: int) (floorline: int) =
//    let hitForSegment (segment: Segment) =
//        let goingUp = 
//            [1..power]
//            |> List.map (fun p -> { Row = segment.Pos.Row - p; Col = segment.Pos.Col + p})
//        let middle = 
//            [1..power]
//            |> List.map (fun p -> { Row = segment.Pos.Row - power; Col = segment.Pos.Col + p + power})
//        let mutable height = segment.Pos.Row - power
//        let mutable column = 1
//        let goingDown =
//            seq {
//                while height < floorline do
//                    yield { Row = height + 1; Col = segment.Pos.Col + (2 * power) + column}
//                    height <- height + 1
//                    column <- column + 1
//            } |> Seq.toList
//        (segment, goingUp @ middle @ goingDown)
//    tower.Segments
//    |> List.map hitForSegment

let possibleHits (tower: Tower) (power: int) (time: int) (floorline: int) =
    if positionsMemory.ContainsKey(power, time) then
        positionsMemory[(power, time)]
    else
        let segmentPositions = 
            seq {
                for segment in tower.Segments do
                    let t = time
                    let p = power

                    // Phase 1: Up-Right movement
                    let upRightSteps = min t p
                    let deltaRowUp = upRightSteps
                    let deltaColUp = upRightSteps

                    // Phase 2: Horizontal movement
                    let rightSteps = max 0 (min (t - p) p)
                    let deltaRowRight = 0
                    let deltaColRight = rightSteps

                    // Phase 3: Down-Right movement
                    let downRightSteps = max 0 (t - 2 * p)
                    let deltaRowDown = -downRightSteps
                    let deltaColDown = downRightSteps

                    let finalRow = segment.Pos.Row + deltaRowUp + deltaRowRight + deltaRowDown
                    let finalCol = segment.Pos.Col + deltaColUp + deltaColRight + deltaColDown
                    
                    if finalRow >= floorline then
                        yield { segment with Pos = { Row = finalRow; Col = finalCol } }
                    else
                        // If below floorline, yield a position that won't match any target
                        yield { segment with Pos = { Row = -1; Col = -1 } }
                        
            } |> Seq.toList
        positionsMemory.Add((power, time), segmentPositions)
        segmentPositions

let destroyTargets (tower: Tower) (targets: Target list) =
    let floorline = -1
    let rec destroy (toDestroy: Target list) (value: int) =
        match toDestroy with
        | [] ->
            value
        | head :: tail ->
            printfn "Destroying target at Row %i Col %i" head.Pos.Row head.Pos.Col
            let mutable counterTime = 0
            let mutable doContinue = true
            let mutable toBeHit = head
            let mutable currentValue = 0
            let mutable bestRanking = 4
            let mutable power = 1
            let maxPower = 250
            while doContinue do
                power <- 1
                if counterTime % 500 = 0 then
                    printfn "\nPower %i Time %i Target at Row %i Col %i" power counterTime toBeHit.Pos.Row toBeHit.Pos.Col
                while power <= maxPower do
                    //if power % 1000 = 0 then 
                    let pHits = possibleHits tower power counterTime floorline
                    //printfn "Possible hits: %A" (pHits |> List.map (fun s -> sprintf "%s at Row %i Col %i" s.Name s.Pos.Row s.Pos.Col))
                    let found = pHits |> List.filter (fun segment -> segment.Pos = toBeHit.Pos)
                    match found with
                    | [] -> 
                        power <- power + 1
                    | _ ->
                        let best = found |> List.minBy (fun segment -> segment.Ranking)
                        if best.Ranking < bestRanking then
                            bestRanking <- best.Ranking
                            currentValue <- bestRanking * power
                        power <- power + 1
                if power > maxPower then
                    counterTime <- counterTime + 1
                    toBeHit <- { toBeHit with Pos = { Row = toBeHit.Pos.Row - 1; Col = toBeHit.Pos.Col - 1 } }   
                    doContinue <- toBeHit.Pos.Col > 0 && toBeHit.Pos.Row >= 0

              
                            
            destroy tail (value + currentValue)            

    destroy targets 0

//let destroyTargets (tower: Tower) (targets: Target list) =
//    let floorline = -1
//    let rec destroy (toDestroy: Target list) (value: int) =
//        match toDestroy with
//        | [] ->
//            value
//        | head :: tail ->
//            let mutable power = 1
//            let mutable destroyed = false
//            let mutable newValue = value
//            let mutable timer = 0
//            let mutable toBeHit = head
//            let mutable oldHits = []
//            let mutable currentRanking = 4
//            let mutable currentValue = 0
//            while not destroyed do
//                let pHits = possibleHits tower power timer floorline
//                if oldHits = pHits then
//                    timer <- timer + 1
//                    power <- 1
//                    toBeHit <- { toBeHit with Pos = { Row = toBeHit.Pos.Row - 1; Col = toBeHit.Pos.Col - 1 } }
//                else
//                    oldHits <- pHits

//                    if pHits.Length = 0 then
//                        power <- power + 1
//                    else
//                        if pHits |> List.forall (fun segment -> segment.Pos.Col > toBeHit.Pos.Col) then
//                            timer <- timer + 1
//                            power <- 1
//                            toBeHit <- { toBeHit with Pos = { Row = toBeHit.Pos.Row - 1; Col = toBeHit.Pos.Col - 1 } }
//                        else
//                            let found = pHits |> List.tryFind (fun segment -> segment.Pos = toBeHit.Pos)
//                            match found with
//                            | None ->
//                                power <- power + 1                
//                            | Some segment ->
//                                if currentRanking > segment.Ranking then
//                                    currentRanking <- segment.Ranking
//                                    currentValue <- currentRanking * power
//                                power <- power + 1                                                                
//                                if destroyed then
//                                    newValue <- newValue + currentValue
//                                    currentRanking <- 4
//                                    currentValue <- 0
//                destroyed <- toBeHit.Pos.Col <= 0 || toBeHit.Pos.Row <= 0

//            destroy tail newValue

//    destroy targets 0

//let destroyTargets (tower: Tower) (targets: Target list) =
//    let floorline = (tower.Segments |> List.head).Pos.Row + 1
//    let rec destroy (toDestroy: Target list) (value: int) =
//        match toDestroy with
//        | [] ->
//            value
//        | head :: tail ->
//            let mutable power = 1
//            let mutable destroyed = false
//            let mutable newValue = value
//            let mutable life = head.Life
//            while not destroyed do
//                let hits = possibleHits tower power floorline
//                let found = hits |> List.tryFind (fun (_, positions) -> positions |> List.contains head.Pos)
//                match found with
//                | None -> power <- power + 1
//                | Some (segment, _) -> 
//                    newValue <- newValue + (segment.Ranking * power); 
//                    life <- life - 1
//                    destroyed <- life = 0

//            destroy tail newValue

//    destroy targets 0

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let meteors = parseContent lines
    let tower = {
        Segments = [
            {
                Name = "C";
                Ranking = 3;
                Pos = {Row = 2; Col = 0}
            };
            {
                Name = "B";
                Ranking = 2;
                Pos = {Row = 1; Col = 0}
            };
            {
                Name = "A";
                Ranking = 1;
                Pos = {Row = 0; Col = 0}
            }
        ]
     }
    destroyTargets tower meteors
