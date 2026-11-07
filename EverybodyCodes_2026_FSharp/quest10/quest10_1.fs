module quest10_1

open EverybodyCodes_2026_FSharp.Modules
open System.Collections.Generic
open quest10_utilities

//let path = "quest10/test_input_01.txt"
let path = "quest10/quest10_input_01.txt"

let nullghosts = ResizeArray<Nullghost>()
let walls = HashSet<(int*int)>()

let mutable (maxRow, maxCol) = (0, 0)

let parseContent (lines: string array) =
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    let mutable id = 0
    for r in 0..maxRow do
        for c in 0..maxCol do
            match lines[r][c] with
            | '.' -> ignore()
            | '#' -> walls.Add((r,c)) |> ignore
            | _ -> nullghosts.Add({ Id = id; Pos=(r, c); Dir = lines[r][c]; Life = 0I })
                   id <- id + 1

let round() : int =
    let moveGhosts() : int =
        let mutable collisions = 0
        for i in 0..nullghosts.Count-1 do
            let newPos =  
                move (movOfDir nullghosts[i].Dir, nullghosts[i].Pos)
            if not (walls.Contains newPos) then
                nullghosts[i] <-  { nullghosts[i] with Pos = newPos }
            else
                let newDir = rotate90 nullghosts[i].Dir
                nullghosts[i] <- { nullghosts[i] with Dir = newDir }
                collisions <- collisions + 1
        collisions
    let countCollisions() =
        let groupedCollided = 
            nullghosts 
            |> Seq.groupBy _.Pos
            |> Seq.filter(fun (_, ghosts) -> ghosts |> Seq.length > 1)
            |> Seq.map snd
        let collisions = 
            groupedCollided 
            |> Seq.sumBy Seq.length
        for group in groupedCollided do
            for ghost in group do
                let ghostIdx = nullghosts |> Seq.findIndex(fun g -> g.Id = ghost.Id)
                // nullghosts[ghostIdx] <- restorePos nullghosts[ghostIdx]
                nullghosts[ghostIdx] <- { ghost with Dir = (rotate180 ghost.Dir) }
        collisions
    let moveCollisions = moveGhosts()
    let ghostCollisions = countCollisions()
    //printfn "Wall collisions: %d - Ghosts collisions: %d" moveCollisions ghostCollisions
    moveCollisions + ghostCollisions

let start (rounds: int) : int =
    let mutable collisions = 0
    for i in 1..rounds do
        let newCollisions = round()
        //printfn "Round: %d" i
        //printfn "Collisions: %d" newCollisions
        collisions <- collisions + newCollisions
    collisions

let execute() =
    let lines = LocalHelper.GetContent path 2026 10 1
    parseContent lines
    start 10
