module quest18_1_visualization

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.Drawing
open System.Drawing.Imaging
open System.IO

//let path = "quest18/test_input_01.txt"
let path = "quest18/quest18_input_01.txt"

type BranchDefinition = {
    Id: int Option;
    Thickness: int;
}

type PlantDefinition = {
    Id: int
    Thickness: int
    Branches: BranchDefinition[]
}

let plantDefinitions = new Dictionary<int, PlantDefinition>()

let parseContent (content: string) =
    let parts = content.Split($"{System.Environment.NewLine}{System.Environment.NewLine}")
    // regular expression to match all numbers in a string
    let numberRegex = System.Text.RegularExpressions.Regex(@"\d+")
    parts
    |> Seq.iter(fun p ->

        let (header, body) = (
            p.Split($"{System.Environment.NewLine}")[0],
            (p.Split($"{System.Environment.NewLine}")[1..])
        )
        let id = numberRegex.Match(header).Value |> int
        let thickness = numberRegex.Matches(header).[1].Value |> int
        let branches =
            body
            |> Seq.map(fun b ->
                let matches = numberRegex.Matches(b)
                let branchId =
                    if matches.Count > 1 then
                        Some (matches[0].Value |> int)
                    else
                        None
                let branchThickness =
                    if matches.Count > 1 then
                        matches[1].Value |> int
                    else
                        matches[0].Value |> int
                {
                    Id = branchId
                    Thickness = branchThickness
                }
            )
            |> Seq.toArray
        plantDefinitions.Add(id, {
            Id = id
            Thickness = thickness
            Branches = branches
        })
    )

let calculateBrightness () =
    let plantEnergies = Array.zeroCreate<int> (plantDefinitions.Count)
    for pIdx in 1..plantDefinitions.Count do
        let plant = plantDefinitions[pIdx]
        let mutable energy = 0
        for branch in plant.Branches do
            match branch.Id with
            | Some bId ->
                energy <- energy + (plantEnergies[bId-1] * branch.Thickness)
            | None ->
                energy <- energy + 1
        plantEnergies[pIdx-1] <- if plant.Thickness > energy then 0 else energy
    plantEnergies |> Array.last

let calculateAllBrightness () =
    let plantEnergies = Array.zeroCreate<int> (plantDefinitions.Count)
    for pIdx in 1..plantDefinitions.Count do
        let plant = plantDefinitions[pIdx]
        let mutable energy = 0
        for branch in plant.Branches do
            match branch.Id with
            | Some bId ->
                energy <- energy + (plantEnergies[bId-1] * branch.Thickness)
            | None ->
                energy <- energy + 1
        plantEnergies[pIdx-1] <- if plant.Thickness > energy then 0 else energy
    plantEnergies

