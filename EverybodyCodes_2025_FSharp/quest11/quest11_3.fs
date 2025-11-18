module quest11_3

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest11/test_input_03.txt"
let path = "quest11/quest11_input_03.txt"

let parseContent (lines: string array) =
    lines |> Array.map (fun v -> bigint.Parse(v))

// BRUTE FORCE RUNNING AROUND ~2 days
//let doFormation (ducks: bigint array) =
//    let inline ducksBalanced (ducks: bigint array) =
//        ducks 
//        |> Array.pairwise
//        |> Array.forall (fun (left, right) -> left = right)

//    let inline areEqual(ducks1: bigint array) (ducks2: bigint array) =
//        Array.forall2 (=) ducks1 ducks2

//    // Phase 1: Left to Right until no more moves possible
//    let rec phaseOne(currentRound: int) (lastForm: bigint array) =
//        if areEqual lastForm ducks then
//            currentRound - 1
//        else
//            let lastFormCopy = Array.copy ducks
//            for dIdx in 0..ducks.Length - 2 do
//                if ducks[dIdx] > ducks[dIdx+1] then
//                    ducks[dIdx] <- ducks[dIdx] - 1I
//                    ducks[dIdx + 1] <- ducks[dIdx + 1] + 1I
//            phaseOne (currentRound + 1) lastFormCopy

//    // Phase 2: Move from more to less until no more moves possible
//    let rec phaseTwo (currentRound: int) =
//        if currentRound % 100000 = 0 then
//            printfn "Phase2: Round %d" currentRound
//        if ducksBalanced ducks then
//            currentRound - 1
//        else
//            for dIdx in 0..ducks.Length-2 do
//                if ducks[dIdx + 1] > ducks[dIdx] then
//                    ducks[dIdx] <- ducks[dIdx] + 1I
//                    ducks[dIdx + 1] <- ducks[dIdx + 1] - 1I
//            phaseTwo (currentRound + 1)

//    let phase1End = phaseOne 1 (Array.zeroCreate ducks.Length)
//    let balancedRound = phaseTwo phase1End
//    balancedRound

let findBalancedRound(ducks: bigint array) =
    let totalDucks = Array.sum ducks
    let numberOfDucks = bigint ducks.Length
    let balancedValue = totalDucks / numberOfDucks
    let roundsNeededPerDuck =
        ducks
        |> Array.sumBy(fun v ->
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
