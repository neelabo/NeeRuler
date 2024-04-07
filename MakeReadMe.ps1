$lines = @(
    "# NeeRuler"
    ""
    "![App Icon](MakePackage/Appx/Resources/Assets/StoreLogo.scale-100.png)"
    ""
)

$phase = 0
$overview = @()
foreach($line in Get-Content ".\MakePackage\Readme\ja-jp\Overview.md")
{
    if ($phase -eq 0)
    {
        if ($line -match "^##")
        {
            $phase = 1
        }
    }
    else
    {
        $overview += $line
    }
}
$lines += $overview

$lines += @(
    ""
    "## ダウンロード"
    ""
    "- [GitHub Releases](https://github.com/neelabo/NeeRuler/releases)"
    "- [Microsoft Store](https://www.microsoft.com/store/apps/9NVP5Q4XM0X7)"
    "- (準備中) Vector"
    ""
)
    
$lines += (Get-Content ".\MakePackage\Readme\ja-jp\Environment.md")

$lines
