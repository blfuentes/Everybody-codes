module quest11_2

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest11/test_input_02.txt"
let path = "quest11/quest11_input_02.txt"

let parseContent (lines: string array) =
    lines |> Array.map int

let doFormation (ducks: int array) =
    let inline ducksBalanced (ducks: int array) =
        ducks 
        |> Array.pairwise
        |> Array.forall (fun (left, right) -> left = right)

    let inline areEqual(ducks1: int array) (ducks2: int array) =
        Array.forall2 (=) ducks1 ducks2

    // Phase 1: Left to Right until no more moves possible
    let rec phaseOne(currentRound: int) (lastForm: int array) =
        if areEqual lastForm ducks then
            currentRound - 1
        else
            let lastFormCopy = Array.copy ducks
            for dIdx in 0..ducks.Length - 2 do
                if ducks[dIdx] > ducks[dIdx+1] then
                    ducks[dIdx] <- ducks[dIdx] - 1
                    ducks[dIdx + 1] <- ducks[dIdx + 1] + 1
            phaseOne (currentRound + 1) lastFormCopy

    // Phase 2: Move from more to less until no more moves possible
    let rec phaseTwo (currentRound: int) =
        if ducksBalanced ducks then
            currentRound - 1
        else
            for dIdx in 0..ducks.Length-2 do
                if ducks[dIdx + 1] > ducks[dIdx] then
                    ducks[dIdx] <- ducks[dIdx] + 1
                    ducks[dIdx + 1] <- ducks[dIdx + 1] - 1
            phaseTwo (currentRound + 1)

    let phase1End = phaseOne 1 (Array.zeroCreate ducks.Length)
    let balancedRound = phaseTwo phase1End
    balancedRound

let flockCheckSum(ducks: int array) =
    ducks
    |> Array.mapi (fun idx value -> (idx, value))
    |> Array.sumBy(fun (idx, value) -> (idx + 1) * value)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let ducksFormed = parseContent lines
    doFormation ducksFormed
