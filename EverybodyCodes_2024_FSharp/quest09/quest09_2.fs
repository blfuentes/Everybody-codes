module quest09_part02

open System
open System.Collections.Generic
open EverybodyCodes_2024_FSharp.Modules

//let path = "quest09/test_input_02.txt"
let path = "quest09/quest09_input_02.txt"

let findBeetlesForBrightness (beetles: int list) (brightness: int) =
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
        None
    else
        let conteo = Dictionary<int, int>()
        for m in beetles do conteo.[m] <- 0

        let mutable valor = brightness
        while valor > 0 do
            let m = lastBeetle.[valor]
            conteo.[m] <- conteo.[m] + 1
            valor <- valor - m

        Some (dp.[brightness], conteo |> Seq.map (fun kv -> kv.Key, kv.Value) |> dict)


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let brightnesses = lines |> Array.map int
    let availableDots = [1; 3; 5; 10; 15; 16; 20; 24; 25; 30]
    let result = 
        brightnesses 
        |> Array.map (fun b -> findBeetlesForBrightness availableDots b)
    result
    |> Array.sumBy (function
        | Some (count, _) -> count
        | None -> 0)
