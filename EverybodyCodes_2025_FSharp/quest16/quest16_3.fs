module quest16_3

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest16/test_input_03.txt"
let path = "quest16/quest16_input_03.txt"

let parseContent(lines: string) =
    lines.Split(",") |> Array.map(fun v -> bigint.Parse(v))

let deconstructWall (towers: bigint[]) =
    let spells = ResizeArray<bigint>()
    let simulated = Array.zeroCreate<bigint> (towers.Length)
    let wallLength = towers.Length

    towers
    |> Array.iteri(fun index towerHeight ->
        let tempHeight = simulated[index]
        if tempHeight < towerHeight then
            let diff = int(towerHeight - tempHeight)
            let pos = index + 1
            spells.Add(pos)
            for j in index..pos..wallLength-1 do
                simulated[j] <- simulated[j] + bigint(diff)
    )   
    spells

let buildEquallyWall (towersHeights: bigint[]) (numOfBlocks: bigint) =
    let spells = deconstructWall towersHeights |> Seq.sort
    
    let calculateRequiredBlocks height =
        spells |> Seq.sumBy (fun spell -> height / spell)
    
    let rec binarySearch lower upper =
        if lower > upper then
            upper
        else
            let mid = (lower + upper) / 2I
            let requiredBlocks = calculateRequiredBlocks mid
            
            if requiredBlocks <= numOfBlocks then
                binarySearch (mid + 1I) upper
            else
                binarySearch lower (mid - 1I)
    
    binarySearch 1I numOfBlocks

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let blocksToUse = 202520252025000I
    let towerHeights = parseContent(lines)
    buildEquallyWall towerHeights blocksToUse
