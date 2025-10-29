module quest02_part01

open EverybodyCodes_2024_S1_FSharp.Modules
open System.Collections.Generic

let path = "quest02/test_input_01.txt"
//let path = "quest02/quest02_input_01.txt"

type Node = {
    Name: string;
    Rank: int;
    Left: Node option;
    Right: Node option;
}

type Operation = {
    Id: int;
    Left: Node;
    Right: Node;
}

let parseContent(lines: string array) =
    //let root = { Name = "root"; Rank = 0; Left = None; Right = None }
    let nodes = 
        seq {
            for line in lines do
                let parts = line.Split(" ")
                let id = int(parts[1].Split("=")[1])
                let leftValue = int((parts[2].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[0])
                let leftName = (parts[2].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[1]
                let rightValue = int((parts[3].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[0])
                let rightName = (parts[3].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[1]
                let left = { Name = leftName; Rank = leftValue; Left = None; Right = None }
                let right = { Name = rightName; Rank = rightValue; Left = None; Right = None }
                yield { Id = id; Left = left; Right = right }
        } |> Seq.toList
    //[(0,root)] @ nodes
    nodes

let buildTree(operations: Operation list) =
    let leftTree = new Dictionary<string, Node>()
    let rightTree = new Dictionary<string, Node>()

    let findNode(rank: int) (searchTree: Dictionary<string, Node>) =
        let toRun = searchTree.Values |> Seq.toArray
        let mutable doContinue = true
        let mutable currentNode = toRun[0]
        let mutable side = 0
        while doContinue do
            if currentNode.Rank > rank then
                match currentNode.Left with
                | Some node -> 
                    currentNode <- searchTree[node.Name]
                | None ->
                    side <- -1
                    doContinue <- false
            else
                match currentNode.Right with
                | Some node ->
                    currentNode <- searchTree[node.Name]
                | None ->
                    side <- 1
                    doContinue <- false

        (side, currentNode)
        

    operations
    |> List.iter(fun op -> 
        let leftNode, rightNode = op.Left, op.Right

        if leftTree.Count > 0 then
            let (side, node) = findNode op.Left.Rank leftTree
            if side = 1 then // right
                leftTree[node.Name] <- { leftTree[node.Name] with Right = Some leftNode }
            else // left
                leftTree[node.Name] <- { leftTree[node.Name] with Left = Some leftNode }

        leftTree.Add(op.Left.Name, op.Left)
        
        if rightTree.Count > 0 then
            let (side, node) = findNode op.Right.Rank rightTree
            if side = 1 then // right
                rightTree[node.Name] <- { rightTree[node.Name] with Right = Some rightNode }
            else // left
                rightTree[node.Name] <- { rightTree[node.Name] with Left = Some rightNode }

        rightTree.Add(op.Right.Name, op.Right)
    )
    (leftTree, rightTree)
   

let buildWord((leftTree, rightTree): Dictionary<string, Node>*Dictionary<string, Node>) =
    0


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let nodes = parseContent(lines)
    let (leftTree, rightTree) = buildTree(nodes)
    0
