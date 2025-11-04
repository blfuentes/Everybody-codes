namespace EverybodyCodes_2025_FSharp.Modules

[<AutoOpen>]
module Utils =

    /// Modulo that handles negative numbers correctly
    let inline modn a n = 
        ((a % n) + n) % n