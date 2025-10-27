module quest17_part02

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

//let path = "quest17/test_input_02.txt"
let path = "quest17/quest17_input_02.txt"

type Star = {
    Id: int
    X: int
    Y: int
}

let parseContent(lines: string array) =
    let maxRows, maxCols = lines.Length, lines.[0].Length
    let mutable currentStar = 1
    let stars =
        seq {
            for y in 0..maxRows - 1 do
                for x in 0 .. maxCols - 1 do
                    let char = lines[y].[x]
                    if char = '*' then
                        let star = { Id = currentStar; X = x + 1; Y = maxCols - y }
                        currentStar <- currentStar + 1
                        yield star
        } |> Seq.toList
    stars

let manhattanDistance(star1: Star, star2: Star) =
    abs(star2.X - star1.X) + abs(star2.Y - star1.Y)

let distances(stars: Star list) =
    let distances = new Dictionary<Star*Star, int>()
    for star1 in stars do
        for star2 in stars do
            if star1.Id <> star2.Id then
                let distance = manhattanDistance (star1, star2)
                if not (distances.ContainsKey(star1, star2)) then
                    distances.Add((star1, star2), distance)
                if not (distances.ContainsKey((star2, star1))) then
                    distances.Add((star2, star1), distance)
    distances

let printStars(stars: Star list) =
    for star in stars do
        printfn "Star %d at (%d,%d)" star.Id star.X star.Y  
 
let findMinimumSpanningTree(stars: Star list) =
    if List.isEmpty stars then
        0
    else
        let mutable totalDistance = 0
        let visited = HashSet<Star>()
        let pq = new PriorityQueue<(int * Star * Star), int>()

        let startStar = stars.Head
        visited.Add(startStar) |> ignore

        for otherStar in stars do
            if otherStar.Id <> startStar.Id then
                let dist = manhattanDistance(startStar, otherStar)
                pq.Enqueue((dist, startStar, otherStar), dist)

        while visited.Count < stars.Length && pq.Count > 0 do
            let (dist, fromStar, toStar) = pq.Dequeue()

            if not (visited.Contains(toStar)) then
                visited.Add(toStar) |> ignore
                totalDistance <- totalDistance + dist
                //printfn "Connecting Star %d to Star %d with distance %d" fromStar.Id toStar.Id dist

                for otherStar in stars do
                    if not (visited.Contains(otherStar)) then
                        let newDist = manhattanDistance(toStar, otherStar)
                        pq.Enqueue((newDist, toStar, otherStar), newDist)
        
        totalDistance + stars.Length

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let stars = parseContent(lines)
    //printStars stars
    findMinimumSpanningTree stars