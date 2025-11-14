open System

open EverybodyCodes_2024_S2_FSharp.Modules

let ms ticks =
    let timespan = (TimeSpan.FromTicks ticks)
    sprintf "%02i:%02i.%03i" timespan.Minutes timespan.Seconds timespan.Milliseconds

[<EntryPoint>]
let main argv =

    // DAY 00
    let (resultquest00_1, time00_1) = duration quest00_1.execute
    printfn "Final result Quest 00 part 1: %A in %s" resultquest00_1 (ms time00_1)
    let (resultquest00_2, time00_2) = duration quest00_2.execute
    printfn "Final result Quest 00 part 2: %A in %s" resultquest00_2 (ms time00_2)
    let (resultquest00_3, time00_3) = duration quest00_3.execute
    printfn "Final result Quest 00 part 3: %A in %s" resultquest00_3 (ms time00_3)

    // DAY 01
    let (resultquest01_1, time01_1) = duration quest01_1.execute
    printfn "Final result Quest 01 part 1: %A in %s" resultquest01_1 (ms time01_1)
    let (resultquest01_2, time01_2) = duration quest01_2.execute
    printfn "Final result Quest 01 part 2: %A in %s" resultquest01_2 (ms time01_2)
    let (resultquest01_3, time01_3) = duration quest01_3.execute
    printfn "Final result Quest 01 part 3: %A in %s" resultquest01_3 (ms time01_3)

    // DAY 02
    let (resultquest02_1, time02_1) = duration quest02_1.execute
    printfn "Final result Quest 02 part 1: %A in %s" resultquest02_1 (ms time02_1)
    let (resultquest02_2, time02_2) = duration quest02_2.execute
    printfn "Final result Quest 02 part 2: %A in %s" resultquest02_2 (ms time02_2)
    let (resultquest02_3, time02_3) = duration quest02_3.execute
    printfn "Final result Quest 02 part 3: %A in %s" resultquest02_3 (ms time02_3)

    // DAY 03
    let (resultquest03_1, time03_1) = duration quest03_1.execute
    printfn "Final result Quest 03 part 1: %A in %s" resultquest03_1 (ms time03_1)
    let (resultquest03_2, time03_2) = duration quest03_2.execute
    printfn "Final result Quest 03 part 2: %A in %s" resultquest03_2 (ms time03_2)
    let (resultquest03_3, time03_3) = duration quest03_3.execute
    printfn "Final result Quest 03 part 3: %A in %s" resultquest03_3 (ms time03_3)

    0 // return an integer exit code
