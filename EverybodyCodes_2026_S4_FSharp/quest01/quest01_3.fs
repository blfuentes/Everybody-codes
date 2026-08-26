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

    let jumpBackwards pos step =
        let backpos = pos - step
        if backpos < 0 then (false, 0)
        else
            if visited.Contains(backpos) then (false, 0)
            else
                
    let jumpForwards pos step =
        (true, 0)

    let (ending, _, _) =
        steps
        |> Array.fold (fun (pos, side:SIDE, forward) step -> 
            match forward with
            | true ->
                let (move, newPos) = jumpForwards pos step
                let newSide = if side.IsUP then DOWN else UP 
                if move then
                    (newPos, newSide, false)
                else
                    let (move, newPos) = jumpBackwards pos step
                    if move then
                        (newPos, newSide, true)
                    else
                        (pos, side, forward)
                        
            | false ->
                let (move, newPos) = jumpBackwards pos step
                let newSide = if side.IsUP then DOWN else UP 
                if move then
                    (newPos, newSide, true)
                else
                    let (move, newPos) = jumpForwards pos step
                    if move then
                        (newPos, newSide, false)
                    else
                        (pos, side, forward)

            let backjumppos = pos - step
            let backjump = (backjumppos, pos)
            if (backjumppos < 0) || (visited.Contains(backjumppos)) || (crossesJumps backjump) then
                // move forward
                // move forward until I don't cross 
                let mutable newPos = pos + step
                if crossesJumps (pos, newPos) then // I'm trapped within another jump
                    pos
                else
                    while visited.Contains(newPos) do newPos <- newPos + 1
                    if crossesJumps (pos, newPos) then 
                        pos
                    else
                        visited.Add(newPos) |> ignore
                        jumps.Add(pos, newPos) |> ignore
                        newPos
            else
                // move backwards by default
                let newPos = pos - step
                visited.Add(newPos) |> ignore
                jumps.Add(newPos, pos) |> ignore
                newPos
        ) (0, DOWN, false)
    ending
    

let execute() =
    let dances = parseContent (ReadLines path)
    dances |> Seq.sumBy dance