module quest10_2

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open quest10_utilities

// let path = "quest10/test_input_02.txt"
let path = "quest10/quest10_input_02.txt"

let nullghosts = Dictionary<int, Nullghost>()
let walls = HashSet<(int*int)>()

let mutable (maxRow, maxCol) = (0, 0)

let parseContent (lines: string array) defaultLife =
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

let start () : int =
    let mutable rounds = 1
    while nullghosts.Count > 0 do
        // let prev = nullghosts.Count
        let _ = round()
        // let newRemainign = nullghosts.Count
        // if prev <> newRemainign then
        //     printfn "Round: %d" rounds
        //     printfn "Nullghosts remaining: %d" nullghosts.Count
        rounds <- rounds + 1
        // collisions <- collisions + newCollisions
    rounds - 1

let execute() =
    let lines = LocalHelper.GetContent path 2026 10 1
    let defaultLife = 10000I
    parseContent lines defaultLife
    start()
