module quest02_part03

open EverybodyCodes_2024_S1_FSharp.Modules
open System.Collections.Generic

let path = "quest02/test_input_03.txt"
//let path = "quest02/quest02_input_03.txt"

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
    let leftTree = new Dictionary<string, Node>()
    let rightTree = new Dictionary<string, Node>()

    let findNode(rank: int) (searchTree: Dictionary<string, Node>) =
        let toRun = searchTree.Values |> Seq.sortBy _.Id |> Seq.toArray
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
        
    let swapNodes(leftNode: Node, rightNode: Node) (twisted: bool) =
        let rec buildSubtree(node: Node) (side: TreeType) =
            match node.Left with
            | Some leftChild ->
                let leftChildNode = 
                    if side.IsLEFT then 
                        let child = leftTree[leftChild.Id] 
                        leftTree.Remove(leftChild.Id) |> ignore
                        rightTree.Add(child.Id, child)
                        child
                    else 
                        let child = rightTree[leftChild.Id]
                        rightTree.Remove(leftChild.Id) |> ignore
                        leftTree.Add(child.Id, child)
                        child

                buildSubtree(leftChildNode) side
            | None -> ()
            match node.Right with
            | Some rightChild ->
                let rightChildNode = 
                    if side.IsLEFT then 
                        let child = leftTree[rightChild.Id] 
                        leftTree.Remove(rightChild.Id) |> ignore
                        rightTree.Add(child.Id, child)
                        child
                    else 
                        let child = rightTree[rightChild.Id]
                        rightTree.Remove(rightChild.Id) |> ignore
                        leftTree.Add(child.Id, child)
                        child
                buildSubtree(rightChildNode) side
            | None -> ()    
        let fromSide = if not twisted then LEFT else RIGHT
        if not twisted then
            leftTree.Remove(leftNode.Id) |> ignore
            rightTree.Add(leftNode.Id, leftNode)
        else
            rightTree.Remove(leftNode.Id) |> ignore
            leftTree.Add(leftNode.Id, leftNode)
        buildSubtree(leftNode) fromSide

        let fromSide = (if not twisted then RIGHT else LEFT)
        if not twisted then
            rightTree.Remove(rightNode.Id) |> ignore
            leftTree.Add(rightNode.Id, rightNode)
        else
            leftTree.Remove(rightNode.Id) |> ignore
            rightTree.Add(rightNode.Id, rightNode)
        buildSubtree(rightNode) fromSide

    operations
    |> List.iter(fun o -> 
        match o with
        | ADD op ->
            let leftNode, rightNode = op.Left, op.Right

            let parent = 
                if leftTree.Count > 0 then
                    let (side, node) = findNode op.Left.Rank leftTree
                    if side = 1 then // right
                        leftTree[node.Id] <- { leftTree[node.Id] with Right = Some leftNode }
                    else // left
                        leftTree[node.Id] <- { leftTree[node.Id] with Left = Some leftNode }
                    Some node
                else
                    None

            leftTree.Add(op.Left.Id, { op.Left with Parent = parent })
        
            let parent =
                if rightTree.Count > 0 then
                    let (side, node) = findNode op.Right.Rank rightTree
                    if side = 1 then // right
                        rightTree[node.Id] <- { rightTree[node.Id] with Right = Some rightNode }
                    else // left
                        rightTree[node.Id] <- { rightTree[node.Id] with Left = Some rightNode }
                    Some node
                else
                    None

            rightTree.Add(op.Right.Id, { op.Right with Parent = parent })
        | SWAP op ->
            let tmpOp = operations |> List.find(fun x -> 
                match x with
                | ADD addOp when addOp.Id = op.Id -> true
                | _ -> false)
            match tmpOp with
            | ADD addOp ->
                if leftTree.ContainsKey(addOp.Left.Id) then
                    let leftNode = leftTree[addOp.Left.Id]
                    let rightNode = rightTree[addOp.Right.Id]
                    if leftNode.Parent.IsSome then
                        let parent = leftTree[leftNode.Parent.Value.Id]
                        match parent.Left, parent.Right with
                        | Some l, _ when l.Id = leftNode.Id ->
                            leftTree[parent.Id] <- { leftTree[parent.Id] with Left = Some rightNode }
                        | _, Some r when r.Id = leftNode.Id ->
                            leftTree[parent.Id] <- { leftTree[parent.Id] with Right = Some rightNode }
                        | _ -> ()
                    if rightNode.Parent.IsSome then
                        let parent = rightTree[rightNode.Parent.Value.Id]
                        match parent.Left, parent.Right with
                        | Some l, _ when l.Id = rightNode.Id ->
                            rightTree[parent.Id] <- { rightTree[parent.Id] with Left = Some leftNode }
                        | _, Some r when r.Id = rightNode.Id ->
                            rightTree[parent.Id] <- { rightTree[parent.Id] with Right = Some leftNode }
                        | _ -> ()
                    swapNodes(leftNode, rightNode) false
                else
                    let leftNode = rightTree[addOp.Left.Id]
                    let rightNode = leftTree[addOp.Right.Id]
                    if leftNode.Parent.IsSome then
                        let parent = rightTree[leftNode.Parent.Value.Id]
                        match parent.Left, parent.Right with
                        | Some l, _ when l.Id = leftNode.Id ->
                            rightTree[parent.Id] <- { rightTree[parent.Id] with Left = Some rightNode }
                        | _, Some r when r.Id = leftNode.Id ->
                            rightTree[parent.Id] <- { rightTree[parent.Id] with Right = Some rightNode }
                        | _ -> ()
                    if rightNode.Parent.IsSome then
                        let parent = leftTree[rightNode.Parent.Value.Id]
                        match parent.Left, parent.Right with
                        | Some l, _ when l.Id = rightNode.Id ->
                            leftTree[parent.Id] <- { leftTree[parent.Id] with Left = Some leftNode }
                        | _, Some r when r.Id = rightNode.Id ->
                            leftTree[parent.Id] <- { leftTree[parent.Id] with Right = Some leftNode }
                        | _ -> ()
                    swapNodes(leftNode, rightNode) true
            | _ -> ()
    )
    (leftTree, rightTree)
   

let buildWord((leftTree, rightTree): Dictionary<string, Node>*Dictionary<string, Node>) =
    let leftNodesByLevel = new Dictionary<int, Node list>()
    let rightNodesByLevel = new Dictionary<int, Node list>()

    let leftRoot = (leftTree.Values |> Seq.sortBy _.Id |> Array.ofSeq)[0]
    leftNodesByLevel.Add(1, [leftRoot])
    let rightRoot = (rightTree.Values |> Seq.sortBy _.Id |> Array.ofSeq)[0]
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
    let (leftTree, rightTree) = buildTree(nodes)
    buildWord (leftTree, rightTree)
