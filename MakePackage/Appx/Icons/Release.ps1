$outputDir = "..\Resources\Assets"
#$outputDir = "Test"
$postfix = ""
#$postfix = "-Blue"

# error to break
trap { break }

$ErrorActionPreference = "stop"


Get-ChildItem Pngs$postfix\*.png | Copy-Item -Destination $outputDir
Get-ChildItem TilePngs$postfix\*.png | Copy-Item -Destination $outputDir


