module quest01_3

open EverybodyCodes_2024_S1_FSharp.Modules
open System.Text.RegularExpressions

//let path = "quest01/test_input_03.txt"
let path = "quest01/quest01_input_03.txt"

type ParameterSet = {
    A: int64
    B: int64
    C: int64
    X: int64
    Y: int64
    Z: int64
    M: int64
}

let parseContent(lines: string array) =
    // use of regex to extract each number from A=4 B=4 C=6 X=3 Y=4 Z=5 M=11
    let pattern = @"([A-Z])=(\d+)"
    let regex = Regex(pattern)
    let parameters =
        seq {
            for line in lines do
                let parts = regex.Matches(line)
                let dict =
                    parts
                    |> Seq.cast<Match>
                    |> Seq.map (fun m -> m.Groups.[1].Value, int64 m.Groups.[2].Value)
                    |> dict

                {
                    A = dict["A"]
                    B = dict["B"]
                    C = dict["C"]
                    X = dict["X"]
                    Y = dict["Y"]
                    Z = dict["Z"]
                    M = dict["M"]
                }
        } |> List.ofSeq
    parameters

let modPow (base': int64) (exp: int64) (modulus: int64) =
    let mutable result = 1L
    let mutable b = base' % modulus
    let mutable e = exp
    while e > 0L do
        if e % 2L = 1L then
            result <- (result * b) % modulus
        b <- (b * b) % modulus
        e <- e / 2L
    result


let eni (n: int64) (exp: int64) (mod': int64) =
    let cycles = ResizeArray<int64>()
    let mutable idx = 0L
    let mutable doContinue = true
    while doContinue do
        let toappend = modPow n (exp-idx) mod'
        if cycles.Contains(toappend) then
            doContinue <- false
        else
            cycles.Add(toappend) |> ignore
        idx <- idx + 1L
    let mutable boundary = exp%(int64(cycles.Count))
    while (modPow n boundary mod') <> cycles[0] do
        boundary <- boundary + int64(cycles.Count)

    let numCycles = (exp-boundary)/(int64(cycles.Count))
    (cycles |> Seq.sum) * numCycles + ([1L..boundary] |> Seq.sumBy(fun i -> modPow n i mod'))

// eni(A, X, M) + eni(B, Y, M) + eni(C, Z, M) 
let applyFormula(p: ParameterSet) =
    // eni(4,3,11) + eni(4,4,11) + eni(6,5,11)
    //printfn "eni(%d,%d,%d) + eni(%d,%d,%d) + eni(%d,%d,%d)" p.A p.X p.M p.B p.Y p.M p.C p.Z p.M
    eni p.A p.X p.M + eni p.B p.Y p.M + eni p.C p.Z p.M

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let parameters = parseContent lines
    parameters
    |> Seq.mapi(fun idx p -> 
        applyFormula p)
    |> Seq.max
