open System

open EverybodyCodes_2024_S1_FSharp.Modules

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

    0 // return an integer exit code
