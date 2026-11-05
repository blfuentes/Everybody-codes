module quest08_3_visualization

open System
open System.Collections.Generic
open System.Drawing
open System.Drawing.Drawing2D
open System.Drawing.Imaging
open System.Drawing.Text
open System.IO
open EverybodyCodes_2026_FSharp.Modules
open quest08_utilities

//let path = "quest08/test_input_03.txt"
let path = "quest08/quest08_input_03.txt"

// reaching 100000 warriors takes ~6000 rounds, so only the first ones are drawn by default
let rounds = 120

let private arrows = [| "↘"; "↙"; "←"; "↖"; "↗"; "→" |]
let private sqrt3 = sqrt 3.0f

let private background = Color.FromArgb(15, 19, 21)
let private gold = Color.FromArgb(255, 204, 64)
let private sunlitYoung = Color.FromArgb(196, 232, 124)
let private sunlitOld = Color.FromArgb(98, 168, 68)
let private shadowYoung = Color.FromArgb(42, 66, 60)
let private shadowOld = Color.FromArgb(27, 47, 43)
let private soil = Color.FromArgb(56, 40, 29)
let private sunlitInk = Color.FromArgb(24, 60, 28)
let private shadowInk = Color.FromArgb(104, 150, 120)
let private warriorInk = Color.FromArgb(96, 46, 10)
let private seedInk = Color.FromArgb(220, 176, 124)

let private mix (a: Color) (b: Color) (t: float32) =
    let channel (x: byte) (y: byte) = int (float32 x + (float32 y - float32 x) * t)
    Color.FromArgb(channel a.R b.R, channel a.G b.G, channel a.B b.B)

let private plant height =
    if height <= 2 then "🌱" elif height <= 5 then "🌿" elif height <= 8 then "🌲" else "🌳"

let private hexagon (center: PointF) (radius: float32) =
    Array.init 6 (fun i ->
        let angle = Math.PI / 3.0 * float i + Math.PI / 6.0
        PointF(center.X + radius * float32 (cos angle), center.Y + radius * float32 (sin angle)))

