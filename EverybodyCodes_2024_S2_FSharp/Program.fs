open System

open EverybodyCodes_2024_S2_FSharp.Modules

let ms ticks =
    let timespan = (TimeSpan.FromTicks ticks)
    sprintf "%02i:%02i.%03i" timespan.Minutes timespan.Seconds timespan.Milliseconds

[<EntryPoint>]
let main argv =

    // DAY 00
    let (resultquest00Part1, time00_1) = duration quest00_part01.execute
    printfn "Final result Quest 00 part 1: %A in %s" resultquest00Part1 (ms time00_1)
    let (resultquest00Part2, time00_2) = duration quest00_part02.execute
    printfn "Final result Quest 00 part 2: %A in %s" resultquest00Part2 (ms time00_2)
    let (resultquest00Part3, time00_3) = duration quest00_part03.execute
    printfn "Final result Quest 00 part 3: %A in %s" resultquest00Part3 (ms time00_3)

    // DAY 01
    let (resultquest01Part1, time01_1) = duration quest01_part01.execute
    printfn "Final result Quest 01 part 1: %A in %s" resultquest01Part1 (ms time01_1)
    let (resultquest01Part2, time01_2) = duration quest01_part02.execute
    printfn "Final result Quest 01 part 2: %A in %s" resultquest01Part2 (ms time01_2)
    let (resultquest01Part3, time01_3) = duration quest01_part03.execute
    printfn "Final result Quest 01 part 3: %A in %s" resultquest01Part3 (ms time01_3)

    // DAY 02
    let (resultquest02Part1, time02_1) = duration quest02_part01.execute
    printfn "Final result Quest 02 part 1: %A in %s" resultquest02Part1 (ms time02_1)
    let (resultquest02Part2, time02_2) = duration quest02_part02.execute
    printfn "Final result Quest 02 part 2: %A in %s" resultquest02Part2 (ms time02_2)
    let (resultquest02Part3, time02_3) = duration quest02_part03.execute
    printfn "Final result Quest 02 part 3: %A in %s" resultquest02Part3 (ms time02_3)

    // DAY 03
    let (resultquest03Part1, time03_1) = duration quest03_part01.execute
    printfn "Final result Quest 03 part 1: %A in %s" resultquest03Part1 (ms time03_1)
    let (resultquest03Part2, time03_2) = duration quest03_part02.execute
    printfn "Final result Quest 03 part 2: %A in %s" resultquest03Part2 (ms time03_2)
    let (resultquest03Part3, time03_3) = duration quest03_part03.execute
    printfn "Final result Quest 03 part 3: %A in %s" resultquest03Part3 (ms time03_3)

    0 // return an integer exit code
