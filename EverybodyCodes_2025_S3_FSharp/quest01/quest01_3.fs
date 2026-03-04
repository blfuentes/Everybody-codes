module quest01_3

open EverybodyCodes_2025_S3_FSharp.Modules

//let path = "quest01/test_input_03.txt"
let path = "quest01/quest01_input_03.txt"

type ColorType =
    | Red
    | Green
    | Blue
    | None

type ShineType =
    | Shiny
    | Matte
    | None

type Identifier = {
    Id: int
    Red: int
    Green: int
    Blue: int
    Shine: int
    Group: ColorType * ShineType
}

let parseContent(lines: string seq) =
    let inline convertToNum(value: string) =
        value |> Seq.fold (fun acc v -> (acc <<< 1) ||| (if v > 'Z' then 0 else 1)) 0

    let inline getShine(value: int) =
        match value with
        | s when s <= 30 -> Matte
        | s when s >= 33 -> Shiny
        | _ -> None

    lines
    |> Seq.toArray
    |> Array.map(fun l ->
        let parts = l.Split(':')
        let dnas  = parts.[1].Split(' ')
        let id    = parts.[0] |> int
        let red   = convertToNum dnas.[0]
        let green = convertToNum dnas.[1]
        let blue  = convertToNum dnas.[2]
        let shine = convertToNum dnas.[3]
        let shine' = getShine shine
        let color =
            if red > green && red > blue then Red
            elif green > red && green > blue then Green
            elif blue > red && blue > green then Blue
            else ColorType.None
        { Id = id; Red = red; Green = green; Blue = blue; Shine = shine; Group = (color, shine') }
    )

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent lines
    |> Array.filter(fun d -> 
        let (_, s) = d.Group
        not s.IsNone
    )
    |> Array.groupBy(fun d -> d.Group)
    |> Array.maxBy(fun (_, l) -> l.Length)
    |> snd
    |> Array.sumBy _.Id