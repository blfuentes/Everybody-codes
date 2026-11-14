module quest15_3

open System.Collections.Generic
open EverybodyCodes_2026_FSharp.Modules
open quest15_utilities

//let path = "quest15/test_input_03.txt"
let path = "quest15/quest15_input_03.txt"

let findShortestRoute (requiredDeposits: int64) (cave: Cave) =
    let directions = availableMoves (0, 0)
    let depositStates, stateIndices = buildDepositStates cave directions

    let routesFrom (startPosition, initialDirection) =
        let visited = HashSet<Point * Point>()
        let reachedDeposits = HashSet<Point * Point>()
        let queue = Queue<Point * Point * int>()
        let nextDeposits = ResizeArray<int * int64>()
        let mutable distanceToFinish = None

        visited.Add(startPosition, initialDirection) |> ignore
        queue.Enqueue(startPosition, initialDirection, 0)

        while queue.Count > 0 do
            let (position, previousDirection, distance) = queue.Dequeue()

            if position = cave.Finish && Option.isNone distanceToFinish then
                distanceToFinish <- Some(int64 distance)

            for direction in availableMoves previousDirection do
                let nextPosition =
                    (fst position + fst direction, snd position + snd direction)
                let nextState = (nextPosition, direction)

                if cave.Walkable.Contains nextPosition then
                    if cave.Deposits.Contains nextPosition then
                        if reachedDeposits.Add nextState then
                            nextDeposits.Add(stateIndices[nextState], int64 distance + 1L)
                    elif visited.Add nextState then
                        queue.Enqueue(nextPosition, direction, distance + 1)

        nextDeposits.ToArray(), distanceToFinish

    let firstDeposits, directFinish = routesFrom (cave.Start, (0, 0))

    if requiredDeposits = 0L then
        directFinish
        |> Option.defaultWith (fun () -> failwith "cannot get to end with expected sulphur")
    else
        let routes = depositStates |> Seq.map routesFrom |> Array.ofSeq
        let infinity = System.Int64.MaxValue
        let mutable distances = Array.create depositStates.Count infinity

        for (stateIndex, distance) in firstDeposits do
            distances[stateIndex] <- min distances[stateIndex] distance

        let seenDistancePatterns = Dictionary<int64 array, int64 * int64>(HashIdentity.Structural)
        let mutable collectedDeposits = 1L
        let mutable cycleSkipped = false

        while collectedDeposits < requiredDeposits do
            let minimumDistance = Array.fold min infinity distances

            if not cycleSkipped then
                let normalizedDistances =
                    distances
                    |> Array.map (fun distance ->
                        if distance = infinity then infinity
                        else distance - minimumDistance)

                match seenDistancePatterns.TryGetValue normalizedDistances with
                | true, (previousDepositCount, previousMinimumDistance) ->
                    let cycleLength = collectedDeposits - previousDepositCount
                    let completeCycles = (requiredDeposits - collectedDeposits) / cycleLength
                    let skippedDistance =
                        Checked.(*) completeCycles (minimumDistance - previousMinimumDistance)

                    distances <-
                        distances
                        |> Array.map (fun distance ->
                            if distance = infinity then infinity
                            else Checked.(+) distance skippedDistance)

                    collectedDeposits <- collectedDeposits + completeCycles * cycleLength
                    cycleSkipped <- true
                | false, _ ->
                    seenDistancePatterns.Add(
                        normalizedDistances,
                        (collectedDeposits, minimumDistance)
                    )

            if collectedDeposits < requiredDeposits then
                let nextDistances = Array.create depositStates.Count infinity

                for stateIndex in 0 .. depositStates.Count - 1 do
                    if distances[stateIndex] <> infinity then
                        let (nextDeposits, _) = routes[stateIndex]

                        for (nextStateIndex, routeLength) in nextDeposits do
                            let distance = Checked.(+) distances[stateIndex] routeLength

                            if distance < nextDistances[nextStateIndex] then
                                nextDistances[nextStateIndex] <- distance

                distances <- nextDistances
                collectedDeposits <- collectedDeposits + 1L

        let mutable answer = None

        for stateIndex in 0 .. depositStates.Count - 1 do
            let (_, finishDistance) = routes[stateIndex]

            match finishDistance with
            | Some finalLeg when distances[stateIndex] <> infinity ->
                let totalDistance = Checked.(+) distances[stateIndex] finalLeg

                answer <-
                    Some(
                        match answer with
                        | Some bestDistance -> min bestDistance totalDistance
                        | None -> totalDistance
                    )
            | _ -> ()

        answer
        |> Option.defaultWith (fun () -> failwith "cannot get to end with expected sulphur")

let execute () =
    let lines = LocalHelper.GetContent path 2026 15 3
    lines |> parseMap |> findShortestRoute 202620262026L
