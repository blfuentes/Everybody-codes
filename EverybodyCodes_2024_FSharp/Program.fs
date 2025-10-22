open System

open EverybodyCodes_2024_FSharp.Modules

let ms ticks =
    let timespan = (TimeSpan.FromTicks ticks)
    sprintf "%02i:%02i.%03i" timespan.Minutes timespan.Seconds timespan.Milliseconds

[<EntryPoint>]
let main argv =

    //// DAY 00
    //let (resultquest00Part1, time00_1) = duration quest00_part01.execute
    //printfn "Final result Quest 00 part 1: %A in %s" resultquest00Part1 (ms time00_1)
    //let (resultquest00Part2, time00_2) = duration quest00_part02.execute
    //printfn "Final result Quest 00 part 2: %A in %s" resultquest00Part2 (ms time00_2)
    //let (resultquest00Part3, time00_3) = duration quest00_part03.execute
    //printfn "Final result Quest 00 part 3: %A in %s" resultquest00Part3 (ms time00_3)

    //// DAY 08
    //let (resultquest08Part1, time08_1) = duration quest08_part01.execute
    //printfn "Final result Quest 08 part 1: %A in %s" resultquest08Part1 (ms time08_1)
    //let (resultquest08Part2, time08_2) = duration quest08_part02.execute
    //printfn "Final result Quest 08 part 2: %A in %s" resultquest08Part2 (ms time08_2)
    //let (resultquest08Part3, time08_3) = duration quest08_part03.execute
    //printfn "Final result Quest 08 part 3: %A in %s" resultquest08Part3 (ms time08_3)

    //// DAY 09
    //let (resultquest09Part1, time09_1) = duration quest09_part01.execute
    //printfn "Final result Quest 09 part 1: %A in %s" resultquest09Part1 (ms time09_1)
    //let (resultquest09Part2, time09_2) = duration quest09_part02.execute
    //printfn "Final result Quest 09 part 2: %A in %s" resultquest09Part2 (ms time09_2)
    //let (resultquest09Part3, time09_3) = duration quest09_part03.execute
    //printfn "Final result Quest 09 part 3: %A in %s" resultquest09Part3 (ms time09_3)

    //// DAY 10
    //let (resultquest10Part1, time10_1) = duration quest10_part01.execute
    //printfn "Final result Quest 10 part 1: %A in %s" resultquest10Part1 (ms time10_1)
    //let (resultquest10Part2, time10_2) = duration quest10_part02.execute
    //printfn "Final result Quest 10 part 2: %A in %s" resultquest10Part2 (ms time10_2)
    //let (resultquest10Part3, time10_3) = duration quest10_part03.execute
    //printfn "Final result Quest 10 part 3: %A in %s" resultquest10Part3 (ms time10_3)

    //// DAY 11
    //let (resultquest11Part1, time11_1) = duration quest11_part01.execute
    //printfn "Final result Quest 11 part 1: %A in %s" resultquest11Part1 (ms time11_1)
    //let (resultquest11Part2, time11_2) = duration quest11_part02.execute
    //printfn "Final result Quest 11 part 2: %A in %s" resultquest11Part2 (ms time11_2)
    //let (resultquest11Part3, time11_3) = duration quest11_part03.execute
    //printfn "Final result Quest 11 part 3: %A in %s" resultquest11Part3 (ms time11_3)

    // DAY 12
    let (resultquest12Part1, time12_1) = duration quest12_part01.execute
    printfn "Final result Quest 12 part 1: %A in %s" resultquest12Part1 (ms time12_1)
    let (resultquest12Part2, time12_2) = duration quest12_part02.execute
    printfn "Final result Quest 12 part 2: %A in %s" resultquest12Part2 (ms time12_2)
    let (resultquest12Part3, time12_3) = duration quest12_part03.execute
    printfn "Final result Quest 12 part 3: %A in %s" resultquest12Part3 (ms time12_3)

    0 // return an integer exit code
