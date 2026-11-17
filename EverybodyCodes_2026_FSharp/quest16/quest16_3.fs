module quest16_3

open EverybodyCodes_2026_FSharp.Modules

//let path = "quest16/test_input_03.txt"
let path = "quest16/quest16_input_03.txt"

type private RunDeque(colors: byte array, lengths: int array, initialCount: int, initialTotalLength: int64) =
    let mutable head = 0
    let mutable count = initialCount
    let mutable totalLength = initialTotalLength
    let mutable reversed = false
    let capacity = colors.Length

    let physicalIndex index =
        let orientedIndex = if reversed then count - index - 1 else index
        (head + orientedIndex) % capacity

    member _.Count = count
    member _.TotalLength = totalLength

    member _.ColorAt(index: int) =
        colors[physicalIndex index]

    member _.LengthAt(index: int) =
        lengths[physicalIndex index]

    member _.PopBack() =
        if count = 0 then
            invalidOp "queue is empty"

        let index = physicalIndex (count - 1)
        let color = colors[index]
        let length = lengths[index]

        if reversed then
            head <- (head + 1) % capacity

        count <- count - 1
        totalLength <- totalLength - int64 length
        color, length

    member _.PushFront(color: byte, length: int) =
        if reversed then
            invalidOp "reversed"

        if count > 0 && colors[physicalIndex 0] = color then
            let index = physicalIndex 0
            lengths[index] <- lengths[index] + length
        else
            if count = capacity then
                invalidOp "capacity overload"

            head <- (head - 1 + capacity) % capacity
            colors[head] <- color
            lengths[head] <- length
            count <- count + 1

        totalLength <- totalLength + int64 length

    member _.Reverse() =
        reversed <- not reversed

let private colorCode = function
    | 'R' -> 0uy
    | 'G' -> 1uy
    | 'B' -> 2uy
    | 'Y' -> 3uy
    | shade -> invalidArg "notes" (sprintf "invalid shade '%c'" shade)

let private copyRuns (source: RunDeque) start count capacity =
    let colors = Array.zeroCreate<byte> capacity
    let lengths = Array.zeroCreate<int> capacity
    let mutable totalLength = 0L

    for offset in 0 .. count - 1 do
        let color = source.ColorAt(start + offset)
        let length = source.LengthAt(start + offset)
        colors[offset] <- color
        lengths[offset] <- length
        totalLength <- totalLength + int64 length

    RunDeque(colors, lengths, count, totalLength)

let totalFlipLength (notes: char array) copies =
    let encodedNotes = notes |> Array.map colorCode
    let mutable patternRunCount = 1
    
    for index in 1 .. encodedNotes.Length - 1 do
        if encodedNotes[index] <> encodedNotes[index - 1] then
            patternRunCount <- patternRunCount + 1
    
    let seamMerges =
        if encodedNotes[0] = encodedNotes[encodedNotes.Length - 1] then
            copies - 1
        else
            0
    
    let runCapacity = copies * patternRunCount - seamMerges
    let runColors = Array.zeroCreate<byte> runCapacity
    let runLengths = Array.zeroCreate<int> runCapacity
    let mutable runCount = 0
    
    for _ in 1 .. copies do
        let mutable index = 0
    
        while index < encodedNotes.Length do
            let color = encodedNotes[index]
            let runStart = index
    
            while index < encodedNotes.Length && encodedNotes[index] = color do
                index <- index + 1
    
            let runLength = index - runStart
    
            if runCount > 0 && runColors[runCount - 1] = color then
                runLengths[runCount - 1] <- runLengths[runCount - 1] + runLength
            else
                runColors[runCount] <- color
                runLengths[runCount] <- runLength
                runCount <- runCount + 1
    
    let mutable remaining = RunDeque(runColors, runLengths, runCount, int64 notes.Length * int64 copies)
    let mutable total = 0L
    let mutable sorted = false
    
    while not sorted do
        let counts = Array.zeroCreate<int> 4
        let firstIndices = Array.create 4 -1
        let lastIndices = Array.create 4 -1
    
        for index in 0 .. remaining.Count - 1 do
            let color = int (remaining.ColorAt index)
            counts[color] <- counts[color] + 1
            if firstIndices[color] = -1 then
                firstIndices[color] <- index
            lastIndices[color] <- index
    
        let mutable targetColor = -1
        let mutable firstIndex = remaining.Count
    
        for color in 0 .. counts.Length - 1 do
            if counts[color] > 1 && firstIndices[color] < firstIndex then
                targetColor <- color
                firstIndex <- firstIndices[color]
    
        if targetColor = -1 then
            sorted <- true
        else
            let lastIndex = lastIndices[targetColor]
            let activeCount = lastIndex - firstIndex
            let tailCount = remaining.Count - lastIndex - 1
            let active = copyRuns remaining (firstIndex + 1) activeCount activeCount
            let tail = copyRuns remaining (lastIndex + 1) tailCount (activeCount + tailCount)
            let mutable phaseComplete = false
    
            while not phaseComplete do
                if active.Count = 0 then
                    phaseComplete <- true
                else
                    total <- total + active.TotalLength
                    let (flippedColor, _) = active.PopBack()
    
                    if int flippedColor <> targetColor then
                        failwith "invalid flip target?"
    
                    active.Reverse()
    
                    while active.Count > 0
                          && int (active.ColorAt(active.Count - 1)) <> targetColor do
                        let (color, length) = active.PopBack()
                        tail.PushFront(color, length)
    
                    if active.Count = 0 then
                        phaseComplete <- true
    
            remaining <- tail
    
    total

let execute () =
    let notes = ((LocalHelper.GetContent path 2026 16 3)[0]).ToCharArray()
    totalFlipLength notes 100