let assignLevels () =
    let levels = Dictionary<int, int>()
    
    // Build parent-child relationships
    let children = Dictionary<int, HashSet<int>>()
    for plant in plantDefinitions.Values do
        if not (children.ContainsKey(plant.Id)) then
            children[plant.Id] <- HashSet<int>()
        for branch in plant.Branches do
            match branch.Id with
            | Some childId ->
                children[plant.Id].Add(childId) |> ignore
            | None -> ()
    
    // Identify leaf plants (plants with only free branches - no branch IDs)
    let leafPlants = 
        plantDefinitions.Values
        |> Seq.filter (fun p -> p.Branches |> Array.forall (fun b -> b.Id.IsNone))
        |> Seq.map (fun p -> p.Id)
        |> Seq.sort
        |> Seq.toList
    
    // Get all plant IDs sorted
    let allPlantIds = 
        plantDefinitions.Keys 
        |> Seq.sort 
        |> Seq.toList
    
    let maxPlantId = allPlantIds |> List.max
    
    // Root is the highest ID plant
    levels[maxPlantId] <- 0
    
    // Get middle plants (excluding root and leaves), sorted DESCENDING
    let middlePlants = 
        allPlantIds 
        |> List.filter (fun id -> id <> maxPlantId && not (List.contains id leafPlants))
        |> List.sortDescending
    
    // Assign middle plants ensuring connected nodes aren't on same level
    let mutable currentLevel = 1
    let mutable plantsToAssign = middlePlants
    
    while not plantsToAssign.IsEmpty do
        let mutable currentLevelPlants = []
        let mutable remainingPlants = []
        
        for plantId in plantsToAssign do
            // Check if this plant is connected to any plant already in current level
            let hasConnectionInLevel = 
                currentLevelPlants 
                |> List.exists (fun levelPlantId ->
                    // Check if plantId connects to levelPlantId
                    (children.ContainsKey(plantId) && children[plantId].Contains(levelPlantId)) ||
                    // Check if levelPlantId connects to plantId
                    (children.ContainsKey(levelPlantId) && children[levelPlantId].Contains(plantId)))
            
            if hasConnectionInLevel then
                // Skip this plant for this level
                remainingPlants <- plantId :: remainingPlants
            else
                // Add to current level
                currentLevelPlants <- plantId :: currentLevelPlants
                levels[plantId] <- currentLevel
        
        // Move to next level
        plantsToAssign <- List.rev remainingPlants
        if not currentLevelPlants.IsEmpty then
            currentLevel <- currentLevel + 1
    
    // All leaf plants at the bottom level
    for plantId in leafPlants do
        levels[plantId] <- currentLevel
    
    //printfn "Leaf plants (bottom level %d): %A" currentLevel leafPlants
    //printfn "Root plant (level 0): %d" maxPlantId
    //printfn "Middle plants: %A" middlePlants
    //printfn ""
    //printfn "Level assignment:"
    //for level in 0 .. (levels.Values |> Seq.max) do
    //    let plantsAtLevel = 
    //        levels 
    //        |> Seq.filter (fun kvp -> kvp.Value = level) 
    //        |> Seq.map (fun kvp -> kvp.Key)
    //        |> Seq.sort
    //        |> Seq.toList
    //    printfn "  Level %d: %A" level plantsAtLevel
    
    levels

let getRandomColor (seed: int) =
    let random = System.Random(seed)
    let hue = random.NextDouble() * 360.0
    let saturation = 0.6 + random.NextDouble() * 0.4
    let lightness = 0.4 + random.NextDouble() * 0.3
    
    // Convert HSL to RGB
    let c = (1.0 - abs(2.0 * lightness - 1.0)) * saturation
    let x = c * (1.0 - abs((hue / 60.0) % 2.0 - 1.0))
    let m = lightness - c / 2.0
    
    let (r, g, b) =
        if hue < 60.0 then (c, x, 0.0)
        elif hue < 120.0 then (x, c, 0.0)
        elif hue < 180.0 then (0.0, c, x)
        elif hue < 240.0 then (0.0, x, c)
        elif hue < 300.0 then (x, 0.0, c)
        else (c, 0.0, x)
    
    Color.FromArgb(int((r + m) * 255.0), int((g + m) * 255.0), int((b + m) * 255.0))

