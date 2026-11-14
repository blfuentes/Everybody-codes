module quest15_1

open System.Collections.Generic
open EverybodyCodes_2026_FSharp.Modules
open quest15_utilities

//let path = "quest15/test_input_01.txt"
let path = "quest15/quest15_input_01.txt"

let findShortestRoute (cave: Cave) =
    let depositBits = Dictionary<Point, bigint>()
    cave.Deposits
    |> Seq.iteri (fun id position -> depositBits.Add(position, bigint.One <<< id))

    let allDepositsCollected = (bigint.One <<< cave.Deposits.Count) - bigint.One
    let initialDirection = (0, 0)
    let visited = HashSet<Point * Point * bigint>()
    let queue = Queue<Point * Point * bigint * int>()

    visited.Add(cave.Start, initialDirection, bigint.Zero) |> ignore
    queue.Enqueue(cave.Start, initialDirection, bigint.Zero, 0)

    let mutable answer = None
    while Option.isNone answer && queue.Count > 0 do
        let (position, previousDirection, collectedDeposits, distance) = queue.Dequeue()

        if position = cave.Finish && collectedDeposits = allDepositsCollected then
            answer <- Some distance
        else
            for direction in availableMoves previousDirection do
                let nextPosition =
                    (fst position + fst direction, snd position + snd direction)

                if cave.Walkable.Contains nextPosition then
                    let nextCollectedDeposits =
                        match depositBits.TryGetValue nextPosition with
                        | true, bit -> collectedDeposits ||| bit
                        | false, _ -> collectedDeposits

                    if visited.Add(nextPosition, direction, nextCollectedDeposits) then
                        queue.Enqueue(nextPosition, direction, nextCollectedDeposits, distance + 1)

    answer
    |> Option.defaultWith (fun () -> failwith "cannot collect all sulphurs")

let execute () =
    let lines = LocalHelper.GetContent path 2026 15 1
    lines |> parseMap |> findShortestRoute
