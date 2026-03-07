module quest03_2

open EverybodyCodes_2025_S3_FSharp.Modules
open System

//let path = "quest03/test_input_02.txt"
let path = "quest03/quest03_input_02.txt"

type Color =
    | WHITE
    | BLACK
    | BLUE
    | GREEN
    | RED
    | YELLOW
    | CYAN
    | MAGENTA

type Shape =
    | HEXAGON
    | CIRCLE
    | PENTAGON
    | SQUARE
    | TRIANGLE
    | DIAMOND
    | STAR
    | OCTAGON

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
                | "YELLOW" -> YELLOW
                | "CYAN" -> CYAN
                | "MAGENTA" -> MAGENTA
                | _ -> failwith "invalid color!"
            let shape =
                match s with
                | "HEXAGON" -> HEXAGON
                | "CIRCLE" -> CIRCLE
                | "PENTAGON" -> PENTAGON
                | "SQUARE" -> SQUARE
                | "TRIANGLE" -> TRIANGLE
                | "DIAMOND" -> DIAMOND
                | "STAR" -> STAR
                | "OCTAGON" -> OCTAGON
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

type TreeNode = {
    Node: Node
    mutable LeftChild: int option
    mutable RightChild: int option
}

let matchesWeak (socket: Color * Shape) (plug: Color * Shape) =
    fst socket = fst plug || snd socket = snd plug

let rec connectPart (i: int) (child: int) (nodes: ResizeArray<TreeNode>) : bool =
    let leftDone =
        match nodes[i].LeftChild with
        | Some leftChild -> connectPart leftChild child nodes
        | None ->
            if matchesWeak nodes[i].Node.LeftSocket nodes[child].Node.Plug then
                nodes[i].LeftChild <- Some child
                true
            else
                false
    if leftDone then true
    else
        match nodes[i].RightChild with
        | Some rightChild -> connectPart rightChild child nodes
        | None ->
            if matchesWeak nodes[i].Node.RightSocket nodes[child].Node.Plug then
                nodes[i].RightChild <- Some child
                true
            else
                false

let rec readTree (i: int) (nodes: ResizeArray<TreeNode>) (result: ResizeArray<int>) =
    match nodes[i].LeftChild with
    | Some leftChild -> readTree leftChild nodes result
    | None -> ()
    result.Add(nodes[i].Node.Id)
    match nodes[i].RightChild with
    | Some rightChild -> readTree rightChild nodes result
    | None -> ()

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let parsed = parseContent lines
    let treeNodes = ResizeArray<TreeNode>()
    for node in parsed do
        let pos = treeNodes.Count
        treeNodes.Add({ Node = node; LeftChild = None; RightChild = None })
        if pos > 0 then
            let mutable inserted = false
            while not inserted do
                inserted <- connectPart 0 pos treeNodes
    let order = ResizeArray<int>()
    readTree 0 treeNodes order
    order |> Seq.mapi (fun i id -> (i + 1) * id) |> Seq.sum
