module quest06_2

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest06/test_input_02.txt"
let path = "quest06/quest06_input_02.txt"

let findAllMentors (notes: string) =
    let people = 
        notes.ToCharArray()
        |> Array.mapi(fun i v -> (i, v))
        |> Array.groupBy snd
    let findMentors(m: char) =
        let mentors =
            people 
            |> Array.filter(fun (p, _) -> p = m)
            |> Array.map snd
            |> Array.collect (fun v -> v |> Array.map fst)
        let novices =
            people 
            |> Array.filter(fun (p, _) -> p = (char(m.ToString().ToLower())))
            |> Array.map snd
            |> Array.collect (fun v -> v |> Array.map fst)
        mentors
        |> Array.sumBy(fun m ->
            novices |> Array.sumBy(fun n -> if m < n then 1 else 0)
        )
    
    let possibleMentors = 
        ['A'..'Z'] 
        |> List.filter(fun m -> 
            people
            |> Array.map fst
            |> Array.contains m
        )

    possibleMentors
    |> List.sumBy findMentors

let execute() =
    let notes = LocalHelper.GetContentFromFile(path)
    findAllMentors notes
