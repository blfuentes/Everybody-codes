module quest03_3

open EverybodyCodes_2024_S1_FSharp.Modules

//let path = "quest03/test_input_03.txt"
let path = "quest03/quest03_input_03.txt"

type Snail = {
    X: bigint
    Y: bigint
}

let parseContent (lines: string array) =
    let snails =
        seq {
            for line in lines do
                { 
                    X = bigint.Parse((line.Split(" ")[0]).Split("=")[1])
                    Y = bigint.Parse((line.Split(" ")[1]).Split("=")[1])
                }    
        } |> Seq.toList
    snails

// Extended Euclidean Algorithm for modular inverse (bigint)
let modInverse (a:bigint) (m:bigint) : bigint =
    let rec egcd (a:bigint) (b:bigint) : bigint*bigint*bigint =
        if b = 0I then (a, 1I, 0I)
        else
            let (g, x, y) = egcd b (a % b)
            (g, y, x - (a / b) * y)
    let (g, x, _) = egcd a m
    if g <> 1I then failwithf "No modular inverse for %A mod %A" a m
    else ((x % m + m) % m)

// Recursive reducer (Chinese Remainder Theorem)
let rec moduloReduce (lst:(bigint*bigint) list) : bigint =
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
            (snail.X + snail.Y - 1I, snail.Y - 1I))

    moduloReduce modulos
