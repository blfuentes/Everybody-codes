module quest02_part01

open EverybodyCodes_2024_S1_FSharp.Modules
open System.Collections.Generic

//let path = "quest02/test_input_01.txt"
let path = "quest02/quest02_input_01.txt"

type Node = {
    Id: string;
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
    let nodes = 
        seq {
            for line in lines do
                let parts = line.Split(" ")
                let id = int(parts[1].Split("=")[1])
                let leftValue = int((parts[2].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[0])
                let leftName = (parts[2].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[1]
                let rightValue = int((parts[3].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[0])
                let rightName = (parts[3].Split("=")[1]).Replace("[", "").Replace("]", "").Split(",")[1]
                let left = { Id = $"{id}_{leftName}"; Name = leftName; Rank = leftValue; Left = None; Right = None }
                let right = { Id = $"{id}_{rightName}"; Name = rightName; Rank = rightValue; Left = None; Right = None }
                yield { Id = id; Left = left; Right = right }
        } |> Seq.toList
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
                    currentNode <- searchTree[node.Id]
                | None ->
                    side <- -1
                    doContinue <- false
            else
                match currentNode.Right with
                | Some node ->
                    currentNode <- searchTree[node.Id]
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
                leftTree[node.Id] <- { leftTree[node.Id] with Right = Some leftNode }
            else // left
                leftTree[node.Id] <- { leftTree[node.Id] with Left = Some leftNode }

        leftTree.Add(op.Left.Id, op.Left)
        
        if rightTree.Count > 0 then
            let (side, node) = findNode op.Right.Rank rightTree
            if side = 1 then // right
                rightTree[node.Id] <- { rightTree[node.Id] with Right = Some rightNode }
            else // left
                rightTree[node.Id] <- { rightTree[node.Id] with Left = Some rightNode }

        rightTree.Add(op.Right.Id, op.Right)
    )
    (leftTree, rightTree)
   

let buildWord((leftTree, rightTree): Dictionary<string, Node>*Dictionary<string, Node>) =
    let leftNodesByLevel = new Dictionary<int, Node list>()
    let rightNodesByLevel = new Dictionary<int, Node list>()

    let leftRoot = (leftTree.Values |> Array.ofSeq)[0]
    leftNodesByLevel.Add(1, [leftRoot])
    let rightRoot = (rightTree.Values |> Array.ofSeq)[0]
    rightNodesByLevel.Add(1, [rightRoot])

    let mutable currentLevel = 2
    // left tree
    let mutable currentNodeLevel = leftNodesByLevel[currentLevel - 1]
    while currentNodeLevel.Length > 0 do
        currentNodeLevel <- 
            currentNodeLevel 
            |> List.collect(fun n -> 
                match n.Left, n.Right with
                | Some nl, Some nr ->
                    [leftTree[nl.Id]; leftTree[nr.Id]]
                | Some nl, _ -> [leftTree[nl.Id]]
                | _, Some nr -> [leftTree[nr.Id]]
                | _ -> [])
        leftNodesByLevel.Add(currentLevel, currentNodeLevel)
        currentLevel <- currentLevel + 1
     
    let mutable currentLevel = 2
    // right tree
    let mutable currentNodeLevel = rightNodesByLevel[currentLevel - 1]
    while currentNodeLevel.Length > 0 do
        currentNodeLevel <- 
            currentNodeLevel 
            |> List.collect(fun n -> 
                match n.Left, n.Right with
                | Some nl, Some nr ->
                    [rightTree[nl.Id]; rightTree[nr.Id]]
                | Some nl, _ -> [rightTree[nl.Id]]
                | _, Some nr -> [rightTree[nr.Id]]
                | _ -> [])
        rightNodesByLevel.Add(currentLevel, currentNodeLevel)
        currentLevel <- currentLevel + 1 

    // 
    let maxLevel =
        leftNodesByLevel
        |> Seq.maxBy(fun level -> level.Value.Length)
    let names =
        maxLevel.Value
        |> Seq.sortBy _.Rank
        |> Seq.map _.Name
    let leftName = String.concat "" names
    let maxLevel =
        rightNodesByLevel
        |> Seq.maxBy(fun level -> level.Value.Length)
    let names =
        maxLevel.Value
        |> Seq.sortBy _.Rank
        |> Seq.map _.Name
    let rightName = String.concat "" names
    leftName + rightName


let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    let nodes = parseContent(lines)
    let (leftTree, rightTree) = buildTree(nodes)
    buildWord (leftTree, rightTree)