let execute() =
    let field = LocalHelper.GetContent path 2026 8 3 |> parseField
    let raysByDirection = directions |> Array.map (rays field)
    let cells = Array.ofSeq field.Keys
    let maxRow = cells |> Array.map fst |> Array.max
    let maxCol = cells |> Array.map snd |> Array.max

    // pointy-top hexes: one column is half a hex wide, one row is 1.5 hex radii tall
    let fieldWidth, margin, header, footer = 760.0f, 84.0f, 72.0f, 56.0f
    let radius = fieldWidth / (sqrt3 * (float32 maxCol / 2.0f + 1.0f))
    let fieldHeight = radius * (1.5f * float32 maxRow + 2.0f)
    let width = int (fieldWidth + 2.0f * margin)
    let height = int (header + fieldHeight + 2.0f * margin + footer)
    let fieldCenter = PointF(margin + fieldWidth / 2.0f, header + margin + fieldHeight / 2.0f)
    let centerOf (r, c) =
        PointF(margin + sqrt3 * radius * float32 (c + 1) / 2.0f, header + margin + radius * (1.5f * float32 r + 1.0f))

    use titleFont = new Font("Segoe UI", 13.0f, GraphicsUnit.Pixel)
    use roundFont = new Font("Segoe UI Semibold", 30.0f, GraphicsUnit.Pixel)
    use arrowFont = new Font("Segoe UI Symbol", 30.0f, GraphicsUnit.Pixel)
    use statsFont = new Font("Segoe UI", 17.0f, GraphicsUnit.Pixel)
    use legendFont = new Font("Segoe UI", 14.0f, GraphicsUnit.Pixel)
    use legendSymbolFont = new Font("Segoe UI Symbol", 16.0f, GraphicsUnit.Pixel)
    use legendEmojiFont = new Font("Segoe UI Emoji", 13.0f, GraphicsUnit.Pixel)
    use sunFont = new Font("Segoe UI Symbol", 58.0f, GraphicsUnit.Pixel)
    use seedFont = new Font("Segoe UI Emoji", radius * 0.8f, GraphicsUnit.Pixel)
    use warriorFont = new Font("Segoe UI Emoji", radius * 0.95f, GraphicsUnit.Pixel)
    let plantFonts = Array.init 10 (fun h -> new Font("Segoe UI Symbol", radius * (0.62f + 0.06f * float32 h), GraphicsUnit.Pixel))
    use centered = new StringFormat(Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center)
    use alignRight = new StringFormat(Alignment = StringAlignment.Far)
    use textBrush = new SolidBrush(Color.FromArgb(236, 236, 228))
    use dimBrush = new SolidBrush(Color.FromArgb(130, 142, 140))
    use goldBrush = new SolidBrush(gold)
    use litPen = new Pen(gold, max 1.2f (radius * 0.09f))

    let folder = Path.Combine(VisualizationFolder, "quest08_3_visualization")
    Directory.CreateDirectory folder |> ignore
    for oldFrame in Directory.GetFiles(folder, "round_*.png") do
        File.Delete oldFrame

    let drawHex (g: Graphics) (center: PointF) hexRadius (fill: Color) (ink: Color) (glyph: string) (font: Font) (outline: Pen option) =
        let corners = hexagon center hexRadius
        use fillBrush = new SolidBrush(fill)
        use inkBrush = new SolidBrush(ink)
        g.FillPolygon(fillBrush, corners)
        outline |> Option.iter (fun pen -> g.DrawPolygon(pen, corners))
        g.DrawString(glyph, font, inkBrush, center, centered)

    let drawFrame round (lit: HashSet<int*int>) totalEnergy warriors newWarriors =
        use bitmap = new Bitmap(width, height)
        use g = Graphics.FromImage bitmap
        g.SmoothingMode <- SmoothingMode.AntiAlias
        g.TextRenderingHint <- TextRenderingHint.AntiAliasGridFit
        g.Clear background

        // the sun sits just beyond the field corner the light comes from
        let dr, dc = directions[round % 6]
        let dx, dy = sqrt3 * radius * float32 dc / 2.0f, 1.5f * radius * float32 dr
        let scale = (fieldWidth / 2.0f + margin * 0.55f) / sqrt (dx * dx + dy * dy)
        let sun = PointF(fieldCenter.X - dx * scale, fieldCenter.Y - dy * scale)
        use glowPath = new GraphicsPath()
        glowPath.AddEllipse(sun.X - 70.0f, sun.Y - 70.0f, 140.0f, 140.0f)
        use glow = new PathGradientBrush(glowPath, CenterColor = Color.FromArgb(150, gold), SurroundColors = [| Color.FromArgb(0, gold) |])
        g.FillPath(glow, glowPath)
        g.DrawString("☀", sunFont, goldBrush, sun, centered)

        for KeyValue(cell, value) in field do
            let isLit = lit.Contains cell
            let age = float32 (max 1 value - 1) / 8.0f
            let hex = drawHex g (centerOf cell) (radius * 0.93f)
            // seeds are -2..0 here and brighten as they get closer to sprouting
            if value <= 0 then hex soil (mix soil seedInk (0.45f + 0.275f * float32 (value + 2))) "🌰" seedFont None
            elif isLit && value = 9 then hex gold warriorInk "⚔" warriorFont None
            elif isLit then hex (mix sunlitYoung sunlitOld age) sunlitInk (plant value) (plantFonts[value]) (Some litPen)
            else hex (mix shadowYoung shadowOld age) shadowInk (plant value) (plantFonts[value]) None

        g.DrawString("EVERYBODY CODES 2026  ·  QUEST 8  ·  PART 3", titleFont, dimBrush, margin, 14.0f)
        let roundText = $"Round {round + 1}"
        g.DrawString(roundText, roundFont, textBrush, margin - 3.0f, 30.0f)
        g.DrawString(arrows[round % 6], arrowFont, goldBrush, margin + g.MeasureString(roundText, roundFont).Width, 30.0f)
        let statsArea top = RectangleF(0.0f, top, float32 width - margin, 24.0f)
        g.DrawString($"energy {lit.Count}  ·  total {totalEnergy}", statsFont, textBrush, statsArea 18.0f, alignRight)
        g.DrawString($"warriors {warriors} (+{newWarriors})", statsFont, goldBrush, statsArea 44.0f, alignRight)

        let legendY = float32 height - footer / 2.0f - 6.0f
        let legend =
            [ Some (mix sunlitYoung sunlitOld 0.4f), sunlitInk, "🌿", legendSymbolFont, "sunlit"
              Some shadowYoung, shadowInk, "🌿", legendSymbolFont, "in shadow"
              Some gold, warriorInk, "⚔", legendEmojiFont, "becomes a warrior"
              Some soil, seedInk, "🌰", legendEmojiFont, "seed"
              None, sunlitYoung, "🌱", legendSymbolFont, "1–2"
              None, sunlitYoung, "🌿", legendSymbolFont, "3–5"
              None, sunlitYoung, "🌲", legendSymbolFont, "6–8"
              None, sunlitYoung, "🌳", legendSymbolFont, "9" ]
        (margin, legend)
        ||> List.fold (fun x (fill, ink, glyph, font, label) ->
            let center = PointF(x + 12.0f, legendY)
            match fill with
            | Some fill -> drawHex g center 13.0f fill ink glyph font None
            | None ->
                use inkBrush = new SolidBrush(ink)
                g.DrawString(glyph, font, inkBrush, center, centered)
            g.DrawString(label, legendFont, dimBrush, x + 27.0f, legendY - 10.0f)
            x + 42.0f + g.MeasureString(label, legendFont).Width)
        |> ignore

        bitmap.Save(Path.Combine(folder, $"round_{round + 1:D5}.png"), ImageFormat.Png)

    let mutable warriors = 0
    let mutable totalEnergy = 0
    let mutable round = 0
    while warriors < 100000 do
        for cell in cells do
            if field[cell] <= 0 then field[cell] <- field[cell] + 1

        let lit = HashSet<int*int>(litCells field (raysByDirection[round % 6]))
        let newWarriors = lit |> Seq.filter (fun cell -> field[cell] = 9) |> Seq.length
        warriors <- warriors + newWarriors
        totalEnergy <- totalEnergy + lit.Count
        drawFrame round lit totalEnergy warriors newWarriors

        for cell in lit do
            field[cell] <- if field[cell] = 9 then -3 else field[cell] + 1
        round <- round + 1

    for font in plantFonts do
        font.Dispose()
    folder
