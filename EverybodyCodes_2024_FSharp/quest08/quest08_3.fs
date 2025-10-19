module quest08_part03

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

//let path = "quest08/test_input_03.txt"
let path = "quest08/quest08_input_03.txt"

let RequiredByLevel = new Dictionary<int64, int64>()
let ThicknessByLevel = new Dictionary<int64, int64>()
let columnsHeightByLevel = new Dictionary<(int64*int64), int64>()

let baseSize level = int64(level * 2 - 1)

let rec thicknessForLevel (level: int64) (highPriests: int64) (acolytes: int64) =
    if ThicknessByLevel.ContainsKey(level) then
        ThicknessByLevel[level]
    else
        if level = 1L then
            ThicknessByLevel[level] <- int64(1)
            RequiredByLevel[level] <- int64(1)
            ThicknessByLevel[level]
        else
            let prev = thicknessForLevel (level - int64(1)) highPriests acolytes
            let thicknessLevel = ((prev * highPriests) % acolytes) + acolytes
            ThicknessByLevel[level] <- thicknessLevel
            RequiredByLevel[level] <- (baseSize (int level) * thicknessLevel) + RequiredByLevel[level - int64(1)]
            thicknessLevel

let emptySpaces (level: int64) (columnHeight: int64) (highPriests: int64) (acolytes: int64) =
    if level = 1L then
        0L
    else
        let baseSz = baseSize (int level)
        (highPriests * baseSz * columnHeight) % acolytes

let rec columnHeight (level: int64) (column: int64) (highPriests: int64) (acolytes: int64) =
    if columnsHeightByLevel.ContainsKey(level, column) then
        columnsHeightByLevel[(level, column)]
    else
        if level = 1L then
            columnsHeightByLevel[(1L, 1L)] <- 1L
            1L
        else
            let thickness = thicknessForLevel level highPriests acolytes
            let baseSz = baseSize (int level)
            match column with
            | col when col = 1L || col = baseSz -> 
                columnsHeightByLevel[(level, column)] <- thickness
                thickness
            | col ->
                //let height  = thickness + columnsHeightByLevel[(level - 1L, column - 1L)]
                let prev = columnHeight (level-1L) (column-1L) highPriests acolytes
                let height = thickness + prev
                columnsHeightByLevel[(level, column)] <- height
                height

let platinumBlocksForLevel (availableBlocks: int64) (highPriests: int64) (acolytes: int64) =
    let mutable current  = 0L
    let mutable level = 1L
    let thickness = thicknessForLevel level highPriests acolytes

    while availableBlocks > RequiredByLevel[level] do
        level <- level + 1L
        let _ = thicknessForLevel level highPriests acolytes
        ignore()

    current <- RequiredByLevel[level-1L]
    while availableBlocks > current do
        let recquired = RequiredByLevel[level]
        let heights = 
            [1L .. (baseSize (int level) / 2L) + 1L]
            |> List.map (fun col -> columnHeight level col highPriests acolytes)

        let empty = 
            if heights.Length <= 1 then 0L
            else
                heights
                |> List.skip 1
                |> List.mapi(fun i h ->
                    let e'=
                        if i = heights.Length - 2 then 
                            emptySpaces level h highPriests acolytes                         
                        else 
                            2L * (emptySpaces level h highPriests acolytes)
                    e')
                |> List.sum

        current <- recquired - empty
        //if [2; 3; 4; 5; 6; 7; 8; 9; 10; 16; 32; 64; 128; 256; 512; 1024; 2048; 4096] |> List.contains (int level) then
        //    printfn "Level: %d, Required: %d, Current: %d" level (RequiredByLevel.GetValueOrDefault(level, 0L)) current
        level <- level + 1L

    
    current - availableBlocks

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let highPriests = int64(lines[0])
    let acolytes = int64(10)
    let availableBlocks = int64(202400000)
    //let highPriests = int64(2)
    //let acolytes = int64(5)
    //let availableBlocks = int64(160)

    platinumBlocksForLevel availableBlocks highPriests acolytes
