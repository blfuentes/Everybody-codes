param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d{2}$')]
    [string]$QuestNumber
)

# Get the script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Define paths
$mainCFile = Join-Path $scriptDir "EverybodyCodes_2025_C.c"
$questsHeaderFile = Join-Path $scriptDir "headers\quests.h"
$mainTemplateFile = Join-Path $scriptDir "templates\main.txt"
$questsTemplateFile = Join-Path $scriptDir "templates\quests.txt"
$cmakeTemplateFile = Join-Path $scriptDir "templates\cmake.txt"
$templateQuestDir = Join-Path $scriptDir "templates\quest00"
$outputQuestDir = Join-Path $scriptDir "quest$QuestNumber"
$cmakeFile = Join-Path $scriptDir "CMakeLists.txt"

# Helper function to safely write content with retry logic
function Set-ContentSafe {
    param(
        [string]$Path,
        [string]$Value,
        [int]$MaxRetries = 5,
        [int]$RetryDelayMs = 500
    )
    
    $retryCount = 0
    while ($retryCount -lt $MaxRetries) {
        try {
            Set-Content -Path $Path -Value $Value -Encoding UTF8 -ErrorAction Stop
            return $true
        }
        catch {
            $retryCount++
            if ($retryCount -ge $MaxRetries) {
                Write-Error "Failed to write to $Path after $MaxRetries attempts. File may be locked by another process."
                throw $_
            }
            Write-Warning "File locked: $Path. Retrying in ${RetryDelayMs}ms... (Attempt $retryCount/$MaxRetries)"
            Start-Sleep -Milliseconds $RetryDelayMs
        }
    }
}

# Validate files exist
if (-not (Test-Path $mainCFile)) {
    Write-Error "Main file not found: $mainCFile"
    exit 1
}
if (-not (Test-Path $questsHeaderFile)) {
    Write-Error "Quests header file not found: $questsHeaderFile"
    exit 1
}
if (-not (Test-Path $mainTemplateFile)) {
    Write-Error "Main template file not found: $mainTemplateFile"
    exit 1
}
if (-not (Test-Path $questsTemplateFile)) {
    Write-Error "Quests template file not found: $questsTemplateFile"
    exit 1
}
if (-not (Test-Path $cmakeTemplateFile)) {
    Write-Error "CMake template file not found: $cmakeTemplateFile"
    exit 1
}
if (-not (Test-Path $templateQuestDir)) {
    Write-Error "Template quest directory not found: $templateQuestDir"
    exit 1
}
if (-not (Test-Path $cmakeFile)) {
    Write-Error "CMakeLists.txt not found: $cmakeFile"
    exit 1
}

Write-Host "Adding Quest $QuestNumber..." -ForegroundColor Green

# 1. Update EverybodyCodes_2025_C.c
Write-Host "Updating $mainCFile..." -ForegroundColor Cyan
$mainContent = Get-Content $mainCFile -Raw
$templateMainContent = Get-Content $mainTemplateFile -Raw

# Replace "00" with the quest number in the template
$updatedTemplate = $templateMainContent -replace '00', $QuestNumber

# Insert the template before "return 0;" - more precise matching
$mainContent = $mainContent -replace '(\s+free\(result\d+_1\);[\s\S]*?)(\s+return 0;)', "`$1`n`n$updatedTemplate`$2"

Set-ContentSafe -Path $mainCFile -Value $mainContent    
Write-Host "Updated $mainCFile" -ForegroundColor Green

# 2. Update headers/quests.h
Write-Host "Updating $questsHeaderFile..." -ForegroundColor Cyan
$questsContent = Get-Content $questsHeaderFile -Raw
$templateQuestsContent = Get-Content $questsTemplateFile -Raw

# Replace "00" with the quest number in the template
$updatedQuestsTemplate = $templateQuestsContent -replace '00', $QuestNumber

# Insert before #endif
$questsContent = $questsContent -replace '(#endif\s*//\s*QUESTS_H)', "`n$updatedQuestsTemplate`n`n`$1"

Set-ContentSafe -Path $questsHeaderFile -Value $questsContent
Write-Host "Updated $questsHeaderFile" -ForegroundColor Green

# 3. Copy and rename quest00 folder to questXX
Write-Host "Copying quest template folder to quest$QuestNumber..." -ForegroundColor Cyan

