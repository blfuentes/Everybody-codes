module quest02_part03

open EverybodyCodes_2024_S1_FSharp.Modules
open System.Collections.Generic

//let path = "quest02/test_input_03.txt"
let path = "quest02/quest02_input_03.txt"

type Node = {
    Id: string;
    Name: string;
    Rank: int;
    Parent: Node option;
    Left: Node option;
    Right: Node option;
}

type AddOperation = {
    Id: int;
    Left: Node;
    Right: Node;
}

type SwapOperation = {
    Id: int;
}

type OpType =
    | ADD of AddOperation
    | SWAP of SwapOperation

let parseContent(lines: string array) : OpType list =
    let nodes = 
        seq {
            for line in lines do
                let parts = line.Split(" ")
                if parts.Length > 2 then
                    let id = int(parts[1].Split("=")[1])
                    let leftValue = int((parts[2].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[0])
                    let leftName = (parts[2].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[1]
                    let rightValue = int((parts[3].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[0])
                    let rightName = (parts[3].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[1]
                    let left = { Id = $"{id}_{leftName}"; Name = leftName; Rank = leftValue; Parent = None; Left = None; Right = None }
                    let right = { Id = $"{id}_{rightName}"; Name = rightName; Rank = rightValue; Parent = None; Left = None; Right = None }
                    yield ADD { Id = id; Left = left; Right = right }
                else
                    yield SWAP { Id = int(parts[1]) }
        } |> Seq.toList
    nodes

type TreeType =
    | LEFT
    | RIGHT

let buildTree(operations: OpType list) =
    let tree = new Dictionary<string, Node>()
    tree.Add("0_root", 
        { 
            Id = "0_root"; 
            Name = "root";
            Rank = 0;
            Parent = None;
            Left = None;
            Right = None;
        });
    let findNode(rank: int) (treeSide: TreeType) =
        let mutable doContinue = true
        let mutable currentNode = if treeSide.IsLEFT then tree["0_root"].Left else tree["0_root"].Right
        let mutable side = 0
        if currentNode.IsNone then
            match treeSide with
            | LEFT -> side <- -1
            | RIGHT -> side <- 1
        elif currentNode.IsSome then
            while doContinue do
                if currentNode.Value.Rank > rank then
                    let leftNode = tree[currentNode.Value.Id].Left
                    match leftNode with
                    | Some node -> 
                        currentNode <- Some tree[node.Id]
                    | None ->
                        side <- -1
                        doContinue <- false
                else
                    let rightNode = tree[currentNode.Value.Id].Right
                    match rightNode with
                    | Some node ->
                        currentNode <- Some tree[node.Id]
                    | None ->
                        side <- 1
                        doContinue <- false

        (side, currentNode)
        
    operations
    |> List.iter(fun o -> 
        match o with
        | ADD op ->
            let leftNode, rightNode = op.Left, op.Right

            let (side, node) = findNode op.Left.Rank LEFT

            match node with
            | Some n ->
                if side = 1 then // right
                    tree[n.Id] <- { tree[n.Id] with Right = Some leftNode }
                else // left
                    tree[n.Id] <- { tree[n.Id] with Left = Some leftNode }
            | None -> 
                tree["0_root"] <- { tree["0_root"] with Left = Some leftNode }

            tree.Add(op.Left.Id, { op.Left with Parent = if node.IsSome then Some node.Value else Some tree["0_root"] })

            let (side, node) = findNode op.Right.Rank RIGHT

            match node with
            | Some n ->
                if side = 1 then // right
                    tree[n.Id] <- { tree[n.Id] with Right = Some rightNode }
                else // left
                    tree[n.Id] <- { tree[n.Id] with Left = Some rightNode }
            | None -> 
                tree["0_root"] <- { tree["0_root"] with Right = Some rightNode }

            tree.Add(op.Right.Id, { op.Right with Parent = if node.IsSome then Some node.Value else Some tree["0_root"] })
            
        | SWAP op ->
            let tmpOp = operations |> List.find(fun x -> 
                match x with
                | ADD addOp when addOp.Id = op.Id -> true
                | _ -> false)
            match tmpOp with
            | ADD addOp ->
                let leftNode = tree[addOp.Left.Id]
                let rightNode = tree[addOp.Right.Id]

                let leftParent = tree[leftNode.Parent.Value.Id]
                let rightParent = tree[rightNode.Parent.Value.Id]
                if leftNode.Parent.IsSome then
                    match leftParent.Left, leftParent.Right with
                    | Some l, _ when l.Id = leftNode.Id ->
                        tree[leftParent.Id] <- { tree[leftParent.Id] with Left = Some rightNode }
                    | _, Some r when r.Id = leftNode.Id ->
                        tree[leftParent.Id] <- { tree[leftParent.Id] with Right = Some rightNode }
                    | _ -> ()
                tree[rightNode.Id] <- { rightNode with Parent = leftNode.Parent }

                if rightNode.Parent.IsSome then
                    match rightParent.Left, rightParent.Right with
                    | Some l, _ when l.Id = rightNode.Id ->
                        tree[rightParent.Id] <- { tree[rightParent.Id] with Left = Some leftNode }
                    | _, Some r when r.Id = rightNode.Id ->
                        tree[rightParent.Id] <- { tree[rightParent.Id] with Right = Some leftNode }
                    | _ -> ()
                tree[leftNode.Id] <- { leftNode with Parent = rightNode.Parent }
            | _ -> ()
    )
    tree
   

let buildWord(tree: Dictionary<string, Node>) =
    let leftNodesByLevel = new Dictionary<int, Node list>()
    let rightNodesByLevel = new Dictionary<int, Node list>()

    let leftRoot = tree[tree["0_root"].Left.Value.Id]
    leftNodesByLevel.Add(0, [leftRoot])
    let rightRoot = tree[tree["0_root"].Right.Value.Id]
    rightNodesByLevel.Add(0, [rightRoot])

    let mutable currentLevel = 1
    // left tree
    let mutable currentNodeLevel = leftNodesByLevel[currentLevel - 1]
    while currentNodeLevel.Length > 0 do
        currentNodeLevel <- 
            currentNodeLevel 
            |> List.collect(fun n -> 
                match n.Left, n.Right with
                | Some nl, Some nr ->
                    [tree[nl.Id]; tree[nr.Id]]
                | Some nl, _ -> [tree[nl.Id]]
                | _, Some nr -> [tree[nr.Id]]
                | _ -> [])
        leftNodesByLevel.Add(currentLevel, currentNodeLevel)
        currentLevel <- currentLevel + 1
     
    let mutable currentLevel = 1
    // right tree
    let mutable currentNodeLevel = rightNodesByLevel[currentLevel - 1]
    while currentNodeLevel.Length > 0 do
        currentNodeLevel <- 
            currentNodeLevel 
            |> List.collect(fun n -> 
                match n.Left, n.Right with
                | Some nl, Some nr ->
                    [tree[nl.Id]; tree[nr.Id]]
                | Some nl, _ -> [tree[nl.Id]]
                | _, Some nr -> [tree[nr.Id]]
                | _ -> [])
        rightNodesByLevel.Add(currentLevel, currentNodeLevel)
        currentLevel <- currentLevel + 1 

    // 
    let maxLevel =
        leftNodesByLevel
        |> Seq.maxBy(fun level -> level.Value.Length)
    let names =
        maxLevel.Value
        |> Seq.map _.Name
    let leftName = String.concat "" names
    let maxLevel =
        rightNodesByLevel
        |> Seq.maxBy(fun level -> level.Value.Length)
    let names =
        maxLevel.Value
        |> Seq.map _.Name
    let rightName = String.concat "" names
    leftName + rightName


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let nodes = parseContent(lines)
    let tree = buildTree(nodes)
    buildWord tree
