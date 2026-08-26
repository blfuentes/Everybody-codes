module quest01_3

open EverybodyCodes_2026_S4_FSharp.Modules
open System.Collections.Generic

let path = "quest01/test_input_03.txt"
//let path = "quest01/quest01_input_03.txt"

let parseContent (lines: string seq) =
    lines
    |> Seq.map(fun line -> line.Split(',') |> Array.map int)

type SIDE =
    | UP
    | DOWN

let dance (steps: int array) =
    let visited = new HashSet<int>()
    let jumpsUp = new HashSet<int*int>()
    let jumpsDown = new HashSet<int*int>()

    visited.Add(0) |> ignore
    let crossesJumps ((jf, ft): int*int) jumps =
        jumps |> Seq.exists(fun (f, t) -> f <= jf && t >= jf && t < ft)

    let jumpBackwards pos step (side: SIDE) =
        let backpos = pos - step
        if backpos < 0 then (false, 0)
        elif visited.Contains(backpos) then (false, 0)
        //elif crossesJumps (backpos, pos) (if side.IsUP then jumpsUp else jumpsDown) then
        //    (false, 0)
        else
            (true, backpos)
                
    let jumpForwards pos step (side: SIDE) =
        let mutable forwardpos = pos + step
        while visited.Contains forwardpos do forwardpos <- forwardpos + 1
        (true, forwardpos)

    let (ending, _) =
        steps
        |> Array.fold (fun (pos, side:SIDE) step -> 
            let newSide = if side.IsUP then DOWN else UP 
            let (move, newPos) = jumpBackwards pos step side
            if move then
                visited.Add(newPos) |> ignore
                (newPos, newSide)
            else
                let (move, newPos) = jumpForwards pos step side
                if move then
                    visited.Add(newPos) |> ignore
                    (newPos, newSide)
                else
                    (pos, side)
        ) (0, DOWN)
    ending
    

let execute() =
    let dances = parseContent (ReadLines path)
    dances |> Seq.sumBy dance