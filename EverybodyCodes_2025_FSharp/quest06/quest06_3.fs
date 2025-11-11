module quest06_part03

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest06/test_input_03.txt"
let path = "quest06/quest06_input_03.txt"

let findAllMentors (notes: string) repeats distance =
    let people = 
        notes.ToCharArray()
        |> Array.mapi(fun i v -> (i, v))
        |> Array.groupBy snd

    let baseLength = notes.Length

    let mutiplyPositions (pos: int) (cap: int) =
        let newPos = 
            seq {
                for i in 1..cap do
                    yield pos + i * baseLength
                
            } |> Array.ofSeq
        newPos            

    let findMentors(m: char) =
        let swordMentors =
            people 
            |> Array.filter(fun (p, _) -> p = m)
            |> Array.map snd
            |> Array.collect (fun v -> v |> Array.map fst)
            |> Array.collect (fun p -> mutiplyPositions p repeats)
        let swordNovices =
            people 
            |> Array.filter(fun (p, _) -> p = (char(m.ToString().ToLower())))
            |> Array.map snd
            |> Array.collect (fun v -> v |> Array.map fst)
            |> Array.collect (fun p -> mutiplyPositions p repeats)
        swordMentors
        |> Array.sumBy(fun m ->
            swordNovices |> Array.sumBy(fun n -> 
                if abs(m - n) <= distance then 1 else 0)
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
    //findAllMentors notes 1 10
    //findAllMentors notes 2 10
    findAllMentors notes 1000 1000
