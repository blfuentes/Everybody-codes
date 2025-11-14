module quest06_3

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest06/test_input_03.txt"
let path = "quest06/quest06_input_03.txt"

let findAllMentors (notes: string) repeats (distance: int) =
    let people =
        notes.ToCharArray()
        |> Array.mapi (fun i c -> (c, i))
        |> Array.groupBy fst
        |> Map.ofArray
        |> Map.map (fun _ positions -> positions |> Array.map snd)

    let baseLength = int64 notes.Length
    let distance = int64 distance
    let repeats = int64 repeats

    let getPositions char =
        match people.TryFind(char) with
        | Some pos -> pos
        | None -> [||]

    let findMentors (mentorChar: char) =
        let mentorPositions = getPositions mentorChar
        let novicePositions = getPositions (System.Char.ToLower(mentorChar))

        if Array.isEmpty mentorPositions || Array.isEmpty novicePositions then
            0L
        else
            Array.sumBy (fun m_pos ->
                Array.sumBy (fun n_pos ->
                    let delta_pos = int64 (m_pos - n_pos)

                    let i_min_num = -distance - delta_pos
                    let i_min = if i_min_num >= 0L then (i_min_num + baseLength - 1L) / baseLength else i_min_num / baseLength

                    let i_max_num = distance - delta_pos
                    let i_max = if i_max_num >= 0L then i_max_num / baseLength else (i_max_num - baseLength + 1L) / baseLength
                    
                    let i_start = max i_min (-(repeats - 1L))
                    let i_end = min i_max (repeats - 1L)

                    if i_start <= i_end then
                        Seq.sumBy (fun i -> repeats - abs i) [i_start..i_end]
                    else
                        0L
                ) novicePositions
            ) mentorPositions

    let possibleMentors =
        notes.ToCharArray()
        |> Array.filter System.Char.IsUpper
        |> Array.distinct

    possibleMentors
    |> Array.sumBy findMentors

let execute() =
    let notes = LocalHelper.GetContentFromFile(path)
    //findAllMentors notes 1 10
    //findAllMentors notes 2 10
    findAllMentors notes 1000 1000
