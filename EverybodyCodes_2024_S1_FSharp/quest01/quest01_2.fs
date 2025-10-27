module quest01_part02

open System.Collections.Generic
open System.Text.RegularExpressions
open EverybodyCodes_2024_S1_FSharp.Modules
open System

//let path = "quest01/test_input_02.txt"
let path = "quest01/quest01_input_02.txt"

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
    let output =
        [0L .. 4L]
        |> List.map (fun i -> 
            let value = modPow n (exp - i) mod'
            string value)
        |> String.concat ""
        |> int64
    output

// eni(A, X, M) + eni(B, Y, M) + eni(C, Z, M) 
let applyFormula(p: ParameterSet) =
    // eni(4,3,11) + eni(4,4,11) + eni(6,5,11)
    //printfn "eni(%d,%d,%d) + eni(%d,%d,%d) + eni(%d,%d,%d)" p.A p.X p.M p.B p.Y p.M p.C p.Z p.M
    eni p.A p.X p.M + eni p.B p.Y p.M + eni p.C p.Z p.M

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let parameters = parseContent lines
    //eni 3 8 16
    parameters
    |> Seq.mapi(fun idx p -> 
        //printfn "Parameter %d from %d" idx parameters.Length
        applyFormula p)
    |> Seq.max

