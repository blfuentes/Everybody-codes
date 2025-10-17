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

    Write-Output "Created $dstName from quest00"
    exit 0
}
catch {
    Write-Error $_.Exception.Message
    exit 1
}