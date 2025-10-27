module quest01_part01

open System.Collections.Generic
open System.Text.RegularExpressions
open EverybodyCodes_2024_S1_FSharp.Modules
open System

//let path = "quest01/test_input_01.txt"
let path = "quest01/quest01_input_01.txt"

type ParameterSet = {
    A: int
    B: int
    C: int
    X: int
    Y: int
    Z: int
    M: int
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
                    |> Seq.map (fun m -> m.Groups.[1].Value, int m.Groups.[2].Value)
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

let eni n exp mod' =
    let mutable value = 1L
    //let remainders = Stack<int64>()
    let mutable result = ""
    for r in 0L..exp-1L do
        value <- (value * n) % mod'
        result <- (string value) + result
        //remainders.Push(value)
    //printfn "%A" (int64(result))
    int64(result)

// eni(A, X, M) + eni(B, Y, M) + eni(C, Z, M) 
let applyFormula(p: ParameterSet) =
    // eni(4,3,11) + eni(4,4,11) + eni(6,5,11)
    //printfn "eni(%d,%d,%d) + eni(%d,%d,%d) + eni(%d,%d,%d)" p.A p.X p.M p.B p.Y p.M p.C p.Z p.M
    eni p.A p.X p.M + eni p.B p.Y p.M + eni p.C p.Z p.M

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let parameters = parseContent lines
    parameters
    |> Seq.map applyFormula
    |> Seq.max
