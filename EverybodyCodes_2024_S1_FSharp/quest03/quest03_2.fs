module quest03_part02

open EverybodyCodes_2024_S1_FSharp.Modules

//let path = "quest03/test_input_02.txt"
let path = "quest03/quest03_input_02.txt"

type Snail = {
    X: int64
    Y: int64
}

let parseContent (lines: string array) =
    let snails =
        seq {
            for line in lines do
                { 
                    X = int64((line.Split(" ")[0]).Split("=")[1])
                    Y = int64((line.Split(" ")[1]).Split("=")[1])
                }    
        } |> Seq.toList
    snails

// Extended Euclidean Algorithm for modular inverse
let modInverse (a:int64) (m:int64) : int64 =
    let rec egcd (a:bigint) (b:bigint) : bigint*bigint*bigint =
        if b = 0I then (a, 1I, 0I)
        else
            let (g, x, y) = egcd b (a % b)
            (g, y, x - (a / b) * y)
    let (g, x, _) = egcd (bigint a) (bigint m)
    if g <> 1I then failwithf "No modular inverse for %d mod %d" a m
    else ((x % (bigint m) + (bigint m)) % (bigint m) |> int64)

// Recursive reducer (Chinese Remainder Theorem)
let rec moduloReduce (lst:(int64*int64) list) : int64 =
    match lst with
    | [(_,r)] -> r
    | (m1,r1)::(m2,r2)::rest ->
        let inv1 = modInverse m2 m1
        let inv2 = modInverse m1 m2
        let remainder =
            ( (r1 * inv1 % m1) * m2 + (r2 * inv2 % m2) * m1 ) % (m1 * m2)
        let modulo = m1 * m2
        moduloReduce ((modulo, remainder)::rest)
    | [] -> failwith "Empty list"

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let snails = parseContent lines
    let modulos = 
        snails 
        |> List.map(fun snail -> 
            (snail.X + snail.Y - 1L, snail.Y - 1L))

    moduloReduce modulos
