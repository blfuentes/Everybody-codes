open System

open EverybodyCodes_2025_FSharp.Modules

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
    //let (resultquest02Visualization, time02_visualization) = duration quest02_visualization.execute
    //printfn "Final result Quest 02 part 3: %A in %s" resultquest02Visualization (ms time02_visualization)

    // DAY 03
    let (resultquest03_1, time03_1) = duration quest03_1.execute
    printfn "Final result Quest 03 part 1: %A in %s" resultquest03_1 (ms time03_1)
    let (resultquest03_2, time03_2) = duration quest03_2.execute
    printfn "Final result Quest 03 part 2: %A in %s" resultquest03_2 (ms time03_2)
    let (resultquest03_3, time03_3) = duration quest03_3.execute
    printfn "Final result Quest 03 part 3: %A in %s" resultquest03_3 (ms time03_3)

    // DAY 04
    let (resultquest04_1, time04_1) = duration quest04_1.execute
    printfn "Final result Quest 04 part 1: %A in %s" resultquest04_1 (ms time04_1)
    let (resultquest04_2, time04_2) = duration quest04_2.execute
    printfn "Final result Quest 04 part 2: %A in %s" resultquest04_2 (ms time04_2)
    let (resultquest04_3, time04_3) = duration quest04_3.execute
    printfn "Final result Quest 04 part 3: %A in %s" resultquest04_3 (ms time04_3)

    // DAY 05
    let (resultquest05_1, time05_1) = duration quest05_1.execute
    printfn "Final result Quest 05 part 1: %A in %s" resultquest05_1 (ms time05_1)
    let (resultquest05_2, time05_2) = duration quest05_2.execute
    printfn "Final result Quest 05 part 2: %A in %s" resultquest05_2 (ms time05_2)
    let (resultquest05_3, time05_3) = duration quest05_3.execute
    printfn "Final result Quest 05 part 3: %A in %s" resultquest05_3 (ms time05_3)

    // DAY 06
    let (resultquest06_1, time06_1) = duration quest06_1.execute
    printfn "Final result Quest 06 part 1: %A in %s" resultquest06_1 (ms time06_1)
    let (resultquest06_2, time06_2) = duration quest06_2.execute
    printfn "Final result Quest 06 part 2: %A in %s" resultquest06_2 (ms time06_2)
    let (resultquest06_3, time06_3) = duration quest06_3.execute
    printfn "Final result Quest 06 part 3: %A in %s" resultquest06_3 (ms time06_3)

    // DAY 07
    let (resultquest07_1, time07_1) = duration quest07_1.execute
    printfn "Final result Quest 07 part 1: %A in %s" resultquest07_1 (ms time07_1)
    let (resultquest07_2, time07_2) = duration quest07_2.execute
    printfn "Final result Quest 07 part 2: %A in %s" resultquest07_2 (ms time07_2)
    let (resultquest07_3, time07_3) = duration quest07_3.execute
    printfn "Final result Quest 07 part 3: %A in %s" resultquest07_3 (ms time07_3)
    //let (resultquest07Part4, time07_4) = duration quest07_part04.execute
    //printfn "Final result Quest 07 part 4: %A in %s" resultquest07Part4 (ms time07_4)

    // DAY 08
    let (resultquest08_1, time08_1) = duration quest08_1.execute
    printfn "Final result Quest 08 part 1: %A in %s" resultquest08_1 (ms time08_1)
    let (resultquest08_2, time08_2) = duration quest08_2.execute
    printfn "Final result Quest 08 part 2: %A in %s" resultquest08_2 (ms time08_2)
    //let (resultquest08_2_visualization, time08_2_visualization) = duration quest08_2_visualization.execute
    //printfn "Final result Quest 08 part 2 visualization: %A in %s" resultquest08_2_visualization (ms time08_2_visualization)
    let (resultquest08_3, time08_3) = duration quest08_3.execute
    printfn "Final result Quest 08 part 3: %A in %s" resultquest08_3 (ms time08_3)
    //let (resultquest08_3_optimized, time08_3_optimized) = duration quest08_3_optimized.execute
    //printfn "Final result Quest 08 part 3 optimized: %A in %s" resultquest08_3_optimized (ms time08_3_optimized)
    //let (resultquest08_3_visualization, time08_3_visualization) = duration quest08_3_visualization.execute
    //printfn "Final result Quest 08 part 3 visualization: %A in %s" resultquest08_3_visualization (ms time08_3_visualization)

    // DAY 09
    let (resultquest09_1, time09_1) = duration quest09_1.execute
    printfn "Final result Quest 09 part 1: %A in %s" resultquest09_1 (ms time09_1)
    let (resultquest09_2, time09_2) = duration quest09_2.execute
    printfn "Final result Quest 09 part 2: %A in %s" resultquest09_2 (ms time09_2)
    let (resultquest09_3, time09_3) = duration quest09_3.execute
    printfn "Final result Quest 09 part 3: %A in %s" resultquest09_3 (ms time09_3)
    //let (resultquest09_3_visualization, time09_3_visualization) = duration quest09_3_visualization.execute
    //printfn "Final result Quest 09 part 3 visualization: %A in %s" resultquest09_3_visualization (ms time09_3_visualization)

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

    0 // return an integer exit code
