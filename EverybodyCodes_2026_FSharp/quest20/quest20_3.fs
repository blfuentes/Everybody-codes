module quest20_3

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

//let path = "quest20/test_input_03.txt"
let path = "quest20/quest20_input_03.txt"

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

let digitMask = function
    | '0' -> 0b0111111
    | '1' -> 0b0000110
    | '2' -> 0b1011011
    | '3' -> 0b1001111
    | '4' -> 0b1100110
    | '5' -> 0b1101101
    | '6' -> 0b1111101
    | '7' -> 0b0000111
    | '8' -> 0b1111111
    | '9' -> 0b1101111
    | 'A' -> 0b1110111
    | '+' -> 0b0100010
    | '-' -> 0b0100000
    | _ -> 0

let segmentCount c =
    let mutable mask = digitMask c
    let mutable count = 0
    while mask <> 0 do
        count <- count + (mask &&& 1)
        mask <- mask >>> 1
    count

let hammingDistance (left: char) (right: char) =
    let mutable difference = digitMask left ^^^ digitMask right
    let mutable count = 0
    while difference <> 0 do
        count <- count + (difference &&& 1)
        difference <- difference >>> 1
    count

let validCharacters(isOperator: bool) =
    if isOperator then [ '+'; '-' ]
    else [ '0'; '1'; '2'; '3'; '4'; '5'; '6'; '7'; '8'; '9'; 'A' ]

type Position =
    | Op1 of side: int * index: int
    | Op2 of side: int * index: int
    | Operator of side: int

let copyPair((e1, e2): Equation * Equation) =
    let copyEquation e = {
        Op1 = Array.copy e.Op1
        Op2 = Array.copy e.Op2
        Op = e.Op
        OpC = e.OpC
    }
    copyEquation e1, copyEquation e2

let getPosition(pair: Equation * Equation) (position: Position) =
    let e1, e2 = pair
    match position with
    | Op1(side, index) -> (if side = 1 then e1 else e2).Op1[index]
    | Op2(side, index) -> (if side = 1 then e1 else e2).Op2[index]
    | Operator side -> (if side = 1 then e1 else e2).OpC

let setPosition(pair: Equation * Equation) (position: Position) (value: char) =
    let mutable e1, e2 = copyPair pair
    match position with
    | Op1(side, index) ->
        if side = 1 then e1.Op1[index] <- value else e2.Op1[index] <- value
    | Op2(side, index) ->
        if side = 1 then e1.Op2[index] <- value else e2.Op2[index] <- value
    | Operator side ->
        if side = 1 then
            e1 <- { e1 with OpC = value; Op = if value = '+' then (+) else (-) }
        else
            e2 <- { e2 with OpC = value; Op = if value = '+' then (+) else (-) }
    e1, e2

let positionsIn(pair: Equation * Equation) =
    let e1, e2 = pair
    seq {
        for side, e in [ 1, e1; 2, e2 ] do
            for index in 0 .. e.Op1.Length - 1 do yield Op1(side, index)
            for index in 0 .. e.Op2.Length - 1 do yield Op2(side, index)
            yield Operator side
    }

let isRepairable(pair: Equation * Equation) =
    if operate(fst pair) = operate(snd pair) then
        true
    else
        let positions = positionsIn pair |> Seq.toArray
        let mutable found = false

        for position in positions do
            let isOperator =
                match position with
                | Operator _ -> true
                | _ -> false
            let original = getPosition pair position
            for candidate in validCharacters isOperator do
                let distance = hammingDistance original candidate
                if distance = 2 && segmentCount original = segmentCount candidate then
                    let repaired = setPosition pair position candidate
                    if operate(fst repaired) = operate(snd repaired) then
                        found <- true

        for source in positions do
            for destination in positions do
                if not found && source <> destination then
                    let sourceOriginal = getPosition pair source
                    let destinationOriginal = getPosition pair destination
                    let sourceIsOperator =
                        match source with
                        | Operator _ -> true
                        | _ -> false
                    let destinationIsOperator =
                        match destination with
                        | Operator _ -> true
                        | _ -> false

                    for sourceCandidate in validCharacters sourceIsOperator do
                        for destinationCandidate in validCharacters destinationIsOperator do
                            let sourceDistance = hammingDistance sourceOriginal sourceCandidate
                            let destinationDistance = hammingDistance destinationOriginal destinationCandidate
                            if sourceDistance = 1 &&
                               destinationDistance = 1 &&
                               segmentCount sourceOriginal = segmentCount sourceCandidate + 1 &&
                               segmentCount destinationOriginal + 1 = segmentCount destinationCandidate then
                                let repaired =
                                    pair
                                    |> fun p -> setPosition p source sourceCandidate
                                    |> fun p -> setPosition p destination destinationCandidate
                                if operate(fst repaired) = operate(snd repaired) then
                                    found <- true
        found

let execute() =
    let lines = LocalHelper.GetContent path 2026 20 3
    let equationPairs = parseContent lines
    let decimalTotal =
        equationPairs
        |> Seq.filter isRepairable
        |> Seq.sumBy (fun (e1, e2) ->
            segmentsOfEquation e1 + segmentsOfEquation e2 + segmentsOf '='
        )
    toBase11 decimalTotal