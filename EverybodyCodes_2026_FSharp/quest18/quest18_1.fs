module quest18_1

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

//let path = "quest18/test_input_01.txt"
let path = "quest18/quest18_input_01.txt"

let parseContent (lines: string[]) =
    [
        for line in lines do
            let numbers = 
                Regex.Matches(line, @"\d+") 
                |> Seq.map(fun m -> int(m.Value))
                |> fun xs ->(Seq.head xs, Seq.last xs)
            yield (numbers)
    ]

let countFilling(stacks: (int*int) list) =
    let rec processStack (substacks: (int*int) list) (count: int)=
        match substacks with
        | [] -> 
            let sumOfStacks = 
                stacks |> List.sumBy snd
            count + sumOfStacks
        | (colStack, height)::tail ->
            let mutable stackCount = 0
            for rowStack in 1..height do
                let hasStopper =
                    tail 
                    |> List.tryFind(fun (pos, h) -> 
                        h >= rowStack
                    )
                match hasStopper with
                | Some (pos, h) ->
                    stackCount <- stackCount + (pos - colStack - 1)
                | None -> ()
            processStack tail (count + stackCount)
        
    processStack stacks 0


let execute() =
    let lines = LocalHelper.GetContent path 2026 18 1
    let stacks = parseContent lines
    countFilling stacks
