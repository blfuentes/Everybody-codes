module quest15_2

open System.Collections.Generic
open EverybodyCodes_2026_FSharp.Modules
open quest15_utilities

//let path = "quest15/test_input_02.txt"
let path = "quest15/quest15_input_02.txt"

let findShortestRoute (requiredDeposits: int) (cave: Cave) =
    let directions = availableMoves (0, 0)
    let depositStates, stateIndices = buildDepositStates cave directions

    let routesFrom (startPosition, initialDirection) =
        let visited = HashSet<Point * Point>()
        let reachedDeposits = HashSet<Point * Point>()
        let queue = Queue<Point * Point * int>()
        let nextDeposits = ResizeArray<int * int>()

        visited.Add(startPosition, initialDirection) |> ignore
        queue.Enqueue(startPosition, initialDirection, 0)

        while queue.Count > 0 do
            let (position, previousDirection, distance) = queue.Dequeue()

            for direction in availableMoves previousDirection do
                let nextPosition =
                    (fst position + fst direction, snd position + snd direction)
                let nextState = (nextPosition, direction)

                if cave.Walkable.Contains nextPosition then
                    if cave.Deposits.Contains nextPosition then
                        if reachedDeposits.Add nextState then
                            nextDeposits.Add(stateIndices[nextState], distance + 1)
                    elif visited.Add nextState then
                        queue.Enqueue(nextPosition, direction, distance + 1)

        nextDeposits.ToArray()

    let distanceToFinish = Dictionary<Point * Point, int>()
    let queue = Queue<Point * Point>()

    for direction in directions do
        let finishState = (cave.Finish, direction)
        distanceToFinish.Add(finishState, 0)
        queue.Enqueue finishState

    while queue.Count > 0 do
        let (position, direction) = queue.Dequeue()
        let distance = distanceToFinish[(position, direction)]
        let previousPosition =
            (fst position - fst direction, snd position - snd direction)

        if cave.Walkable.Contains previousPosition then
            for previousDirection in directions do
                if previousDirection <> (-fst direction, -snd direction) then
                    let previousState = (previousPosition, previousDirection)

                    if distanceToFinish.TryAdd(previousState, distance + 1) then
                        queue.Enqueue previousState

    let infinity = System.Int32.MaxValue
    let mutable answer = infinity

    if requiredDeposits = 0 then
        if cave.Start = cave.Finish then
            answer <- 0

        for direction in directions do
            let nextPosition =
                (fst cave.Start + fst direction, snd cave.Start + snd direction)
            let nextState = (nextPosition, direction)

            match distanceToFinish.TryGetValue nextState with
            | true, distance -> answer <- min answer (distance + 1)
            | false, _ -> ()
    else
        let routes = depositStates |> Seq.map routesFrom |> Array.ofSeq
        let mutable distances = Array.create depositStates.Count infinity

        for (stateIndex, distance) in routesFrom (cave.Start, (0, 0)) do
            distances[stateIndex] <- min distances[stateIndex] distance

        for _ in 2 .. requiredDeposits do
            let nextDistances = Array.create depositStates.Count infinity

            for stateIndex in 0 .. depositStates.Count - 1 do
                if distances[stateIndex] <> infinity then
                    for (nextStateIndex, routeLength) in routes[stateIndex] do
                        let distance = Checked.(+) distances[stateIndex] routeLength

                        if distance < nextDistances[nextStateIndex] then
                            nextDistances[nextStateIndex] <- distance

            distances <- nextDistances

        for stateIndex in 0 .. depositStates.Count - 1 do
            if distances[stateIndex] <> infinity then
                match distanceToFinish.TryGetValue depositStates[stateIndex] with
                | true, remainingDistance ->
                    answer <- min answer (Checked.(+) distances[stateIndex] remainingDistance)
                | false, _ -> ()

    if answer = infinity then
        failwith "cannot collect enough sulphur"

    answer

let execute () =
    let lines = LocalHelper.GetContent path 2026 15 2
    lines |> parseMap |> findShortestRoute 2026
