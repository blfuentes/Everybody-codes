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

let buildEquallyWall(towersHeights: bigint[]) (numOfBlocks: bigint) =
    let spells = deconstructWall towersHeights |> Seq.sort
    let mutable (lower, high) = (1I, numOfBlocks)
    let mutable result = 0I
    while lower <= high do
        let mid = (lower + high) / 2I
        let requiredBlocks =
            spells
            |> Seq.sumBy(fun spell -> 
                mid / spell
            )
        if requiredBlocks <= numOfBlocks then
            result <- mid
            lower <- mid + 1I
        else
            high <- mid - 1I
    result

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let blocksToUse = 202520252025000I
    let towerHeights = parseContent(lines)
    buildEquallyWall towerHeights blocksToUse
