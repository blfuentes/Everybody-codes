module quest11_1

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open System.Text.RegularExpressions

//let path = "quest11/test_input_01.txt"
let path = "quest11/quest11_input_01.txt"

let Connections = Dictionary<int, ResizeArray<int>>()

let ensureNode key value (d: Dictionary<int, ResizeArray<int>>) =
    if d.ContainsKey(key) then
        d[key].Add(value)
    else
        d.Add(key, new ResizeArray<int>([value]))

let parseContent (lines: string array) =
    let numOfPortals = int(Regex.Match(lines[0], @"\d+").Value)
    lines
    |> Seq.skip(2)
    //|> Seq.filter (fun line -> line.Length > 1)
    |> Seq.iter(fun line ->
        let numbers = Regex.Matches(line, @"\d+") |> Seq.map (fun m -> int(m.Value)) |> Array.ofSeq
        ensureNode numbers[0] numbers[1] Connections
        ensureNode numbers[1] numbers[0] Connections
    )
    for p in 1..numOfPortals do  
        ensureNode p ((p+1) % numOfPortals + (if p + 1 = numOfPortals then numOfPortals else 0)) Connections
        ensureNode p (((p-1) % numOfPortals) + (if (p - 1) = 0 then numOfPortals else 0)) Connections

    numOfPortals

let distanceSum n =
    (n * n) / 4

let minTransport (portalStart: int) (portals: int) =
    let toVisit = Queue<int*int>()
    let visited = HashSet<int>()
    
    let jumpsForEnd = Array.zeroCreate<int>(portals)

    //let jumpNoExtra = distanceSum portals * portalStart

    for portalEnd in 1..portals do
        if portalEnd <> portalStart then
            toVisit.Clear()
            visited.Clear()
            let finalJump = 
                toVisit.Enqueue((portalStart, 0))
                visited.Add(portalStart) |> ignore
     
                let mutable currentJump = Unchecked.defaultof<int * int>
                let mutable finalJump = 0
                while toVisit.TryDequeue(&currentJump) && finalJump = 0 do
                    let (current, jump) = currentJump
                    if current = portalEnd then
                        finalJump <- jump
                    else
                        visited.Add(current) |> ignore
                        Connections[current]
                        |> Seq.iter(fun c ->
                            if not (visited.Contains c) then
                                toVisit.Enqueue((c, jump + 1))
                        )
                finalJump
            jumpsForEnd[portalEnd-1] <- finalJump
    jumpsForEnd |> Array.sum

let execute() =
    let lines = LocalHelper.GetContent path 2026 11 1
    let numOfPortals = parseContent lines
    let nomin = distanceSum numOfPortals * numOfPortals
    nomin - ([1..numOfPortals] |> Seq.sumBy(fun p ->  minTransport p numOfPortals))
