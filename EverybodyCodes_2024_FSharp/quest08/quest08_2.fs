module quest08_2

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

//let path = "quest08/test_input_02.txt"
let path = "quest08/quest08_input_02.txt"

let RequiredByLevel = new Dictionary<int, int>()
let ThicknessByLevel = new Dictionary<int, int>()

let baseSize level = level * 2 - 1

let rec thicknessForLevel (level: int) (currentPriests: int) (acolytes: int) =
    if ThicknessByLevel.ContainsKey(level) then
        ThicknessByLevel[level]
    else
        if level = 1 then
            ThicknessByLevel[level] <- 1
            1
        else
            let prev = thicknessForLevel (level - 1) currentPriests acolytes
            let thicknessLevel = (prev * currentPriests) % acolytes
            ThicknessByLevel[level] <- thicknessLevel
            thicknessLevel

let rec requiredForLevel (level: int) (currentPriests: int) (acolytes: int) =    
    if RequiredByLevel.ContainsKey(level) then
        RequiredByLevel[level]
    else
        if level = 1 then 
            RequiredByLevel[level] <- 1
            1
        else
            let prev = requiredForLevel (level - 1) currentPriests acolytes
            let thisLevel = (thicknessForLevel level currentPriests acolytes) * baseSize level
            let required = thisLevel + prev
            RequiredByLevel[level] <- required
            required

let getNeededBlocks (available: int) (priests: int) (acolytes: int) =
    let rec findLevel (target: int) (current: int) (level: int) =
        if target > current then
            findLevel target (requiredForLevel(level + 1) priests acolytes) (level + 1)
        else
            level
    let workingLevel = findLevel available 0 1
    let requiredBlocks = requiredForLevel workingLevel priests acolytes
    (requiredBlocks - available) * (baseSize workingLevel)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let numberOfPriests = int(lines[0])
    let availableBlocks = 20240000
    let numberOfAcolytes = 1111

    getNeededBlocks availableBlocks numberOfPriests numberOfAcolytes
