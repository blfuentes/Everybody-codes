module quest01_part02

open EverybodyCodes_2024_S2_FSharp.Modules

//let path = "quest01/test_input_02.txt"
let path = "quest01/quest01_input_02.txt"

let parseContent (lines: string array) =
    let idx = lines |> Array.findIndex(fun l -> l = "")
    (lines[..idx-1], lines[idx+1..])

let colFromSlot (slot:int) : int =
    (slot - 1) * 2

let slotFromCol (col:int) : int =
    if col % 2 <> 0 then failwithf "Column %d is not even" col
    col / 2 + 1

// Score function
let score (grid: string array) (token: string) (slot:int) : int =
    let mutable row = 0
    let mutable col = colFromSlot slot
    let directions = token.GetEnumerator()

    while row < grid.Length do
        if grid.[row].[col] = '*' then
            // advance in token
            if not (directions.MoveNext()) then
                failwith "Ran out of directions"
            col <- col + (if directions.Current = 'R' then 1 else -1)

            if col = -1 then
                col <- 1
            elif col = grid.[row].Length then
                col <- col - 2
        else
            row <- row + 1

    max ((slotFromCol col * 2) - slot) 0

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let (grid, tokens) = parseContent lines
    let total =
        tokens
        |> Array.sumBy (fun token ->
            [1..13]
            |> List.map (fun i -> score grid token i)
            |> List.max
        )
    total
