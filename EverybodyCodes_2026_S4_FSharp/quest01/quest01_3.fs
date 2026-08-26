module quest01_3

open EverybodyCodes_2026_S4_FSharp.Modules
open System.Collections.Generic

//let path = "quest01/test_input_03.txt"
//let path = "quest01/test_input_03b.txt"
let path = "quest01/quest01_input_03.txt"

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
    let crossesJumps ((jf, jt): int*int) jumps =
        jumps |> Seq.exists(fun (f, t) -> (f > jf && f < jt && t > jt) || (f < jf && t > jf && t < jt))

    let jumpBackwards pos step (side: SIDE) =
        let backpos = pos - step
        if backpos < 0 then (false, 0)
        elif visited.Contains(backpos) then (false, 0)
        elif crossesJumps (backpos, pos) (if side.IsUP then jumpsUp else jumpsDown) then
            (false, 0)
        else
            (true, backpos)
                
    let jumpForwards pos step (side: SIDE) =
        // Start at pos + step. If that point was already visited,
        // keep moving forward until we find the first new point.
        // Example: if pos = 5 and step = 3, we try 8, then 9, then 10, etc.
        let mutable forwardpos = pos + step
        let jumps = if side.IsUP then jumpsUp else jumpsDown

        // If an old jump crosses over our current position, it blocks us.
        // Example: old jump is from 2 to 10 and we are at 5. We cannot jump past 10,
        // because that new jump would cross the old one.
        let maximumForwardpos =
            jumps
            |> Seq.choose (fun (fromPos, toPos) ->
                if fromPos < pos && pos < toPos then Some toPos else None)
            |> Seq.sort
            |> Seq.tryHead

        // Keep increasing the jump while the landing point is already used,
        // or while the new jump would cross an old one.
        while
            (maximumForwardpos |> Option.forall (fun maximum -> forwardpos <= maximum))
            && (visited.Contains(forwardpos) || crossesJumps (pos, forwardpos) jumps) do
            forwardpos <- forwardpos + 1

        // If we would have to pass the blocking arc, this jump is not allowed,
        // so we skip it and keep trying the next step size.
        match maximumForwardpos with
        | Some maximum when forwardpos > maximum -> (false, 0)
        | _ -> (true, forwardpos)

        // In short: a forward jump is valid only when it lands on a new point
        // and does not cross any old path.
        // If not, we skip this jump and continue with the next one.

    let (ending, _) =
        steps
        |> Array.fold (fun (pos, side:SIDE) step -> 
            let newSide = if side.IsUP then DOWN else UP 
            let (move, newPos) = jumpBackwards pos step side
            if move then
                visited.Add(newPos) |> ignore
                if side.IsUP then
                    jumpsUp.Add(newPos, pos) |> ignore
                else
                    jumpsDown.Add(newPos, pos) |> ignore

                (newPos, newSide)
            else
                let (move, newPos) = jumpForwards pos step side
                if move then
                    visited.Add(newPos) |> ignore
                    if side.IsUP then
                        jumpsUp.Add(pos, newPos) |> ignore
                    else
                        jumpsDown.Add(pos, newPos) |> ignore
                    (newPos, newSide)
                else
                    (pos, side)
        ) (0, DOWN)
    ending
    

let execute() =
    let dances = parseContent (ReadLines path)
    dances |> Seq.sumBy dance