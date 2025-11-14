module quest08_1

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

//let path = "quest08/test_input_01.txt"
let path = "quest08/quest08_input_01.txt"

let AvailableByLevel = new Dictionary<int, int>()
let BaseByLevel = new Dictionary<int, int>()

let rec neededForLevel (level: int) =
    if AvailableByLevel.ContainsKey(level) then
        AvailableByLevel[level]
    else
        if level = 1 then 
            AvailableByLevel[level] <- 1
            BaseByLevel[level] <- 1
            1
        else
            let prev = neededForLevel (level - 1)
            let thisLevel = level * 2 - 1
            BaseByLevel[level] <- thisLevel
            let required = thisLevel + prev
            AvailableByLevel[level] <- required
            AvailableByLevel[level]

let getNeededBlocks (available: int) =
    let rec findLevel (target: int) (current: int) (level: int) =
        if target > current then
            findLevel target (neededForLevel(level + 1)) (level + 1)
        else
            level
    let workingLevel = findLevel available 0 1
    let requiredBlocks = neededForLevel workingLevel
    (requiredBlocks - available) * BaseByLevel[workingLevel]

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let availableBlocks = int(lines[0])

    getNeededBlocks availableBlocks