namespace EverybodyCodes_2025_FSharp.Modules

[<AutoOpen>]
module Utils =

    let inline modn a n = 
        ((a % n) + n) % n