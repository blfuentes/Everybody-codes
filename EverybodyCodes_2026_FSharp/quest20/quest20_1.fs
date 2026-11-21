module quest20_1

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

// let path = "quest20/test_input_01.txt"
let path = "quest20/quest20_input_01.txt"

type Equation = {
    Op1: char array
    Op2: char array
    Op: int -> int -> int
    OpC: char
}

let parseContent(lines: string array) =
    let extractOp(op: string) =
        let (o, oc) =
            match op: string with
            | "+" -> ((+), '+')
            | "-" -> ((-), '-')
            | _ -> failwith "invalid operation"
        (o, oc)
        
    seq {
        for line in lines do
            let parts = 
                Regex.Matches(line, @"(?<op1>\d+) (?<op>[-+]) (?<op2>\d+)") 
            let (o1, o1c) = extractOp parts.Item(0).Groups["op"].Value
            let (o2, o2c) = extractOp parts.Item(1).Groups["op"].Value
            let leftSide = { 
                Op1 = parts.Item(0).Groups["op1"].Value.ToCharArray(); 
                Op2 = parts.Item(0).Groups["op2"].Value.ToCharArray();
                Op = o1;
                OpC = o1c
            }
            let rightSide = { 
                Op1 = parts.Item(1).Groups["op1"].Value.ToCharArray(); 
                Op2 = parts.Item(1).Groups["op2"].Value.ToCharArray();
                Op = o2;
                OpC = o2c
            }
            yield (leftSide, rightSide)
    }

let operate(eq: Equation) =
    let inline buildNumber(v: char array) =
        let mutable value = 0
        for i in v.Length-1.. -1 .. 0 do
            value <- value * 10 + (int(v[i]) - int('0')) 
        value
    let result = eq.Op (buildNumber eq.Op1) (buildNumber eq.Op2)
    result

let segmentsOf(c: char) =
    match c with
    | '0' -> 6
    | '1' -> 2
    | '2' -> 5
    | '3' -> 5
    | '4' -> 4
    | '5' -> 5
    | '6' -> 6
    | '7' -> 3
    | '8' -> 7
    | '9' -> 6
    | '+' -> 2
    | '-' -> 1
    | '=' -> 2
    | _ -> failwith "invalid segment"

let segmentsOfEquation(equation: Equation) =
    let op1 = equation.Op1 |> Seq.sumBy segmentsOf
    let op2 = equation.Op2 |> Seq.sumBy segmentsOf
    let op = segmentsOf equation.OpC
    op1 + op2 + op


let execute() =
    let lines = LocalHelper.GetContent path 2026 20 1
    let equationPairs = parseContent lines
    equationPairs
    |> Seq.filter (fun (e1, e2) -> operate e1 = operate e2)
    |> Seq.sumBy (fun (e1, e2) ->
        segmentsOfEquation e1 + segmentsOfEquation e2 + segmentsOf '='
    )