let visualizeGraphToPNG (filename: string) =
    let brightnesses = calculateAllBrightness()
    let maxBrightness = if brightnesses.Length > 0 then Array.max brightnesses else 1
    let minBrightness = if brightnesses.Length > 0 then Array.min brightnesses else 0
    let maxThickness = 
        plantDefinitions.Values 
        |> Seq.collect (fun p -> seq { yield p.Thickness; yield! p.Branches |> Seq.map (fun b -> b.Thickness) })
        |> Seq.max
    
    // Find min and max brightness for mid-level nodes (excluding 0 and 1)
    let midBrightnesses = brightnesses |> Array.filter (fun b -> b > 1 && b < maxBrightness)
    let minMidBrightness = if midBrightnesses.Length > 0 then Array.min midBrightnesses else 2
    let maxMidBrightness = if midBrightnesses.Length > 0 then Array.max midBrightnesses else maxBrightness - 1
    
    let levels = assignLevels()
    let maxLevel = levels.Values |> Seq.max
    
    // Group plants by level and sort by ID (descending at each level)
    let plantsByLevel = 
        levels 
        |> Seq.groupBy (fun kvp -> kvp.Value)
        |> Seq.map (fun (level, plants) -> 
            (level, plants |> Seq.map (fun kvp -> kvp.Key) |> Seq.sortDescending |> Seq.toList))
        |> dict
    
    // Calculate positions with random horizontal spacing
    let positions = Dictionary<int, float * float>()
    let levelHeight = 450.0  // Increased from 350.0 for even more vertical distance
    let random = System.Random(42) // Fixed seed for reproducibility
    
    for level in 0 .. maxLevel do
        let plantsInLevel = plantsByLevel[level]
        
        // Adjust spacing based on level - middle levels get wider spacing
        let (minSpacing, maxSpacing) = 
            if level = 0 || level = maxLevel then
                // Root and leaf levels: moderate spacing
                (180.0, 280.0)
            else
                // Middle levels: wider spacing to avoid overlap
                (100.0, 2000.0)
        
        // Calculate total width needed for this level with random spacing
        let spacings = 
            List.init plantsInLevel.Length (fun _ -> 
                minSpacing + random.NextDouble() * (maxSpacing - minSpacing))
        
        let totalWidth = spacings |> List.sum
        
        // Start from the left and distribute nodes with random spacing
        let mutable x = -totalWidth / 2.0
        
        for i in 0 .. plantsInLevel.Length - 1 do
            let plantId = plantsInLevel[i]
            x <- x + spacings[i] / 2.0 // Half spacing before node
            positions[plantId] <- (x, float level * levelHeight)
            x <- x + spacings[i] / 2.0 // Half spacing after node
    
    // Calculate canvas bounds with more margin
    let allX = positions.Values |> Seq.map fst
    let allY = positions.Values |> Seq.map snd
    let minX = Seq.min allX
    let maxX = Seq.max allX
    let maxY = Seq.max allY
    
    let margin = 250.0
    let width = int(maxX - minX + 2.0 * margin)
    let height = int(maxY + 2.0 * margin)
    let offsetX = -minX + margin
    let offsetY = margin
    
    use bitmap = new Bitmap(width, height)
    use g = Graphics.FromImage(bitmap)
    g.Clear(Color.White)
    g.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.AntiAlias
    g.TextRenderingHint <- System.Drawing.Text.TextRenderingHint.AntiAlias
    
    // Create color map for each branch connection (using parent-child as seed)
    let branchColors = Dictionary<int*int, Color>()
    
    // Draw all connections first
    for plant in plantDefinitions.Values do
        let (parentX, parentY) = positions[plant.Id]
        
        let mutable branchIndex = 0
        for branch in plant.Branches do
            match branch.Id with
            | Some childId ->
                let (childX, childY) = positions[childId]
                
                let branchThickness = max 3.0f (float32 branch.Thickness / float32 maxThickness * 20.0f)
                
                // Get or create random color for this branch
                let branchKey = (plant.Id, childId)
                let branchColor = 
                    if branchColors.ContainsKey(branchKey) then
                        branchColors[branchKey]
                    else
                        let color = getRandomColor (plant.Id * 1000 + childId)
                        branchColors[branchKey] <- color
                        color
                
                use pen = new Pen(branchColor, branchThickness)
                pen.EndCap <- System.Drawing.Drawing2D.LineCap.ArrowAnchor
                g.DrawLine(pen, 
                    float32(parentX + offsetX), float32(parentY + offsetY + 40.0),
                    float32(childX + offsetX), float32(childY + offsetY - 40.0))
                
                branchIndex <- branchIndex + 1
            | None -> ()
    
    // Draw all nodes
    for plant in plantDefinitions.Values do
        let (nodeX, nodeY) = positions[plant.Id]
        let brightness = brightnesses[plant.Id - 1]
        
        // Color based on brightness with new scheme
        let plantColor = 
            if brightness = maxBrightness then
                // Highest brightness (root plant): orange
                Color.FromArgb(255, 140, 0)
            elif brightness = 0 then
                // Zero brightness: white
                Color.White
            elif brightness = 1 then
                // Leaf nodes (brightness 1): very light yellow, almost white
                Color.FromArgb(255, 255, 240)
            else
                // Mid-range brightness: gradient from light yellow to darker yellow
                // Scale based on actual min/max of mid-level nodes
                let brightnessRatio = 
                    if maxMidBrightness > minMidBrightness then
                        (float brightness - float minMidBrightness) / (float maxMidBrightness - float minMidBrightness)
                    else
                        0.5
                
                // Gradient from light yellow (255,255,200) to darker yellow (255,200,0)
                let redValue = 255
                let greenValue = int(255.0 - (brightnessRatio * 55.0))
                let blueValue = int(200.0 * (1.0 - brightnessRatio))
                Color.FromArgb(redValue, greenValue, blueValue)
        
        // Node size based on brightness - minimum 35px to fit text
        let nodeRadius = 
            if maxBrightness > 0 then
                // Range from 35px to 90px for dramatic size difference while fitting text
                max 35.0f (35.0f + (float32 brightness / float32 maxBrightness * 55.0f))
            else
                40.0f
        
        let x = float32(nodeX + offsetX)
        let y = float32(nodeY + offsetY)
        
        // Draw node circle
        use brush = new SolidBrush(plantColor)
        g.FillEllipse(brush, x - nodeRadius, y - nodeRadius, nodeRadius * 2.0f, nodeRadius * 2.0f)
        
        // Draw border
        use borderPen = new Pen(Color.Black, 3.0f)
        g.DrawEllipse(borderPen, x - nodeRadius, y - nodeRadius, nodeRadius * 2.0f, nodeRadius * 2.0f)
        
        // Draw plant ID
        use font = new Font("Arial", 14.0f, FontStyle.Bold)
        use textBrush = new SolidBrush(Color.Black)
        use sf = new StringFormat()
        sf.Alignment <- StringAlignment.Center
        sf.LineAlignment <- StringAlignment.Center
        g.DrawString($"P{plant.Id}", font, textBrush, x, y - 8.0f, sf)
        
        // Draw brightness below ID
        use smallFont = new Font("Arial", 11.0f, FontStyle.Regular)
        g.DrawString($"B:{brightness}", smallFont, textBrush, x, y + 10.0f, sf)
        
        // Draw leaf branches
        let leafBranches = plant.Branches |> Array.filter (fun b -> b.Id.IsNone)
        if leafBranches.Length > 0 then
            for i in 0 .. leafBranches.Length - 1 do
                let branch = leafBranches[i]
                let angle = System.Math.PI / 2.0 + (float i - float leafBranches.Length / 2.0) * 0.3
                let leafX = x + float32(System.Math.Cos(angle) * 60.0)
                let leafY = y + float32(System.Math.Sin(angle) * 60.0)
                
                let leafThickness = max 3.0f (float32 branch.Thickness / float32 maxThickness * 6.0f)
                use leafPen = new Pen(Color.FromArgb(100, 200, 50), leafThickness)
                g.DrawLine(leafPen, x, y, leafX, leafY)
                
                // Draw leaf
                use leafBrush = new SolidBrush(Color.FromArgb(100, 200, 50))
                g.FillEllipse(leafBrush, leafX - 6.0f, leafY - 6.0f, 12.0f, 12.0f)
    
    // Add legend
    use legendFont = new Font("Arial", 16.0f, FontStyle.Bold)
    use legendBrush = new SolidBrush(Color.Black)
    let legendText = sprintf "Plant Graph (Max Brightness: %d)" maxBrightness
    g.DrawString(legendText, legendFont, legendBrush, 15.0f, 15.0f)
    
    let fullPath = Path.Combine(VisualizationFolder, filename)
    bitmap.Save(fullPath, ImageFormat.Png)
    
    printfn "Visualization saved to: %s" fullPath
    fullPath

let execute() =
    let lines = LocalHelper.GetContentFromFile(path)
    parseContent lines
    
    // Create visualization
    visualizeGraphToPNG "quest18_1_visualization.png" |> ignore
    
    calculateBrightness()