if (Test-Path $outputQuestDir) {
    Write-Warning "Quest directory already exists: $outputQuestDir"
    $proceed = Read-Host "Do you want to overwrite it? (y/n)"
    if ($proceed -ne 'y' -and $proceed -ne 'Y') {
        Write-Host "Operation cancelled." -ForegroundColor Yellow
        exit 0
    }
    Remove-Item $outputQuestDir -Recurse -Force
}

# Copy the entire directory
Copy-Item $templateQuestDir -Destination $outputQuestDir -Recurse

# Rename files and replace content
$files = Get-ChildItem $outputQuestDir -File -Recurse

foreach ($file in $files) {
    # Rename file if it contains "quest00"
    if ($file.Name -like "*quest00*") {
        $newName = $file.Name -replace 'quest00', "quest$QuestNumber"
        $newPath = Join-Path $file.Directory $newName
        Rename-Item $file.FullName -NewName $newName
        $file = Get-Item $newPath
    }
    
    # Replace content in files
    if ($file.Extension -eq ".c" -or $file.Extension -eq ".txt") {
        $content = Get-Content $file.FullName -Raw
        $newContent = $content -replace '00', $QuestNumber
        Set-ContentSafe -Path $file.FullName -Value $newContent
    }
}

# Create three empty input txt files
Write-Host "Creating empty input files..." -ForegroundColor Cyan
for ($i = 1; $i -le 3; $i++) {
    $inputFileName = "quest$($QuestNumber)_input_$i.txt"
    $inputFilePath = Join-Path $outputQuestDir $inputFileName
    New-Item -Path $inputFilePath -ItemType File -Force | Out-Null
    Write-Host "Created $inputFileName" -ForegroundColor Gray
}

Write-Host "Created quest$QuestNumber folder with all files updated" -ForegroundColor Green

# 4. Update CMakeLists.txt
Write-Host "Updating CMakeLists.txt..." -ForegroundColor Cyan
$cmakeContent = Get-Content $cmakeFile -Raw
$cmakeTemplateContent = Get-Content $cmakeTemplateFile -Raw

# Replace "00" with the quest number in the cmake template
$updatedCmakeTemplate = $cmakeTemplateContent -replace '00', $QuestNumber

# Find and update add_executable section
# Match the closing parenthesis of add_executable and add new quest files before it
$addExecutablePattern = '(add_executable\(EverybodyCodes_2025_C[\s\S]*?quest\d+/quest\d+_3\.c)(\s*\))'
$newQuestFiles = "`$1`n  quest$QuestNumber/quest$QuestNumber`_1.c`n  quest$QuestNumber/quest$QuestNumber`_2.c`n  quest$QuestNumber/quest$QuestNumber`_3.c`$2"

$cmakeContent = $cmakeContent -replace $addExecutablePattern, $newQuestFiles

# Add directory creation and file copy commands
# Find where to insert the new quest cmake configuration - before the existing quest copy comments
$questCommentPattern = '(# Copy only txt files from quest\d+ to build directory at build time)'
$directoryCreationBlock = "# Create quest$QuestNumber directory in build folder if it doesn't exist`nadd_custom_command(TARGET EverybodyCodes_2025_C PRE_BUILD`n    COMMAND `${CMAKE_COMMAND} -E make_directory`n    `${CMAKE_BINARY_DIR}/quest$QuestNumber`n)`n`n$updatedCmakeTemplate`n`n"

if ($cmakeContent -match $questCommentPattern) {
    $cmakeContent = $cmakeContent -replace $questCommentPattern, "$directoryCreationBlock`$1"
} else {
    # If pattern not found, append at the end
    $cmakeContent = $cmakeContent + "`n`n$directoryCreationBlock"
}

Set-ContentSafe -Path $cmakeFile -Value $cmakeContent
Write-Host "Updated CMakeLists.txt" -ForegroundColor Green

Write-Host "`nQuest $QuestNumber has been successfully added!" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Review the generated files in quest$QuestNumber/" -ForegroundColor Yellow
Write-Host "  2. Update the implementation in quest$($QuestNumber)_*.c files" -ForegroundColor Yellow
Write-Host "  3. Add test data to quest$($QuestNumber)_input_*.txt files as needed" -ForegroundColor Yellow
Write-Host "  4. Rebuild your project" -ForegroundColor Yellow