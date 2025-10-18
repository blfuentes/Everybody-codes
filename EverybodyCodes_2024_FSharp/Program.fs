open System

open EverybodyCodes_2024_FSharp.Modules

let ms ticks =
    let timespan = (TimeSpan.FromTicks ticks)
    sprintf "%02i:%02i.%03i" timespan.Minutes timespan.Seconds timespan.Milliseconds

[<EntryPoint>]
let main argv =
    let timer = new System.Diagnostics.Stopwatch()
    timer.Start()

    // DAY 00
    let (resultquest00Part1, time00_1) = duration quest00_part01.execute
    printfn "Final result Quest 00 part 1: %A in %s" resultquest00Part1 (ms time00_1)
    let (resultquest00Part2, time00_2) = duration quest00_part02.execute
    printfn "Final result Quest 00 part 2: %A in %s" resultquest00Part2 (ms time00_2)
    let (resultquest00Part3, time00_3) = duration quest00_part03.execute
    printfn "Final result Quest 00 part 3: %A in %s" resultquest00Part3 (ms time00_3)

    // DAY 08
    let (resultquest08Part1, time08_1) = duration quest08_part01.execute
    printfn "Final result Quest 08 part 1: %A in %s" resultquest08Part1 (ms time08_1)
    let (resultquest08Part2, time08_2) = duration quest08_part02.execute
    printfn "Final result Quest 08 part 2: %A in %s" resultquest08Part2 (ms time08_2)
    let (resultquest08Part3, time08_3) = duration quest08_part03.execute
    printfn "Final result Quest 08 part 3: %A in %s" resultquest08Part3 (ms time08_3)

    0 // return an integer exit code
