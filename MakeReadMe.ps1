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
    "- (準備中) Vector"
    "- (準備中) Microsoft Store"
    ""
)
    
$lines += (Get-Content ".\MakePackage\Readme\ja-jp\Environment.md")

$lines
