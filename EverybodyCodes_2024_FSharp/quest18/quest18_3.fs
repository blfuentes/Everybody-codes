module quest18_3

open EverybodyCodes_2024_FSharp.Modules
open System.Collections.Generic

//let path = "quest18/test_input_03.txt"
let path = "quest18/quest18_input_03.txt"

let parseContent (lines: string array) =
    lines |> Array.map (fun line -> line.ToCharArray())

let map2d (map: 'a[][]) (cb: 'a -> int -> int -> 'b) : 'b[][] =
    map
    |> Array.mapi (fun y row ->
        row
        |> Array.mapi (fun x value ->
            cb value x y
        )
    )

let DIRS = [(1, 0); (0, 1); (-1, 0); (0, -1)]

let distanceMap (map: char[][]) (startingPoints: (int * int) list) =
    let rows = map.Length
    let cols = map[0].Length
    let dmap = Array2D.create rows cols System.Int32.MaxValue
    
    let queue = new Queue<(int * int * int)>()
    
    startingPoints |> List.iter (fun (x, y) ->
        dmap[y, x] <- 0
        queue.Enqueue(x, y, 0)
    )

    while queue.Count > 0 do
        let (x, y, dist) = queue.Dequeue()

        for (dx, dy) in DIRS do
            let nx = x + dx
            let ny = y + dy

            if nx >= 0 && nx < cols && ny >= 0 && ny < rows && map[ny][nx] <> '#' then
                if dmap[ny, nx] > dist + 1 then
                    dmap[ny, nx] <- dist + 1
                    queue.Enqueue(nx, ny, dist + 1)
    dmap

let fillWater (map: char[][]) =
    let palms =
        map2d map (fun value x y ->
            if value = 'P' then Some(x, y) else None
        )
        |> Array.concat
        |> Array.choose id
        |> List.ofArray

    let dmaps = palms |> List.map (fun (x, y) -> distanceMap map [(x, y)])

    let totalDistances =
        map2d map (fun value x y ->
            if value = '.' then
                dmaps
                |> List.sumBy (fun dm -> dm[y, x])
            else
                System.Int32.MaxValue
        )

    totalDistances
    |> Array.concat
    |> Array.min


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let map = parseContent lines
    fillWater map
