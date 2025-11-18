module quest11_3

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic

//let path = "quest11/test_input_03.txt"
let path = "quest11/quest11_input_03.txt"

let parseContent (lines: string array) =
    lines |> Array.mapi (fun i v -> (i, bigint.Parse(v)))

// BRUTE FORCE RUNNING AROUND ~5 hours
//let doFormation (ducks: (int*bigint) array) =
//    let inline ducksToString (ducks: (int*bigint) array) =
//        ducks
//        |> Array.map (fun (_, v) -> v.ToString())
//        |> String.concat ","
//    let inline ducksBalanced (ducks: (int*bigint) array) =
//        ducks 
//        |> Array.pairwise
//        |> Array.forall (fun ((_, left), (_, right)) -> left = right)

//    // Phase 1: Left to Right until no more moves possible
//    let rec phaseOne(seen: HashSet<string>) (currentRound: bigint)=
//        if seen.Contains(ducksToString ducks) then
//            currentRound - 1I
//        else
//            seen.Add(ducksToString ducks) |> ignore
//            for dIdx in 0.. ducks.Length - 2 do
//                let (_, left), (_, right) = ducks[dIdx], ducks[dIdx + 1]
//                if left > right then
//                    ducks[dIdx] <- (fst ducks[dIdx], (snd ducks[dIdx]) - 1I)
//                    ducks[dIdx + 1] <- (fst ducks[dIdx + 1], (snd ducks[dIdx + 1]) + 1I)
//            phaseOne seen (currentRound + 1I)

//    // Phase 2: Move from more to less until no more moves possible
//    let rec phaseTwo(currentRound: bigint) =
//        if currentRound % 10000I = 0I then
//            printfn "Phase 2 - Current Round: %A" currentRound

//        if ducksBalanced ducks then
//            currentRound
//        else
//            let seen = HashSet<string>()
//            while not (seen.Contains(ducksToString ducks)) do
//                for dIdx in 0.. ducks.Length - 2 do
//                    let (_, left), (_, right) = ducks[dIdx], ducks[dIdx + 1]
//                    if right > left then
//                        ducks[dIdx] <- (fst ducks[dIdx], (snd ducks[dIdx]) + 1I)
//                        ducks[dIdx + 1] <- (fst ducks[dIdx + 1], (snd ducks[dIdx + 1]) - 1I)
//                seen.Add(ducksToString ducks) |> ignore
//            phaseTwo (currentRound + 1I)

//    let phase1End = phaseOne (new HashSet<string>()) 0I
//    let balancedRound = phaseTwo phase1End
//    balancedRound

let findBalancedRound(ducks: (int*bigint) array) =
    let totalDucks =
        ducks
        |> Array.sumBy (fun (_, v) -> v)
    let numberOfDucks = bigint ducks.Length
    let balancedValue = totalDucks / numberOfDucks
    let roundsNeededPerDuck =
        ducks
        |> Array.sumBy(fun (_, v) ->
            if v > balancedValue then
                v - balancedValue
            else
                0I
        )
        
    roundsNeededPerDuck

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let ducksFormed = parseContent lines
    findBalancedRound ducksFormed
