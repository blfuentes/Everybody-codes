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
        let v' = 
            String.concat ""(value.ToCharArray()
            |> Array.map(fun v -> if v > 'Z' then 0 else 1)
            |> Array.map string)
        System.Convert.ToInt32(v', 2)

    let inline getShine(value: int) =
        match value with
        | s when s <= 30 -> Matte
        | s when s >= 33 -> Shiny
        | _ -> None

    let identifiers =
        lines
        |> Seq.map(fun l ->
            (
                l.Split(":")[0] |> int),
                convertToNum((l.Split(":")[1]).Split(" ")[0]),
                convertToNum((l.Split(":")[1]).Split(" ")[1]),
                convertToNum((l.Split(":")[1]).Split(" ")[2]),
                convertToNum((l.Split(":")[1]).Split(" ")[3])
        )
    identifiers
    |> Seq.map(fun (id, red, green, blue, shine) ->
        let shine' = getShine shine
        let color =
            if red > green && red > blue then
                Red
            elif (green > red && green > blue) then
                Green
            elif(blue > red && blue > green) then
                Blue
            else
                ColorType.None
                
        { Id = id; Red = red; Green = green; Blue = blue; Shine = shine; Group = (color, shine') }
    )

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent lines
    |> Seq.filter(fun d -> 
        let (c, s) = d.Group
        not s.IsNone
    )
    |> Seq.groupBy(fun d -> d.Group)
    |> Seq.map(fun (g, l) -> l)
    |> Seq.sortByDescending Seq.length
    |> Seq.head
    |> Seq.sumBy _.Id