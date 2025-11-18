module quest11_1

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest11/test_input_01.txt"
let path = "quest11/quest11_input_01.txt"

let parseContent (lines: string array) =
    lines |> Array.mapi (fun i v -> (i, int v))

let doFormation (ducks: (int*int) array) (numberOfRounds: int) =
    let inline ducksToString (ducks: (int*int) array) =
        ducks
        |> Array.map (fun (_, v) -> v.ToString())
        |> String.concat ","
    // Phase 1: Left to Right until no more moves possible
    let rec phaseOne(seen: HashSet<string>) (currentRound: int)=
        if seen.Contains(ducksToString ducks) then
            currentRound - 1
        else
            seen.Add(ducksToString ducks) |> ignore
            for dIdx in 0.. ducks.Length - 2 do
                let (_, left), (_, right) = ducks[dIdx], ducks[dIdx + 1]
                if left > right then
                    ducks[dIdx] <- (fst ducks[dIdx], (snd ducks[dIdx]) - 1)
                    ducks[dIdx + 1] <- (fst ducks[dIdx + 1], (snd ducks[dIdx + 1]) + 1)
            phaseOne seen (currentRound + 1)

    // Phase 2: Move from more to less until no more moves possible
    let rec phaseTwo(currentRound: int) (stopAt: int)=
        if stopAt = currentRound then
            currentRound - 1
        else
            let seen = HashSet<string>()
            while not (seen.Contains(ducksToString ducks)) do
                for dIdx in 0.. ducks.Length - 2 do
                    let (_, left), (_, right) = ducks[dIdx], ducks[dIdx + 1]
                    if right > left then
                        ducks[dIdx] <- (fst ducks[dIdx], (snd ducks[dIdx]) + 1)
                        ducks[dIdx + 1] <- (fst ducks[dIdx + 1], (snd ducks[dIdx + 1]) - 1)
                seen.Add(ducksToString ducks) |> ignore
            phaseTwo (currentRound + 1) stopAt

    let phase1End = phaseOne (new HashSet<string>()) 0
    phaseTwo phase1End numberOfRounds |> ignore
    ducks

let flockCheckSum(ducks: (int*int) array) =
    ducks
    |> Array.sumBy(fun (idx, value) -> (idx + 1) * value)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let ducksFormed = parseContent lines
    let formedAt = doFormation ducksFormed 10    
    flockCheckSum formedAt