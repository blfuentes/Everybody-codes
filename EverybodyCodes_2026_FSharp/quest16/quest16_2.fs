module quest16_2

open EverybodyCodes_2026_FSharp.Modules

//let path = "quest16/test_input_02.txt"
let path = "quest16/quest16_input_02.txt"

let totalFlipLength (toSort: char array) =
    let tablets = Array.copy toSort
    let mutable totalLength = 0L
    let mutable targetGroup = None
    let mutable sorted = false

    while not sorted do
        let mutable groupStart = 0

        while groupStart < tablets.Length && Option.isNone targetGroup do
            let color = tablets[groupStart]
            let mutable groupEnd = groupStart

            while groupEnd + 1 < tablets.Length && tablets[groupEnd + 1] = color do
                groupEnd <- groupEnd + 1

            let mutable laterGroupStart = groupEnd + 1
            while laterGroupStart < tablets.Length && tablets[laterGroupStart] <> color do
                laterGroupStart <- laterGroupStart + 1

            if laterGroupStart < tablets.Length then
                targetGroup <- Some(color, groupEnd)
            else
                groupStart <- groupEnd + 1

        if Option.isNone targetGroup then
            sorted <- true

        match targetGroup with
        | Some(color, firstGroupEnd) ->
            let mutable lastGroupEnd = firstGroupEnd + 1
            for index in firstGroupEnd + 1 .. tablets.Length - 1 do
                if tablets[index] = color then
                    lastGroupEnd <- index

            let mutable left = firstGroupEnd + 1
            let mutable right = lastGroupEnd
            while left < right do
                let temporary = tablets[left]
                tablets[left] <- tablets[right]
                tablets[right] <- temporary
                left <- left + 1
                right <- right - 1

            totalLength <- totalLength + int64 (lastGroupEnd - firstGroupEnd)
            targetGroup <- None
        | None -> ()

    totalLength

let execute () =
    let notes = ((LocalHelper.GetContent path 2026 16 2)[0]).ToCharArray()
    Array.replicate 10 notes
    |> Array.concat
    |> totalFlipLength
