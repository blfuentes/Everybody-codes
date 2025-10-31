module quest03_part03

open EverybodyCodes_2024_S2_FSharp.Modules
open System

//let path = "quest03/test_input_03.txt"
let path = "quest03/quest03_input_03.txt"

type Dice(diceId:int, faces:int list, seed:int) =
    let mutable pulse = bigint seed
    let mutable faceId = bigint 0
    let mutable rollNumber = bigint 1
    let mutable spin = bigint 0
    let mutable target = 0
    let mutable rolls : int list = []
    let mutable currentRoll = 0

    member _.Id = diceId
    member _.Faces = faces
    member _.Seed = seed
    member _.Target with get() = target
    member _.CurrentRoll with get() = currentRoll

    member this.Roll() : int =
        spin <- rollNumber * pulse
        faceId <- (faceId + spin) % (bigint faces.Length)
        if faceId < 0I then faceId <- faceId + (bigint faces.Length)

        pulse <- pulse + spin
        pulse <- pulse % (bigint seed)
        pulse <- pulse + (bigint (1 + seed)) + rollNumber
        rollNumber <- rollNumber + 1I

        faces.[int faceId]

    member this.NextTarget(sequence:int list) : bool =
        if target >= sequence.Length - 1 then
            true
        else
            target <- target + 1
            false

    member this.Precompute(cellCount:int) =
        rolls <- []
        currentRoll <- 0
        // generate cellCount rolls and store them
        rolls <- [ for _ in 1 .. cellCount -> this.Roll() ]

    member this.Next(step:int) : int =
        // return the precomputed roll at given step
        rolls.[step]

// DFS exploration
let explore (grid:int list list) (dice:Dice) (startPositions:(int*int*int*bool) list) : Set<int*int> =
    let mutable totalVisited = Set.empty
    for (x0,y0,step0,moved0) in startPositions do
        let mutable stack = [(x0,y0,step0,moved0)]
        let mutable visited = Set.empty
        while not stack.IsEmpty do
            let (x,y,step,moved) = List.head stack
            stack <- List.tail stack
            if visited.Contains(x,y,step) && moved then
                () // skip
            else
                visited <- visited.Add(x,y,step)
                let dxy = [ (0,1); (1,0); (0,-1); (-1,0) ]
                let nextVal = dice.Next(step+1)
                for (dx,dy) in dxy do
                    let nx,ny = x+dx, y+dy
                    if nx >= 0 && nx < grid.[0].Length && ny >= 0 && ny < grid.Length then
                        if grid.[ny].[nx] = nextVal then
                            stack <- (nx,ny,step+1,true) :: stack
                if grid.[y].[x] = nextVal then
                    stack <- (x,y,step+1,false) :: stack
        // add all visited positions (ignoring step)
        for (x,y,_) in visited |> Seq.map (fun (x,y,step) -> (x,y,step)) do
            totalVisited <- totalVisited.Add(x,y)
    totalVisited

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)

    let mutable readDice = true
    let mutable dices : Dice list = []
    let mutable grid : int list list = []

    for line in lines do
        if line = "" then
            readDice <- false
        elif readDice then
            let parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            let diceId = int (parts.[0].TrimEnd(':'))
            let faces =
                parts.[1].Split('=').[1].Trim()
                |> fun s -> s.Trim([|'[';']'|])
                |> fun s -> s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                |> Array.map int
                |> Array.toList
            let seed = int (parts.[2].Split('=').[1])
            dices <- dices @ [Dice(diceId, faces, seed)]
        else
            grid <- grid @ [ line |> Seq.map (fun ch -> int (string ch)) |> Seq.toList ]

    let cellCount = grid.Length * grid.[0].Length
    let mutable totalVisited : Set<int*int> = Set.empty

    for dice in dices do
        dice.Precompute(cellCount + 10000)
        let v0 = dice.Next(0)
        let stack =
            [ for x in 0 .. grid.[0].Length-1 do
                for y in 0 .. grid.Length-1 do
                    if grid.[y].[x] = v0 then
                        yield (x,y,0,true) ]
        totalVisited <- totalVisited + (explore grid dice stack)

    totalVisited.Count
