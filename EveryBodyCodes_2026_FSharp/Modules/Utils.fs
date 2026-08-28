namespace EverybodyCodes_2026_FSharp.Modules

[<AutoOpen>]
module Utils =

    /// Modulo that handles negative numbers correctly
    let inline modn a n = 
        ((a % n) + n) % n