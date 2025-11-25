module quest16_2

open EverybodyCodes_2025_FSharp.Modules

//let path = "quest16/test_input_02.txt"
let path = "quest16/quest16_input_02.txt"

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

let calculateWall (instructions: bigint[]) =
    let spells = deconstructWall instructions
    spells
    |> Seq.reduce(fun acc i-> 
        acc * i
    )

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    let instructions = parseContent(lines)
    calculateWall instructions
