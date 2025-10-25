param(
    [Parameter(Mandatory = $true)]
    [int]$Day
)

# Usage: .\add-new-entry.ps1 5   # creates quest05 from quest00

try {
    $xx = "{0:00}" -f $Day
    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
    Set-Location $scriptDir

    $src = Join-Path $scriptDir "quest00"
    if (-not (Test-Path $src)) { throw "Source folder 'quest00' not found in $scriptDir" }

    $dstName = "quest$xx"
    $dst = Join-Path $scriptDir $dstName
    if (Test-Path $dst) { throw "Destination '$dstName' already exists" }

    Copy-Item -Path $src -Destination $dst -Recurse -Force

    # Rename nested directories that contain "00" (deepest first)
    Get-ChildItem -Path $dst -Recurse -Directory |
        Sort-Object -Property FullName -Descending |
        ForEach-Object {
            if ($_.Name -like "*00*") {
                $newName = $_.Name -replace "00",$xx
                Rename-Item -Path $_.FullName -NewName $newName
            }
        }

    # Add quest_00_input_0X.txt being X 1..3 files
    for ($i = 1; $i -le 3; $i++) {
        $inputFile = Join-Path $dst "quest${xx}_input_0${i}.txt"
        New-Item -Path $inputFile -ItemType File -Force | Out-Null
    }

    # Replace contents and rename files that contain "00"
    Get-ChildItem -Path $dst -Recurse -File | ForEach-Object {
        $path = $_.FullName
        $content = Get-Content -Raw -LiteralPath $path
        $newContent = $content -replace "00", $xx
        if ($newContent -ne $content) {
            Set-Content -LiteralPath $path -Value $newContent -Encoding UTF8
        }

        if ($_.Name -like "*00*") {
            $newName = $_.Name -replace "00",$xx
            Rename-Item -LiteralPath $path -NewName $newName
        }
    }

    # Update the fsproj file to include the new quest files
    # Add new entries without removing existing ones for other quests
    # And they should be placed just before the Localhelper include
    # Files to be included, the questXX_1.fs, questXX_2.fs, questXX_3.fs and the test_input_0X.txt files
    $fsprojPath = Join-Path $scriptDir "EverybodyCodes_2024_S1_FSharp.fsproj"
    if (-not (Test-Path $fsprojPath)) { throw "Project file 'EverybodyCodes_2024_S1_FSharp.fsproj' not found in $scriptDir" }
    $fsprojContent = Get-Content -Raw -LiteralPath $fsprojPath
    $insertionPoint = $fsprojContent.IndexOf("<Compile Include=""Program.fs")
    if ($insertionPoint -eq -1) { throw "Could not find insertion point in project file" }
    $newEntries = @()
    $newEntries += "  <Compile Include=""$dstName\quest${xx}_1.fs"" />"
    $newEntries += "  <Compile Include=""$dstName\quest${xx}_2.fs"" />"
    $newEntries += "  <Compile Include=""$dstName\quest${xx}_3.fs"" />"
    $newEntries += "  <Compile Include=""$dstName\utils${xx}.fs"" />"
    $newEntries += "  <Content Include=""$dstName\test_input_01.txt"" />"
    $newEntries += "  <Content Include=""$dstName\test_input_02.txt"" />"
    $newEntries += "  <Content Include=""$dstName\test_input_03.txt"" />"
    $newEntries += "  <Content Include=""$dstName\quest${xx}_input_01.txt"" />"
    $newEntries += "  <Content Include=""$dstName\quest${xx}_input_02.txt"" />"
    $newEntries += "  <Content Include=""$dstName\quest${xx}_input_03.txt"" />"
    $newFsprojContent = $fsprojContent.Insert($insertionPoint, ($newEntries -join "`n") + "`n")
    Set-Content -LiteralPath $fsprojPath -Value $newFsprojContent -Encoding UTF8

    # Update the Program.fs to include a call to the new quest
    # Copy the last six lines after the latest // Day XX comment
    # And replace the quest00 with questXX adding them after the last day
    $programFsPath = Join-Path $scriptDir "Program.fs"
    if (-not (Test-Path $programFsPath)) { throw "Program file 'Program.fs' not found in $scriptDir" }
    # Read the file as an array of lines so we can work with indexes safely
    $programFsContent = Get-Content -LiteralPath $programFsPath
    $lastDayIndex = -1
    for ($i = 0; $i -lt $programFsContent.Count; $i++) {
        if ($programFsContent[$i] -match "// Day \d{2}") {
            $lastDayIndex = $i
        }
    }
    if ($lastDayIndex -eq -1) { throw "Could not find any Day comment in Program.fs" }

    $lastDayLine = $programFsContent[$lastDayIndex]
    $dayToReplace = ""
    if ($lastDayLine -match '(\d{2})') {
        $dayToReplace = $matches[1]
    }
    else {
        # Fallback to 00 if no number is found in the comment line, to maintain old behavior
        $dayToReplace = "00"
    }

    # Determine how many lines we can safely copy after the last day comment (up to 6)
    # We'll copy the Day comment line itself (to preserve indentation/formatting) plus up to $maxCopy lines after it.
    $startCopy = $lastDayIndex + 1
    $maxCopy = 6
    $available = $programFsContent.Count - $startCopy

    if ($available -le 0) {
        # Nothing to copy after the Day comment; we'll copy just the comment line
        $linesToCopy = @()
        $countToCopy = 0
        $endCopyIndex = $lastDayIndex
    }
    else {
        $countToCopy = [Math]::Min($maxCopy, $available)
        $endCopyIndex = $startCopy + $countToCopy - 1
        $linesToCopy = $programFsContent[$startCopy..$endCopyIndex]
    }

    # Prepare the block to insert: copy the original comment line (preserve indentation) and rename it to the new day,
    # then insert a blank line followed by the copied following lines with replacements.
    $originalCommentLine = $programFsContent[$lastDayIndex]
    # Replace the day number in the copied comment while preserving indentation/spacing
    # Use -match to capture the comment prefix in $matches and then append the $xx value explicitly.
    if ($originalCommentLine -match '^(
\s*//\s*Day\s*)\d{2}') {
        $prefix = $matches[1]
        $modifiedCommentLine = $prefix + $xx
    }
    else {
        # Fallback: replace any two-digit number with the new day
        $modifiedCommentLine = $originalCommentLine -replace '\d{2}', $xx
    }

    if ($linesToCopy.Count -gt 0) {
        $modifiedLines = $linesToCopy | ForEach-Object { $_ -replace $dayToReplace, $xx }
    }
    else {
        # Fallback stub if nothing to copy (no following lines present)
        $modifiedLines = @(
            "// auto-generated calls for quest$xx",
            "quest$xx.Quest1()",
            "quest$xx.Quest2()",
            "quest$xx.Quest3()",
            "()"
        )
    }

    # Build new file content inserting AFTER the copied block (or after lastDayIndex if nothing copied)
    $insertionIndex = $endCopyIndex

    $before = @()
    if ($insertionIndex -ge 0) { $before = $programFsContent[0..$insertionIndex] }

    $after = @()
    if ($insertionIndex -lt ($programFsContent.Count - 1)) { $after = $programFsContent[($insertionIndex + 1)..($programFsContent.Count - 1)] }

    $newProgramFsContent = @()
    $newProgramFsContent += $before
    # ensure a blank line before inserted comment
    $newProgramFsContent += ""
    # preserve indentation by using the modified copy of the original comment line
    $newProgramFsContent += $modifiedCommentLine
    $newProgramFsContent += $modifiedLines
    $newProgramFsContent += $after

    Set-Content -LiteralPath $programFsPath -Value ($newProgramFsContent -join "`n") -Encoding UTF8


    Write-Output "Created $dstName from quest00"
    exit 0
}
catch {
    Write-Error $_.Exception.Message
    exit 1
}