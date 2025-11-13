open System

open EverybodyCodes_2025_FSharp.Modules

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

    //// DAY 01
    //let (resultquest01Part1, time01_1) = duration quest01_part01.execute
    //printfn "Final result Quest 01 part 1: %A in %s" resultquest01Part1 (ms time01_1)
    //let (resultquest01Part2, time01_2) = duration quest01_part02.execute
    //printfn "Final result Quest 01 part 2: %A in %s" resultquest01Part2 (ms time01_2)
    //let (resultquest01Part3, time01_3) = duration quest01_part03.execute
    //printfn "Final result Quest 01 part 3: %A in %s" resultquest01Part3 (ms time01_3)

    //// DAY 02
    //let (resultquest02Part1, time02_1) = duration quest02_part01.execute
    //printfn "Final result Quest 02 part 1: %A in %s" resultquest02Part1 (ms time02_1)
    //let (resultquest02Part2, time02_2) = duration quest02_part02.execute
    //printfn "Final result Quest 02 part 2: %A in %s" resultquest02Part2 (ms time02_2)
    //let (resultquest02Part3, time02_3) = duration quest02_part03.execute
    //printfn "Final result Quest 02 part 3: %A in %s" resultquest02Part3 (ms time02_3)
    ////let (resultquest02Visualization, time02_visualization) = duration quest02_visualization.execute
    ////printfn "Final result Quest 02 part 3: %A in %s" resultquest02Visualization (ms time02_visualization)

    //// DAY 03
    //let (resultquest03Part1, time03_1) = duration quest03_part01.execute
    //printfn "Final result Quest 03 part 1: %A in %s" resultquest03Part1 (ms time03_1)
    //let (resultquest03Part2, time03_2) = duration quest03_part02.execute
    //printfn "Final result Quest 03 part 2: %A in %s" resultquest03Part2 (ms time03_2)
    //let (resultquest03Part3, time03_3) = duration quest03_part03.execute
    //printfn "Final result Quest 03 part 3: %A in %s" resultquest03Part3 (ms time03_3)

    //// DAY 04
    //let (resultquest04Part1, time04_1) = duration quest04_part01.execute
    //printfn "Final result Quest 04 part 1: %A in %s" resultquest04Part1 (ms time04_1)
    //let (resultquest04Part2, time04_2) = duration quest04_part02.execute
    //printfn "Final result Quest 04 part 2: %A in %s" resultquest04Part2 (ms time04_2)
    //let (resultquest04Part3, time04_3) = duration quest04_part03.execute
    //printfn "Final result Quest 04 part 3: %A in %s" resultquest04Part3 (ms time04_3)

    //// DAY 05
    //let (resultquest05Part1, time05_1) = duration quest05_part01.execute
    //printfn "Final result Quest 05 part 1: %A in %s" resultquest05Part1 (ms time05_1)
    //let (resultquest05Part2, time05_2) = duration quest05_part02.execute
    //printfn "Final result Quest 05 part 2: %A in %s" resultquest05Part2 (ms time05_2)
    //let (resultquest05Part3, time05_3) = duration quest05_part03.execute
    //printfn "Final result Quest 05 part 3: %A in %s" resultquest05Part3 (ms time05_3)

    //// DAY 06
    //let (resultquest06Part1, time06_1) = duration quest06_part01.execute
    //printfn "Final result Quest 06 part 1: %A in %s" resultquest06Part1 (ms time06_1)
    //let (resultquest06Part2, time06_2) = duration quest06_part02.execute
    //printfn "Final result Quest 06 part 2: %A in %s" resultquest06Part2 (ms time06_2)
    //let (resultquest06Part3, time06_3) = duration quest06_part03.execute
    //printfn "Final result Quest 06 part 3: %A in %s" resultquest06Part3 (ms time06_3)

    //// DAY 07
    //let (resultquest07Part1, time07_1) = duration quest07_part01.execute
    //printfn "Final result Quest 07 part 1: %A in %s" resultquest07Part1 (ms time07_1)
    //let (resultquest07Part2, time07_2) = duration quest07_part02.execute
    //printfn "Final result Quest 07 part 2: %A in %s" resultquest07Part2 (ms time07_2)
    //let (resultquest07Part3, time07_3) = duration quest07_part03.execute
    //printfn "Final result Quest 07 part 3: %A in %s" resultquest07Part3 (ms time07_3)
    ////let (resultquest07Part4, time07_4) = duration quest07_part04.execute
    ////printfn "Final result Quest 07 part 4: %A in %s" resultquest07Part4 (ms time07_4)

    // DAY 08
    let (resultquest08Part1, time08_1) = duration quest08_part01.execute
    printfn "Final result Quest 08 part 1: %A in %s" resultquest08Part1 (ms time08_1)
    let (resultquest08Part2, time08_2) = duration quest08_part02.execute
    printfn "Final result Quest 08 part 2: %A in %s" resultquest08Part2 (ms time08_2)
    let (resultquest08Part3, time08_3) = duration quest08_part03.execute
    printfn "Final result Quest 08 part 3: %A in %s" resultquest08Part3 (ms time08_3)
    
    0 // return an integer exit code
