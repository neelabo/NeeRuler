Param(
    [parameter(mandatory)][string]$workDir
)

#$workDir = "Pngs"

# error to break
trap { break }

$ErrorActionPreference = "stop"


function Copy-AlfFormLigtUnplated($baseName)
{
    $baseNameEscaped = [Regex]::Escape($baseName)

    $items = Get-ChildItem -Path $workDir -Filter "$baseName*_altform-unplated.png"
    foreach($item in $items)
    {
        if ($item.Name -match "$baseNameEscaped(?<size>\d+)_altform-unplated\.png")
        {
            $size = $matches["size"]
            Copy-Item $item "$workDir\$baseName${size}_altform-lightunplated.png"
        }
    }
}

Copy-AlfFormLigtUnplated "AppList.targetsize-"



