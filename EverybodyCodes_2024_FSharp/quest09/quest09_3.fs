module quest09_3

open System
open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest09/test_input_03.txt"
let path = "quest09/quest09_input_03.txt"

let beetlesForBrightness = Dictionary<int, (int * IDictionary<int, int>) option>()

let findBeetlesForBrightness (beetles: int list) (brightness: int) =
    if beetlesForBrightness.ContainsKey(brightness) then
        beetlesForBrightness.[brightness]
    else        
        let dp = Array.create (brightness + 1) Int32.MaxValue
        let lastBeetle = Array.create (brightness + 1) -1
        dp[0] <- 0

        for i in 1 .. brightness do
            for beetle in beetles do
                if i >= beetle && dp[i - beetle] <> Int32.MaxValue then
                    let posible = dp[i - beetle] + 1
                    if posible < dp[i] then
                        dp[i] <- posible
                        lastBeetle[i] <- beetle

        if dp.[brightness] = Int32.MaxValue then
            beetlesForBrightness[brightness] <- None
            None
        else
            let conteo = Dictionary<int, int>()
            for m in beetles do conteo.[m] <- 0

            let mutable valor = brightness
            while valor > 0 do
                let m = lastBeetle.[valor]
                conteo.[m] <- conteo.[m] + 1
                valor <- valor - m

            let res = Some (dp.[brightness], conteo |> Seq.map (fun kv -> kv.Key, kv.Value) |> dict)
            beetlesForBrightness[brightness] <- res
            res

let getSumOfBeetles beetlesCount =
    beetlesCount
    |> Array.sumBy (function
        | Some (count, _) -> count
        | None -> 0)

let lowestBeetles (brightness: int) (beetles: int list) =
    //printfn "Processing brightness %d" brightness
    let findPairs b =        
        [for a in 1..b/2 do
            yield a, b-a
        ]
    let possiblepars = 
        findPairs brightness
        |> List.filter(fun (b1, b2) -> Math.Abs(b1-b2) <= 100)

    possiblepars
    |> List.map (fun (b1, b2) -> 
        let r1 = findBeetlesForBrightness beetles b1
        let r2 = findBeetlesForBrightness beetles b2
        match r1, r2 with
        | Some (c1, _), Some (c2, _) -> 
            c1+c2
        | _ , _ -> 
            -1)
    |> List.filter (fun x -> x <> -1)
    |> List.minBy (fun x -> x)

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let brightnesses = lines |> Array.map int
    let availableDots = 
        [1; 3; 5; 10; 15; 16; 20; 24; 25; 30; 37; 38; 49; 50; 74; 75; 100; 101]
    brightnesses
    |> Array.sumBy (fun b -> lowestBeetles b availableDots)
