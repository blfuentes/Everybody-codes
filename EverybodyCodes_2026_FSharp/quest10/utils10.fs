module quest10_utilities


type Nullghost = {
    Id: int
    Pos: int*int
    Dir: char
    Life: bigint
}

let movOfDir d =
    match d with
    | '>' -> (0, 1)
    | 'v' -> (1, 0)
    | '<' -> (0, -1)
    | '^' -> (-1,0)
    | _ -> failwith "Invalid direction move"

let rotate90 d =
    match d with
    | '>' -> 'v'
    | 'v' -> '<'
    | '<' -> '^'
    | '^' -> '>'
    | _ -> failwith "Invalid direction 90"

let rotate180 d=
    match d with
    | '>' -> '<'
    | 'v' -> '^'
    | '<' -> '>'
    | '^' -> 'v'
    | _ -> failwith "invalid direction 180"

let restorePos (ghost: Nullghost) =
    let (r, c) = ghost.Pos
    match ghost.Dir with
    | '>' -> { ghost with Pos = (r, c - 1) }
    | 'v' -> { ghost with Pos = (r - 1, c) }
    | '<' -> { ghost with Pos = (r, c + 1) }
    | '^' -> { ghost with Pos = (r + 1, c) }
    | _ -> failwith "Invalid restore dir"

let move ((ax,ay), (bx, by)) =
    (ax + bx, ay + by)