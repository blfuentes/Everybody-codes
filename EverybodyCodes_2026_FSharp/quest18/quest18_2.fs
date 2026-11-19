module quest18_2

open EverybodyCodes_2026_FSharp.Modules
open System.Text.RegularExpressions

//let path = "quest18/test_input_02.txt"
let path = "quest18/quest18_input_02.txt"

let parseContent (lines: string[]) =
    [
        for line in lines do
            let numbers = 
                Regex.Matches(line, @"\d+") 
                |> Seq.map(fun m -> int(m.Value))
                |> Array.ofSeq
                |> fun xs ->(xs[0], xs[1], xs[2])
            yield (numbers)
    ]

let countFilling(stacks: (int*int*int) list) =
    let rec processStack (substacks: (int*int*int) list) (count: int)=
        match substacks with
        | [] -> 
            let sumOfStacks = 
                stacks |> List.sumBy(fun (_, h, w)-> h*w)
            count + sumOfStacks
        | (colStack, height, width)::tail ->
            let mutable stackCount = 0
            for rowStack in 1..height do
                let hasStopper =
                    tail 
                    |> List.tryFind(fun (_, h, _) -> 
                        h >= rowStack
                    )
                match hasStopper with
                | Some (p, _, _) ->
                    stackCount <- stackCount + (p - colStack - width)
                | None -> ()
            processStack tail (count + stackCount)
        
    processStack stacks 0


let execute() =
    let lines = LocalHelper.GetContent path 2026 18 2
    let stacks = parseContent lines
    countFilling stacks
