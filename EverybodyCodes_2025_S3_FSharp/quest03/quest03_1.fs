module quest03_1

open EverybodyCodes_2025_S3_FSharp.Modules
open System.Text.RegularExpressions
open System.IO
open System

let path = "quest03/test_input_01.txt"
// let path = "quest03/quest03_input_01.txt"

type Color =
    | WHITE
    | BLACK
    | BLUE
    | GREEN
    | RED

type Shape =
    | HEXAGON
    | CIRCLE
    | PENTAGON
    | SQUARE
    | TRIANGLE

type Node = {
    Id: int
    Plug: Color * Shape
    LeftSocket: Color * Shape
    RightSocket: Color * Shape
    Data: string
}

let parseContent (lines: string array) =    
    let parseLine (line: string) =
        let parsePart (part: string) =
            let (c, s) = (part.Split(" ")[0], part.Split(" ")[1])
            let color =
                match c with
                | "WHITE" -> WHITE
                | "BLACK" -> BLACK
                | "BLUE" -> BLUE
                | "GREEN" -> GREEN
                | "RED" -> RED
                | _ -> failwith "invalid color!"
            let shape =
                match s with
                | "HEXAGON" -> HEXAGON
                | "CIRCLE" -> CIRCLE
                | "PENTAGON" -> PENTAGON
                | "SQUARE" -> SQUARE
                | "TRIANGLE" -> TRIANGLE
                | _ -> failwith "invalid shape!"
            (color, shape)

        let parts = line.Split(",")
        let id = int(parts[0].Split("=")[1])
        let plug = parsePart (parts[1].Split("=")[1])
        let leftSocket = parsePart (parts[2].Split("=")[1])
        let rightSocket = parsePart (parts[3].Split("=")[1])
        let data = parts[4].Split("=")[1]
        {
            Id = id;
            Plug = plug;
            LeftSocket = leftSocket;
            RightSocket = rightSocket;
            Data = data
        }

    lines |> Array.map parseLine

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let nodes = parseContent lines
    nodes.Length
