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
    let mutable leftRoot = ""
    let mutable rightRoot = ""

    let rec findNode(rank: int) (searchTree: Dictionary<string, Node>) =
        let toRun = searchTree.Values |> Seq.toList
        let mutable foundNode: Node option = None
        let smaller = toRun |> Seq.tryFind(fun n -> n.Rank < rank)
        let larger = toRun |> Seq.tryFind(fun n -> n.Rank > rank)
        

    operations
    |> List.iter(fun op -> 
        let leftNode, rightNode = op.Left, op.Right

        if leftTree.Count = 0 then
            leftTree.Add(op.Left.Name, op.Left)
            leftRoot <- op.Left.Name
        else
            ()
        
        if rightTree.Count = 0 then
            rightTree.Add(op.Right.Name, op.Right)
            rightRoot <- op.Right.Name
        else
            ()

        if nodeDict.Count = 0 then
            nodeDict.Add(op.Left.Name, op.Left)
            lastLeftId <- op.Left.Name
            nodeDict.Add(op.Right.Name, op.Right)
            rightRoot <- op.Right.Name
        else
            let leftChecker = nodeDict[lastLeftId]
            nodeDict.Add(op.Left.Name, op.Left)
            if op.Left.Rank < leftChecker.Rank then
                nodeDict[lastLeftId] <- { nodeDict[lastLeftId] with Left = Some op.Left }
                lastLeftId <- op.Left.Name
            else
                nodeDict[lastLeftId] <- { nodeDict[lastLeftId] with Right = Some op.Left }
            let rightChecker = nodeDict[lastRightId]
            nodeDict.Add(op.Right.Name, op.Right)
            if op.Right.Rank < rightChecker.Rank then
                nodeDict[lastRightId] <- { nodeDict[lastRightId] with Left = Some op.Right }
                rightRoot <- op.Right.Name
            else
                nodeDict[lastRightId] <- { nodeDict[lastRightId] with Right = Some op.Right }
    )
    nodeDict

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let nodes = parseContent(lines)
    buildTree(nodes) |> ignore
    0
