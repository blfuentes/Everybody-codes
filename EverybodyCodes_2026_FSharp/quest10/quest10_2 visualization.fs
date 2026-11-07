module quest10_2_visualization

open EverybodyCodes_2026_FSharp.Modules
open System
open System.Collections.Generic
open System.Drawing
open System.Drawing.Imaging
open System.Drawing.Text
open System.Globalization
open System.IO
open quest10_utilities

let path = "quest10/test_input_02.txt"
//let path = "quest10/quest10_input_02.txt"

let nullghosts = Dictionary<int, Nullghost>()
let walls = HashSet<int * int>()
let rip = HashSet<int * int>()
let collisions = HashSet<int * int>()

let mutable (maxRow, maxCol) = (0, 0)

let parseContent (lines: string array) defaultLife =
    nullghosts.Clear()
    walls.Clear()
    rip.Clear()
    collisions.Clear()
    maxRow <- lines.Length - 1
    maxCol <- lines[0].Length - 1
    let mutable id = 0
    for r in 0..maxRow do
        for c in 0..maxCol do
            match lines[r][c] with
            | '.' -> ignore()
            | '#' -> walls.Add((r,c)) |> ignore
            | _ ->
                nullghosts.Add(id, { Id = id; Pos = (r, c); Dir = lines[r][c]; Life = defaultLife })
                id <- id + 1

let round() : int =
    rip.Clear()
    collisions.Clear()
    let moveGhosts() : int =
        let mutable count = 0
        for kvp in nullghosts do
            let (key, ghost) = (kvp.Key, kvp.Value)
            let newPos = move (movOfDir ghost.Dir, ghost.Pos)
            if not (walls.Contains newPos) then
                nullghosts[key] <- { ghost with Pos = newPos }
            else
                let newLife = ghost.Life - 1I
                if newLife = 0I then
                    rip.Add(ghost.Pos) |> ignore
                    nullghosts.Remove(key) |> ignore
                else
                    nullghosts[key] <- { ghost with Life = newLife; Dir = rotate90 ghost.Dir }
                count <- count + 1
        count

    let countCollisions() =
        let groups =
            nullghosts
            |> Seq.groupBy _.Value.Pos
            |> Seq.map (fun (pos, ghosts) -> pos, Seq.toArray ghosts)
            |> Seq.filter (fun (_, ghosts) -> ghosts.Length > 1)
            |> Seq.toArray
        let mutable count = 0
        for pos, group in groups do
            collisions.Add(pos) |> ignore
            for kvp in group do
                let ghost = kvp.Value
                let newLife = ghost.Life - 1I
                if newLife = 0I then
                    rip.Add(pos) |> ignore
                    nullghosts.Remove(kvp.Key) |> ignore
                else
                    nullghosts[kvp.Key] <- { ghost with Life = newLife; Dir = rotate180 ghost.Dir }
                count <- count + 1
        count

    moveGhosts() + countCollisions()

let start () : int =
    let runId = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fffffff", CultureInfo.InvariantCulture)
    let folder = Path.Combine(VisualizationFolder, "quest10_2_visualization", runId)
    let assets = Path.Combine(__SOURCE_DIRECTORY__, "emoji")
    Directory.CreateDirectory(folder) |> ignore
    printfn "Saving frames to %s" folder

    use wall = Image.FromFile(Path.Combine(assets, "wall.png"))
    use ghost = Image.FromFile(Path.Combine(assets, "ghost.png"))
    use collision = Image.FromFile(Path.Combine(assets, "collision.png"))
    use grave = Image.FromFile(Path.Combine(assets, "grave.png"))
    use font = new Font("Segoe UI", 20.0f, GraphicsUnit.Pixel)
    use textBrush = new SolidBrush(Color.White)
    let width = max 320 ((maxCol + 1) * 32)
    let height = 40 + (maxRow + 1) * 32

    let export rounds =
        let occupied = nullghosts.Values |> Seq.map _.Pos |> HashSet
        use bitmap = new Bitmap(width, height)
        use graphics = Graphics.FromImage(bitmap)
        graphics.Clear(Color.Black)
        graphics.TextRenderingHint <- TextRenderingHint.AntiAliasGridFit
        graphics.DrawString(sprintf "Round %d - Remaining: %d" rounds nullghosts.Count, font, textBrush, 6.0f, 6.0f)

        for r in 0..maxRow do
            for c in 0..maxCol do
                let pos = (r, c)
                let image =
                    if walls.Contains pos then Some wall
                    elif collisions.Contains pos then Some collision
                    elif occupied.Contains pos then Some ghost
                    elif rip.Contains pos then Some grave
                    else None
                image |> Option.iter (fun sprite -> graphics.DrawImageUnscaled(sprite, c * 32, 40 + r * 32))

        bitmap.Save(Path.Combine(folder, sprintf "frame_%08d.png" rounds), ImageFormat.Png)

    let mutable rounds = 0
    export rounds
    while nullghosts.Count > 0 do
        round() |> ignore
        rounds <- rounds + 1
        export rounds
    rounds

let execute() =
    let lines = LocalHelper.GetContent path 2026 10 2
    let defaultLife = 10I
    parseContent lines defaultLife
    start()
