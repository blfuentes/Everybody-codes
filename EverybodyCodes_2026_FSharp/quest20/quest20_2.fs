module quest20_2

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

// let path = "quest20/test_input_02.txt"
let path = "quest20/quest20_input_02.txt"

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
                Regex.Matches(line, @"(?<op1>[\d+A]*) (?<op>[-+]) (?<op2>[\d+A]*)") 
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
        for c in v do
            let digit = if c = 'A' then 10 else int(c) - int('0')
            value <- value * 11 + digit
        value
    let result = eq.Op (buildNumber eq.Op1) (buildNumber eq.Op2)
    result

let toBase11(value: int) =
    if value = 0 then
        "0"
    else
        let mutable remaining = value
        let digits = System.Collections.Generic.List<char>()
        while remaining > 0 do
            let digit = remaining % 11
            digits.Add(if digit = 10 then 'A' else char(int('0') + digit))
            remaining <- remaining / 11
        digits
        |> Seq.rev
        |> Seq.toArray
        |> System.String

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
    | 'A' -> 6
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
    let lines = LocalHelper.GetContent path 2026 20 2
    let equationPairs = parseContent lines
    let decimalTotal =
        equationPairs
        |> Seq.filter (fun (e1, e2) -> operate e1 = operate e2)
        |> Seq.sumBy (fun (e1, e2) ->
            segmentsOfEquation e1 + segmentsOfEquation e2 + segmentsOf '='
        )
    toBase11 decimalTotal
