module quest12_part03

open EverybodyCodes_2024_FSharp.Modules

//let path = "quest12/test_input_03.txt"
let path = "quest12/quest12_input_03.txt"

type Entity = { Pos: float * float; Val: char }

let parseContent (catapultLayout: string) (targetLines: string array) =
    let catapults =
        catapultLayout.Split('\n')
        |> Array.mapi (fun y line ->
            line.ToCharArray()
            |> Array.mapi (fun x ch ->
                if ['A'; 'B'; 'C'] |> List.contains ch then
                    Some { Pos = (float x, float y); Val = ch }
                else None
            )
        )
        |> Array.concat
        |> Array.choose id
        |> Array.toList

    let aPos = (catapults |> List.find (fun c -> c.Val = 'A')).Pos

    let targets =
        targetLines
        |> Array.map (fun line ->
            let parts = line.Split(' ')
            let x, y = float parts[0], float parts[1]
            { Pos = (fst aPos + x, snd aPos - y); Val = 'T' }
        )
        |> Array.toList
    
    (catapults, targets)

let vectUR = (1.0, -1.0)
let vectR  = (1.0, 0.0)
let vectDR = (1.0, 1.0)
let vectDL = (-1.0, 1.0)

let private areFloatsEqual a b = abs (a - b) < 1e-9

let canHitTarget (fromPos: float * float) (_toPos: float * float) (moving: int) (shift: int) (maxPower: int) =
    let mutable result = None
    let mutable power = 1

    let toPos =
        if shift = 1 then (fst _toPos + fst vectDL, snd _toPos + snd vectDL)
        else _toPos

    while result.IsNone && power < maxPower do
        let p = float power
        let m = float moving
        let (fromX, fromY) = fromPos
        let (toX, toY) = toPos
        let (urX, urY) = vectUR
        let (dlX, dlY) = vectDL

        // Phase 1: Up-Right
        let denX1 = urX - m * dlX
        let denY1 = urY - m * dlY
        if denX1 <> 0.0 && denY1 <> 0.0 then
            let tX = (toX - fromX) / denX1
            let tY = (toY - fromY) / denY1
            if areFloatsEqual tX tY && areFloatsEqual tX (round tX) && tX > 0.0 && tX <= p then
                result <- Some power

        if result.IsNone then
            let from2 = (fromX + p * urX, fromY + p * urY)
            let to2 = (toX + m * p * dlX, toY + m * p * dlY)
            let (rX, rY) = vectR
            let denX2 = rX - m * dlX
            let denY2 = rY - m * dlY

            // Phase 2: Right
            if denX2 <> 0.0 && denY2 <> 0.0 then
                let tX = (fst to2 - fst from2) / denX2
                let tY = (snd to2 - snd from2) / denY2
                if areFloatsEqual tX tY && areFloatsEqual tX (round tX) && tX > 0.0 && tX <= p then
                    result <- Some power
            elif areFloatsEqual denY2 0.0 && areFloatsEqual (snd to2) (snd from2) && denX2 <> 0.0 then
                let tX = (fst to2 - fst from2) / denX2
                if areFloatsEqual tX (round tX) && tX > 0.0 && tX <= p then
                    result <- Some power

            if result.IsNone then
                let from3 = (fst from2 + p * rX, snd from2 + p * rY)
                let to3 = (fst to2 + m * p * dlX, snd to2 + m * p * dlY)
                let (drX, drY) = vectDR
                let denX3 = drX - m * dlX
                let denY3 = drY - m * dlY

                // Phase 3: Down-Right
                if denX3 <> 0.0 && denY3 <> 0.0 then
                    let tX = (fst to3 - fst from3) / denX3
                    let tY = (snd to3 - snd from3) / denY3
                    if areFloatsEqual tX tY && areFloatsEqual tX (round tX) && tX > 0.0 then
                        result <- Some power
                elif areFloatsEqual denY3 0.0 && areFloatsEqual (snd to3) (snd from3) && denX3 <> 0.0 then
                    let tX = (fst to3 - fst from3) / denX3
                    if tX > 0.0 && areFloatsEqual tX (round tX) && (snd to3 + tX <= 2.0) then
                        result <- Some power
        
        power <- power + 1
    
    result

let rank (catapult: Entity) (power: int) (target: Entity) =
    let catapultValue = int catapult.Val - int 'A' + 1
    let targetMultiplier = if target.Val = 'H' then 2 else 1
    catapultValue * power * targetMultiplier

let solve (catapults: Entity list) (targets: Entity list) (movingTargets: int) =
    let folder (sum: int option) (target: Entity) =
        match sum with
        | None -> None // If sum is already "infinity", propagate it
        | Some currentSum ->
            let aPos = (catapults |> List.find (fun c -> c.Val = 'A')).Pos
            let dist =
                if movingTargets = 1 then abs(snd aPos - snd target.Pos)
                else abs(fst aPos - fst target.Pos)

            let maxPower = int (ceil (dist * 1.5))

            let ranks =
                catapults
                |> List.choose (fun catapult ->
                    canHitTarget catapult.Pos target.Pos movingTargets 0 maxPower
                    |> Option.map (fun power -> rank catapult power target)
                )

            let finalRanks =
                if not (List.isEmpty ranks) then ranks
                else
                    catapults
                    |> List.choose (fun catapult ->
                        canHitTarget catapult.Pos target.Pos movingTargets 1 maxPower
                        |> Option.map (fun power -> rank catapult power target)
                    )
            
            if not (List.isEmpty finalRanks) then
                Some(currentSum + List.min finalRanks)
            else
                None // No hit possible, result becomes "infinity"
    
    let result = targets |> List.fold folder (Some 0)
    result |> Option.defaultValue -1 // Using -1 to indicate failure/infinity

let execute() =
    let catapultLayout = """
.C...........
.B...........
.A...........
==============
"""
    let targetLines = LocalHelper.GetLinesFromFile(path)
    let catapults, targets = parseContent catapultLayout targetLines
    
    solve catapults targets 1