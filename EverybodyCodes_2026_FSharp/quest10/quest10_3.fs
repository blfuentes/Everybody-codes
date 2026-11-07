module quest10_3

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open quest10_utilities

// let path = "quest10/test_input_03.txt"
let path = "quest10/quest10_input_03.txt"

let nullghosts = Dictionary<int, Nullghost>()
let walls = HashSet<(int*int)>()

let mutable (maxRow, maxCol) = (0, 0)

let parseContent (lines: string array) defaultLife =
    nullghosts.Clear()
    walls.Clear()
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    let mutable id = 0
    for r in 0..maxRow do
        for c in 0..maxCol do
            match lines[r][c] with
            | '.' -> ignore()
            | '#' -> walls.Add((r,c)) |> ignore
            | _ -> nullghosts.Add(id, { Id = id; Pos=(r, c); Dir = lines[r][c]; Life = defaultLife })
                   id <- id + 1

let round() : int =
    let moveGhosts() : int =
        let mutable collisions = 0
        for kvp in nullghosts do
            let (key, ghost) = (kvp.Key, kvp.Value)
            let newPos =
                move (movOfDir ghost.Dir, ghost.Pos)
            if not (walls.Contains newPos) then
                nullghosts[key] <-  { ghost with Pos = newPos }
            else
                let newDir = rotate90 ghost.Dir
                let newLife = ghost.Life - 1I
                if newLife = 0I then
                    nullghosts.Remove(key) |> ignore
                else
                    nullghosts[key] <- { ghost with Life = newLife; Dir = newDir }
                collisions <- collisions + 1
        collisions
    let countCollisions() =
        let groupedCollided =
            nullghosts
            |> Seq.groupBy _.Value.Pos
            |> Seq.filter(fun (_, ghosts) -> ghosts |> Seq.length > 1)
            |> Seq.map snd
        let collisions =
            groupedCollided
            |> Seq.sumBy Seq.length
        for group in groupedCollided do
            for kvp in group do
                let (key, ghost) = (kvp.Key, kvp.Value)
                let newLife = nullghosts[key].Life - 1I
                if newLife = 0I then
                    nullghosts.Remove(key) |> ignore
                else
                    nullghosts[key] <- { ghost with Life = newLife; Dir = (rotate180 ghost.Dir) }
        collisions
    let moveCollisions = moveGhosts()
    let ghostCollisions = countCollisions()
    //printfn "Wall collisions: %d - Ghosts collisions: %d" moveCollisions ghostCollisions
    moveCollisions + ghostCollisions

let start () : bigint =
    let seen = Dictionary<(int * (int * int) * char) list, bigint * bigint array>()
    let mutable rounds = 0I
    while nullghosts.Count > 0 do
        let ghosts =
            nullghosts.Values
            |> Seq.sortBy _.Id
            |> Seq.toArray
        let state =
            ghosts
            |> Array.map (fun ghost -> (ghost.Id, ghost.Pos, ghost.Dir))
            |> Array.toList

        match seen.TryGetValue state with
        | true, (previousRound, previousLives) ->
            let losses =
                ghosts
                |> Array.mapi (fun i ghost -> previousLives[i] - ghost.Life)
            // Stop before any dies to simulate their fully round
            let limits =
                ghosts
                |> Array.mapi (fun i ghost ->
                    if losses[i] > 0I then Some ((ghost.Life - 1I) / losses[i])
                    else None)
                |> Array.choose id
            //if limits.Length = 0 then
            //    failwith "remaining repeats without collision???"

            let cycles = Array.min limits
            for i in 0 .. ghosts.Length - 1 do
                let ghost = ghosts[i]
                nullghosts[ghost.Id] <- { ghost with Life = ghost.Life - cycles * losses[i] }
            rounds <- rounds + cycles * (rounds - previousRound)
            seen.Clear()
        | false, _ ->
            seen.Add(state, (rounds, ghosts |> Array.map _.Life))

        //let prev = nullghosts.Count
        let _ = round()
        rounds <- rounds + 1I
        //let newRemaining = nullghosts.Count
        //if prev <> newRemaining then
        //    seen.Clear()
        //    printfn "Round: %A" rounds
        //    printfn "Nullghosts remaining: %d" nullghosts.Count
    rounds

let execute() =
    let lines = LocalHelper.GetContent path 2026 10 3
    let defaultLife = 1000000000000I
    parseContent lines defaultLife
    start()
