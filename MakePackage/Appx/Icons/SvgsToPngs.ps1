
Param(
    [parameter(mandatory)][string]$inputDir,
    [parameter(mandatory)][string]$outputDir
)

$inkscape = 'E:\Bin\inkscape-1.2.2\bin\inkscape.exe'
#$inputDir = "Svgs"
#$outputDir = "Pngs"

# error to break
trap { break }

$ErrorActionPreference = "stop"


function Export-Png($svg, $png)
{
    & $inkscape --export-filename=$png --export-overwrite $svg
}


if (!(Test-Path $outputDir))
{
    New-Item -Path . -Name $outputDir -ItemType Directory
}

$files = (Get-ChildItem $inputDir\*.svg).Name

foreach($file in $files)
{
    $png = [System.IO.Path]::ChangeExtension($file,".png")
    Export-Png $inputDir\$file $outputDir\$png
}




