module quest19_game

open EverybodyCodes_2025_FSharp.Modules
open System.Collections.Generic
open System.IO
open System.Text
open System
open System.Threading

//let path = "quest19/test_input_01.txt"
let path = "quest19/quest19_input_01.txt"

let empty = new HashSet<int*int>()
let walls = new HashSet<int>()

let mutable maxX = 0
let mutable maxY = 0

let reverseX (x: int) =
    maxX - x
let reverseY (y: int) =
    maxY - y

let mutable BirdPosition = (0, 0)
let mutable gameRunning = true
let mutable gameStarted = false
let mutable gameOverEmoji = ""
let mutable gameOverPosition = (0, 0)
let mutable shouldRestart = false

let parseContent (lines: string array) =
    lines
    |> Array.iter(fun line ->
        let parts = line.Split(",")
        let x = parts[0] |> int
        let endingY = (parts[1] |> int) + (parts[2] |> int)
        walls.Add(x) |> ignore
        if endingY > maxY then
            maxY <- endingY + (x - maxX)
        maxX <- x
    )
    lines
    |> Array.iter(fun line ->
        let parts = line.Split(",")
        let x = parts[0] |> int
        let startingY = parts[1] |> int 
        let endingY = (parts[1] |> int) + (parts[2] |> int)
        for y in startingY .. (endingY-1) do
            let (eY, eX) = (reverseY y, x)
            if not (empty.Contains((eY, eX))) then
                empty.Add(eY, eX) |> ignore
    )

let visualizeToConsole() =
    // Set console encoding to UTF8 for proper emoji rendering
    Console.OutputEncoding <- System.Text.Encoding.UTF8
    
    // Use StringBuilder for buffered output
    let buffer = StringBuilder((maxY + 1) * (maxX + 5))
    
    // Build the entire grid in buffer
    for y in 0 .. maxY do
        for x in 0 .. maxX do
            let isWall = walls.Contains(x) && not (empty.Contains((y, x)))
            let isEmpty = empty.Contains((y, x)) || not (walls.Contains(x))
            let isBird = (y, x) = BirdPosition
            let isGameOverPos = not gameRunning && (y, x) = gameOverPosition
            
            let symbol = 
                if isGameOverPos then gameOverEmoji
                elif isBird then "🐦"
                elif isWall then "🧱"
                elif isEmpty then "  "  // Two spaces to account for emoji width
                else "  "
            
            buffer.Append(sprintf "%s" symbol) |> ignore
        
        buffer.AppendLine() |> ignore
    
    // Add legend and controls
    buffer.AppendLine() |> ignore
    buffer.AppendLine("Legend:") |> ignore
    buffer.AppendLine("🧱 = Wall (Brick)") |> ignore
    buffer.AppendLine("🐦 = Bird") |> ignore
    buffer.AppendLine("   = Empty/Path (Air)") |> ignore
    buffer.AppendLine(sprintf "Grid size: %d x %d" (maxX + 1) (maxY + 1)) |> ignore
    buffer.AppendLine(sprintf "Bird Position: (%d, %d)" (fst BirdPosition) (snd BirdPosition)) |> ignore
    buffer.AppendLine() |> ignore
    
    if not gameStarted then
        buffer.AppendLine("Press UP arrow to start the game | Press ESC to quit") |> ignore
    elif not gameRunning then
        if gameOverEmoji = "💥" then
            buffer.AppendLine("💥 GAME OVER! You hit a wall!") |> ignore
        elif gameOverEmoji = "💀" then
            buffer.AppendLine("💀 GAME OVER! Bird fell off the bottom!") |> ignore
        elif gameOverEmoji = "🎉" then
            buffer.AppendLine("🎉 YOU WIN! You made it through!") |> ignore
        buffer.AppendLine() |> ignore
        buffer.AppendLine("Press 'R' to restart | Press ESC to quit") |> ignore
    else
        buffer.AppendLine("Controls: Press UP arrow to fly up | Press ESC to quit") |> ignore
    
    // Use SetCursorPosition instead of Clear to reduce flickering
    Console.SetCursorPosition(0, 0)
    Console.Write(buffer.ToString())

let resetGame() =
    BirdPosition <- (reverseY 0, 0)
    gameRunning <- true
    gameStarted <- false
    gameOverEmoji <- ""
    gameOverPosition <- (0, 0)
    shouldRestart <- false

let moveBirdUp() =
    let (currentY, currentX) = BirdPosition
    let newY = max 0 (currentY - 1)  // Go up, but don't go below 0
    BirdPosition <- (newY, currentX)
    visualizeToConsole()

let moveBirdHorizontal() =
    let (currentY, currentX) = BirdPosition
    
    // Move forward (increase X)
    let newX = currentX + 1
    
    // Bird falls down by default (gravity)
    let newY = currentY + 1
    
    // Check if bird went below maxY (fell off the bottom)
    if newY > maxY then
        // Fell off the bottom - game over
        gameRunning <- false
        gameOverEmoji <- "💀"
        gameOverPosition <- (maxY, currentX)  // Show at bottom edge
        BirdPosition <- (maxY, currentX)
    else
        // Check if new position is valid (not hitting a wall)
        let isWall = walls.Contains(newX) && not (empty.Contains((newY, newX)))
        
        if isWall then
            // Hit a wall - game over
            gameRunning <- false
            gameOverEmoji <- "💥"
            gameOverPosition <- (newY, newX)
            BirdPosition <- (newY, newX)
        elif newX > maxX then
            // Reached the end - game won
            gameRunning <- false
            gameOverEmoji <- "🎉"
            gameOverPosition <- (newY, currentX)  // Show where bird was, not beyond
            BirdPosition <- (newY, currentX)
        else
            // Valid move
            BirdPosition <- (newY, newX)

let keyboardListener() =
    async {
        while not shouldRestart do
            if Console.KeyAvailable then
                let key = Console.ReadKey(true)
                match key.Key with
                | ConsoleKey.UpArrow -> 
                    if not gameStarted && gameRunning then
                        gameStarted <- true
                    if gameStarted && gameRunning then
                        moveBirdUp()  // Immediate vertical movement
                | ConsoleKey.R when not gameRunning -> 
                    shouldRestart <- true
                | ConsoleKey.Escape -> 
                    gameRunning <- false
                    shouldRestart <- false
                | _ -> ()
            do! Async.Sleep(50)  // Check for input every 50ms
    }

let gameLoop() =
    async {
        while gameRunning && not shouldRestart do
            // Only update horizontal position if game has started
            if gameStarted then
                moveBirdHorizontal()
                visualizeToConsole()
            
            do! Async.Sleep(500)  // Update every 0.5 seconds
    }

let runGame() =
    Console.CursorVisible <- false
    Console.Clear()  // Clear once at the start
    
    // Show initial game state immediately
    visualizeToConsole()
    
    // Start both async tasks
    let keyboard = keyboardListener()
    let game = gameLoop()
    
    // Run both tasks concurrently
    Async.Parallel [keyboard; game]
    |> Async.RunSynchronously
    |> ignore
    
    // Keep showing the final state
    visualizeToConsole()
    
    Console.CursorVisible <- true

let rec playGame() =
    resetGame()
    runGame()
    
    // Check if player wants to restart
    if shouldRestart then
        playGame()  // Recursive call to restart the game

let execute() =
    let lines = LocalHelper.GetLinesFromFile(path)
    parseContent lines
    BirdPosition <- (reverseY 0, 0)
    
    // Start the game loop
    playGame()
    
    (maxX, maxY, empty, walls)