open System

open EverybodyCodes_2024_FSharp.Modules

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

    // DAY 08
    let (resultquest08_1, time08_1) = duration quest08_1.execute
    printfn "Final result Quest 08 part 1: %A in %s" resultquest08_1 (ms time08_1)
    let (resultquest08_2, time08_2) = duration quest08_2.execute
    printfn "Final result Quest 08 part 2: %A in %s" resultquest08_2 (ms time08_2)
    let (resultquest08_3, time08_3) = duration quest08_3.execute
    printfn "Final result Quest 08 part 3: %A in %s" resultquest08_3 (ms time08_3)

    // DAY 09
    let (resultquest09_1, time09_1) = duration quest09_1.execute
    printfn "Final result Quest 09 part 1: %A in %s" resultquest09_1 (ms time09_1)
    let (resultquest09_2, time09_2) = duration quest09_2.execute
    printfn "Final result Quest 09 part 2: %A in %s" resultquest09_2 (ms time09_2)
    let (resultquest09_3, time09_3) = duration quest09_3.execute
    printfn "Final result Quest 09 part 3: %A in %s" resultquest09_3 (ms time09_3)

    // DAY 10
    let (resultquest10_1, time10_1) = duration quest10_1.execute
    printfn "Final result Quest 10 part 1: %A in %s" resultquest10_1 (ms time10_1)
    let (resultquest10_2, time10_2) = duration quest10_2.execute
    printfn "Final result Quest 10 part 2: %A in %s" resultquest10_2 (ms time10_2)
    let (resultquest10_3, time10_3) = duration quest10_3.execute
    printfn "Final result Quest 10 part 3: %A in %s" resultquest10_3 (ms time10_3)

    // DAY 11
    let (resultquest11_1, time11_1) = duration quest11_1.execute
    printfn "Final result Quest 11 part 1: %A in %s" resultquest11_1 (ms time11_1)
    let (resultquest11_2, time11_2) = duration quest11_2.execute
    printfn "Final result Quest 11 part 2: %A in %s" resultquest11_2 (ms time11_2)
    let (resultquest11_3, time11_3) = duration quest11_3.execute
    printfn "Final result Quest 11 part 3: %A in %s" resultquest11_3 (ms time11_3)

    // DAY 12
    let (resultquest12_1, time12_1) = duration quest12_1.execute
    printfn "Final result Quest 12 part 1: %A in %s" resultquest12_1 (ms time12_1)
    let (resultquest12_2, time12_2) = duration quest12_2.execute
    printfn "Final result Quest 12 part 2: %A in %s" resultquest12_2 (ms time12_2)
    let (resultquest12_3, time12_3) = duration quest12_3.execute
    printfn "Final result Quest 12 part 3: %A in %s" resultquest12_3 (ms time12_3)

    // DAY 13
    let (resultquest13_1, time13_1) = duration quest13_1.execute
    printfn "Final result Quest 13 part 1: %A in %s" resultquest13_1 (ms time13_1)
    let (resultquest13_2, time13_2) = duration quest13_2.execute
    printfn "Final result Quest 13 part 2: %A in %s" resultquest13_2 (ms time13_2)
    let (resultquest13_3, time13_3) = duration quest13_3.execute
    printfn "Final result Quest 13 part 3: %A in %s" resultquest13_3 (ms time13_3)

    // DAY 14
    let (resultquest14_1, time14_1) = duration quest14_1.execute
    printfn "Final result Quest 14 part 1: %A in %s" resultquest14_1 (ms time14_1)
    let (resultquest14_2, time14_2) = duration quest14_2.execute
    printfn "Final result Quest 14 part 2: %A in %s" resultquest14_2 (ms time14_2)
    let (resultquest14_3, time14_3) = duration quest14_3.execute
    printfn "Final result Quest 14 part 3: %A in %s" resultquest14_3 (ms time14_3)

    // DAY 15
    let (resultquest15_1, time15_1) = duration quest15_1.execute
    printfn "Final result Quest 15 part 1: %A in %s" resultquest15_1 (ms time15_1)
    let (resultquest15_2, time15_2) = duration quest15_2.execute
    printfn "Final result Quest 15 part 2: %A in %s" resultquest15_2 (ms time15_2)
    let (resultquest15_3, time15_3) = duration quest15_3.execute
    printfn "Final result Quest 15 part 3: %A in %s" resultquest15_3 (ms time15_3)

    // DAY 16
    let (resultquest16_1, time16_1) = duration quest16_1.execute
    printfn "Final result Quest 16 part 1: %A in %s" resultquest16_1 (ms time16_1)
    let (resultquest16_2, time16_2) = duration quest16_2.execute
    printfn "Final result Quest 16 part 2: %A in %s" resultquest16_2 (ms time16_2)
    let (resultquest16_3, time16_3) = duration quest16_3.execute
    printfn "Final result Quest 16 part 3: %A in %s" resultquest16_3 (ms time16_3)

    // DAY 17
    let (resultquest17_1, time17_1) = duration quest17_1.execute
    printfn "Final result Quest 17 part 1: %A in %s" resultquest17_1 (ms time17_1)
    let (resultquest17_2, time17_2) = duration quest17_2.execute
    printfn "Final result Quest 17 part 2: %A in %s" resultquest17_2 (ms time17_2)
    let (resultquest17_3, time17_3) = duration quest17_3.execute
    printfn "Final result Quest 17 part 3: %A in %s" resultquest17_3 (ms time17_3)

    // DAY 18
    let (resultquest18_1, time18_1) = duration quest18_1.execute
    printfn "Final result Quest 18 part 1: %A in %s" resultquest18_1 (ms time18_1)
    let (resultquest18_2, time18_2) = duration quest18_2.execute
    printfn "Final result Quest 18 part 2: %A in %s" resultquest18_2 (ms time18_2)
    let (resultquest18_3, time18_3) = duration quest18_3.execute
    printfn "Final result Quest 18 part 3: %A in %s" resultquest18_3 (ms time18_3)

    // DAY 19
    let (resultquest19_1, time19_1) = duration quest19_1.execute
    printfn "Final result Quest 19 part 1: %A in %s" resultquest19_1 (ms time19_1)
    let (resultquest19_2, time19_2) = duration quest19_2.execute
    printfn "Final result Quest 19 part 2: %A in %s" resultquest19_2 (ms time19_2)
    let (resultquest19_3, time19_3) = duration quest19_3.execute
    printfn "Final result Quest 19 part 3: %A in %s" resultquest19_3 (ms time19_3)

    // DAY 20
    let (resultquest20_1, time20_1) = duration quest20_1.execute
    printfn "Final result Quest 20 part 1: %A in %s" resultquest20_1 (ms time20_1)
    let (resultquest20_2, time20_2) = duration quest20_2.execute
    printfn "Final result Quest 20 part 2: %A in %s" resultquest20_2 (ms time20_2)
    let (resultquest20_3, time20_3) = duration quest20_3.execute
    printfn "Final result Quest 20 part 3: %A in %s" resultquest20_3 (ms time20_3)

    0 // return an integer exit code
