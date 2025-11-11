module quest06_part01

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest06/test_input_01.txt"
let path = "quest06/quest06_input_01.txt"

let findSwordsMentors (notes: string) =
    let people = 
        notes.ToCharArray()
        |> Array.mapi(fun i v -> (i, v))
        |> Array.groupBy snd
    let swordMentors =
        people 
        |> Array.filter(fun (p, _) -> p = 'A')
        |> Array.map snd
        |> Array.collect (fun v -> v |> Array.map fst)
    let swordNovices =
        people 
        |> Array.filter(fun (p, _) -> p = 'a')
        |> Array.map snd
        |> Array.collect (fun v -> v |> Array.map fst)
    swordMentors
    |> Array.sumBy(fun m ->
        swordNovices |> Array.sumBy(fun n -> if m < n then 1 else 0)
    )
    

let execute() =
    let notes = LocalHelper.GetContentFromFile(path)
    findSwordsMentors notes
