module quest17_3

open EverybodyCodes_2024_FSharp.Modules

let path = "quest17/quest17_input_03.txt"

type Star = { Id: int; X: int; Y: int }

let manhattanDistance (star1: Star) (star2: Star) =
    abs(star1.X - star2.X) + abs(star1.Y - star2.Y)

let parseContent (lines: string array) =
    lines
    |> Array.mapi (fun y line ->
        line.ToCharArray()
        |> Array.mapi (fun x char -> if char = '*' then Some(x, y) else None)
    )
    |> Array.concat
    |> Array.choose id
    |> Array.mapi (fun i (x, y) -> { Id = i; X = x; Y = y })
    |> Array.toList

let crashConstellations (constellations: list<list<int>>) (dists: int[,]) (getLinks: bool) (threshold: int) =
    let mutable cons = constellations
    let mutable links = []
    let mutable i = 0
    while cons.Length > 1 && i < cons.Length do
        let curr = cons.Head
        let rest = cons.Tail

        let mutable globalMin = System.Int32.MaxValue
        let mutable mergeWithId = -1
        let mutable globalLinkTo = -1
        let mutable globalLinkFrom = -1

        for conId in 0 .. rest.Length - 1 do
            let otherCon = rest[conId]
            let mutable minBetweenCons = System.Int32.MaxValue
            let mutable linkTo = -1
            let mutable linkFrom = -1

            for s1 in curr do
                for s2 in otherCon do
                    let d = dists[s1, s2]
                    if d < minBetweenCons then
                        minBetweenCons <- d
                        linkTo <- s2
                        linkFrom <- s1
            
            if minBetweenCons < globalMin then
                globalMin <- minBetweenCons
                globalLinkTo <- linkTo
                globalLinkFrom <- linkFrom
                mergeWithId <- conId
        
        if mergeWithId <> -1 && globalMin < threshold then
            let mergedList = rest[mergeWithId] @ curr
            let updatedRest = rest |> List.removeAt mergeWithId
            cons <- mergedList :: updatedRest
            i <- 0 // Restart the scan
        else
            cons <- rest @ [curr]
            i <- i + 1
        
        if globalLinkFrom <> -1 then
            links <- (globalLinkFrom, globalLinkTo) :: links
    
    if getLinks then Choice1Of2 links else Choice2Of2 cons

let rec findConstellations (stars: Star list) (p3: bool) (allStarsDists: int[,]) =
    let starIds = stars |> List.map(fun s -> s.Id) |> Set.ofList

    if not p3 then
        let initialConstellations = stars |> List.map (fun s -> [s.Id])
        match crashConstellations initialConstellations allStarsDists true System.Int32.MaxValue with
        | Choice1Of2 links ->
            let mstWeight =
                links
                |> List.filter (fun (a, b) -> starIds.Contains(a) && starIds.Contains(b))
                |> List.distinct
                |> List.sumBy (fun (a, b) -> allStarsDists[a, b])
            int64(stars.Length + mstWeight)
        | _ -> failwith "Expected links"
    else
        let initialConstellations = stars |> List.map (fun s -> [s.Id])
        match crashConstellations initialConstellations allStarsDists false 6 with
        | Choice2Of2 finalConstellations ->
            let sizes =
                finalConstellations
                |> List.map (fun conIds ->
                    let conStars = conIds |> List.map (fun id -> stars |> List.find (fun s -> s.Id = id))
                    findConstellations conStars false allStarsDists
                )
            
            let topThree =
                sizes
                |> List.sortDescending
                |> List.take 3
            
            if topThree.Length < 3 then 0L
            else topThree |> List.map int64 |> List.reduce (*)
        | _ -> failwith "Expected constellations"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let allStars = parseContent lines

    let allDists = Array2D.create allStars.Length allStars.Length 0
    for s1 in allStars do
        for s2 in allStars do
            allDists[s1.Id, s2.Id] <- manhattanDistance s1 s2

    findConstellations allStars true allDists
