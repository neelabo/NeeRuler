# パッケージ生成スクリプト
#
# 使用ツール：
#   - Wix Toolset
#   - pandoc

Param(
	[ValidateSet("All", "Zip", "Installer", "Appx", "Canary", "Beta", "Dev")]$Target = "All",

	# ビルドをスキップする
	[switch]$continue,

	# ログ出力のあるパッケージ作成
	[switch]$trace,

	# MSI作成時にMainComponents.wsxを更新する
	[switch]$updateComponent,

	# Postfix. Canary や Beta での番号重複回避用
	[string]$versionPostfix = ""
)

# error to break
trap { break }

$ErrorActionPreference = "stop"

# check parameters
Write-Host "Target: $Target"
Write-Host "Continue: $continue"
Write-Host "Trace: $trace"
Write-Host "UpdateComponent: $updateComponent" 
Write-Host "VersionPostfix: $versionPostfix" 
Write-Host
Read-Host "Press Enter to continue"

# environment
$product = 'NeeRuler'
$Win10SDK = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64"
$issuesUrl = "https://github.com/neelabo/NeeRuler/issues"

# sync current directory
[System.IO.Directory]::SetCurrentDirectory((Get-Location -PSProvider FileSystem).Path)


# get file version
function Get-FileVersion($fileName)
{
	throw "not supported."

	$major = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($fileName).FileMajorPart
	$minor = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($fileName).FileMinorPart

	"$major.$minor"
}

# get version from .csproj and git
function Get-Version($projectFile)
{
	$xml = [xml](Get-Content $projectFile)
	
	$version = [String]$xml.Project.PropertyGroup.VersionPrefix
	
	if ($version -match '\d+\.\d+\.\d+') {
		return $version
	}
	
	throw "Cannot get version."
}

# create display version (MajorVersion.MinorVersion) from raw version
function Get-AppVersion($version) {
	$tokens = $version.Split(".")
	if ($tokens.Length -ne 3) {
		throw "Wrong version format."
	}
	$tokens = $version.Split(".")
	$majorVersion = [int]$tokens[0]
	$minorVersion = [int]$tokens[1]
	return "$majorVersion.$minorVersion"
}

# get git log
function Get-GitLog()
{
    $branch = Invoke-Expression "git rev-parse --abbrev-ref HEAD"
    $descrive = Invoke-Expression "git describe --abbrev=0 --tags"
	$date = Invoke-Expression 'git log -1 --pretty=format:"%ad" --date=iso'
	$result = Invoke-Expression "git log $descrive..head --encoding=Shift_JIS --pretty=format:`"%ae %s`""
	$result = $result | Where-Object {$_ -match "^nee.laboratory"} | ForEach-Object {$_ -replace "^[\w\.@]+ ",""}
	$result = $result | Where-Object { -not ($_ -match '^m.rge|^開発用|^作業中|\(dev\)|^-|^\.\.') } 

    return "[${branch}] $descrive to head", $date, $result
}

# get git log (markdown)
function Get-GitLogMarkdown($title)
{
    $result = Get-GitLog
	$header = $result[0]
	$date = $result[1]
    $logs = $result[2]

	"## $title"
	"### $header"
	"Rev. $revision / $date"
	""
	$logs | ForEach-Object { "- $_" }
	""
	"This list of changes was auto generated."
}

# replace keyword
function Replace-Content
{
	Param([string]$filepath, [string]$rep1, [string]$rep2)
	if ( $(Test-Path $filepath) -ne $True )
	{
		Write-Error "file not found"
		return
	}
	# input UTF8, output UTF8
	$file_contents = $(Get-Content -Encoding UTF8 $filepath) -replace $rep1, $rep2
	$file_contents | Out-File -Encoding UTF8 $filepath
}

# build
function Build-Project($platform, $outputDir, $options)
{
	$defaultOptions = @(
		"-p:PublishProfile=FolderProfile-$platform.pubxml"
		"-c", "Release"
	)

	& dotnet publish $project $defaultOptions $options -o Publish\$outputDir
	if ($? -ne $true)
	{
		throw "build error"
	}
}

function Build-ProjectSelfContained($platform)
{
	$options = @(
		"--self-contained", "true"
	)

	Build-Project $platform "$product-$platform" $options
}

# package section
function New-Package($platform, $productName, $productDir, $packageDir)
{
	$temp = New-Item $packageDir -ItemType Directory

	Copy-Item $productDir\* $packageDir -Recurse -Exclude ("*.pdb", "$product.exe.config")

	# custom config
	New-ConfigForZip $productDir "$productName.exe.config" $packageDir

	# generate README.html
	New-Readme $packageDir "en-us" ".zip"
	New-Readme $packageDir "ja-jp" ".zip"
}

# generate README.html
function New-Readme($packageDir, $culture, $target)
{
	$readmeSource = "Readme\$culture"

	$readmeDir = $packageDir + "\readme.$culture"

	$temp = New-Item $readmeDir -ItemType Directory 

	Copy-Item "$readmeSource\Overview.md" $readmeDir
	#Copy-Item "$readmeSource\Canary.md" $readmeDir
	Copy-Item "$readmeSource\Package-zip.md" $readmeDir
	Copy-Item "$readmeSource\Package-msi.md" $readmeDir
	Copy-Item "$readmeSource\Package-appx.md" $readmeDir
	Copy-Item "$readmeSource\Environment.md" $readmeDir
	Copy-Item "$readmeSource\Contact.md" $readmeDir
	Copy-Item "$readmeSource\Contact.md" $readmeDir
	Copy-Item "$readmeSource\License.md" $readmeDir

	#Copy-Item "$solutionDir\THIRDPARTY_LICENSES.md" $readmeDir

	if ($target -eq ".canary")
	{
		Get-GitLogMarkdown "$product <VERSION/> - ChangeLog" | Set-Content -Encoding UTF8 "$readmeDir\ChangeLog.md"
	}
	else
	{
		Copy-Item "$readmeSource\ChangeLog.md" $readmeDir
	}

	$license = Get-Content "$solutionDir\LICENSE" -Raw
	$postfix = $appVersion
	$announce = ""
	if ($target -eq ".canary")
	{
		$postfix = "Canary ${dateVersion}"
		#$announce = "Rev. ${revision}`r`n`r`n" + (Get-Content -Path "$readmeDir/Canary.md" -Raw -Encoding UTF8)
	}

	# edit README.md
	Replace-Content "$readmeDir\Overview.md" "<VERSION/>" "$postfix"
	Replace-Content "$readmeDir\Overview.md" "<ANNOUNCE/>" "$announce"
	Replace-Content "$readmeDir\Environment.md" "<VERSION/>" "$postfix"
	Replace-Content "$readmeDir\Contact.md" "<VERSION/>" "$postfix"
	Replace-Content "$readmeDir\License.md" "<LICENSE/>" "$license"
	Replace-Content "$readmeDir\ChangeLog.md" "<VERSION/>" "$postfix"

	$readmeHtml = "README.html"

	if (-not ($culture -eq "en-us"))
	{
		$readmeHtml = "README.$culture.html"
	}

	$inputs = @()
	$inputs += "$readmeDir\Overview.md"

	if ($target -eq ".appx")
	{
		$inputs += "$readmeDir\Package-appx.md"
	}
	elseif ($target -eq ".msi")
	{
		$inputs += "$readmeDir\Package-msi.md"
	}
	else {
		$inputs += "$readmeDir\Package-zip.md"
	}

	$inputs += "$readmeDir\Environment.md"

	$inputs += "$readmeDir\Contact.md"
	$inputs += "$readmeDir\License.md"

	#if ($culture -eq "ja-jp")
	#{
	#	$inputs += "$readmeDir\LICENSE.ja-jp.md"
	#}

	#$inputs += "$readmeDir\THIRDPARTY_LICENSES.md"
	$inputs += "$readmeDir\ChangeLog.md"

	$output = "$packageDir\$readmeHtml"
	$css = "Readme\Style.html"
	
	# markdown to html by pandoc
	pandoc -s -t html5 -o $output --metadata title="$product $postfix" -H $css $inputs
	if ($? -ne $true)
	{
		throw "pandoc error"
	}

	Remove-Item $readmeDir -Recurse
}

# remove ZIP
function Remove-Zip($packageZip)
{
	if (Test-Path $packageZip)
	{
		Remove-Item $packageZip
	}
}

# archive to ZIP
function New-Zip($packageDir, $packageZip)
{
	Compress-Archive $packageDir -DestinationPath $packageZip
}

function New-ConfigForZip($inputDir, $config, $outputDir)
{
	# make config for zip
	[xml]$xml = Get-Content "$inputDir\$config"

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'PackageType' } | Select -First 1
	$add.value = '.zip'

	#$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'Watermark' } | Select -First 1
	#$add.value = 'False'

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'UseSharedAppData' } | Select -First 1
	$add.value = 'False'

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'Revision' } | Select -First 1
	$add.value = $revision

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'DateVersion' } | Select -First 1
	$add.value = $dateVersion
	
	if ($trace)
	{
		#<add key="LogFile" value="TraceLog.txt" />
		$attribute1 = $xml.CreateAttribute('key')
		$attribute1.Value = 'LogFile';
		$attribute2 = $xml.CreateAttribute('value')
		$attribute2.Value = 'TraceLog.txt';
		$element = $xml.CreateElement('add');
		$element.Attributes.Append($attribute1);
		$element.Attributes.Append($attribute2);
		$xml.configuration.appSettings.AppendChild($element);
	}

	$utf8WithoutBom = New-Object System.Text.UTF8Encoding($false)
	$outputFile = Join-Path (Convert-Path $outputDir) $config

	$sw = New-Object System.IO.StreamWriter($outputFile, $false, $utf8WithoutBom)
	$xml.Save( $sw )
	$sw.Close()
}

function New-ConfigForMsi($inputDir, $config, $outputDir)
{
	# make config for installer
	[xml]$xml = Get-Content "$inputDir\$config"

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'PackageType' } | Select -First 1
	$add.value = '.msi'

	#$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'Watermark' } | Select -First 1
	#$add.value = 'False'

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'UseSharedAppData' } | Select -First 1
	$add.value = 'True'

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'Revision' } | Select -First 1
	$add.value = $revision

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'DateVersion' } | Select -First 1
	$add.value = $dateVersion

	$utf8WithoutBom = New-Object System.Text.UTF8Encoding($false)
	$outputFile = Join-Path (Convert-Path $outputDir) $config
	$sw = New-Object System.IO.StreamWriter($outputFile, $false, $utf8WithoutBom)
	$xml.Save( $sw )
	$sw.Close()
}

function New-ConfigForAppx($inputDir, $config, $outputDir)
{
	# make config for appx
	[xml]$xml = Get-Content "$inputDir\$config"

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'PackageType' } | Select -First 1
	$add.value = '.appx'

	#$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'Watermark' } | Select -First 1
	#$add.value = 'False'

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'UseSharedAppData' } | Select -First 1
	$add.value = 'True'

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'Revision' } | Select -First 1
	$add.value = $revision

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'DateVersion' } | Select -First 1
	$add.value = $dateVersion

	$utf8WithoutBom = New-Object System.Text.UTF8Encoding($false)
	$outputFile = Join-Path (Convert-Path $outputDir) $config

	$sw = New-Object System.IO.StreamWriter($outputFile, $false, $utf8WithoutBom)
	$xml.Save( $sw )
	$sw.Close()
}

function New-ConfigForDevPackage($inputDir, $config, $target, $outputDir)
{
	# make config for canary
	[xml]$xml = Get-Content "$inputDir\$config"

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'PackageType' } | Select -First 1
	$add.value = $target

	#$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'Watermark' } | Select -First 1
	#$add.value = 'True'

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'UseSharedAppData' } | Select -First 1
	$add.value = 'False'

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'Revision' } | Select -First 1
	$add.value = $revision

	$add = $xml.configuration.appSettings.add | Where { $_.key -eq 'DateVersion' } | Select -First 1
	$add.value = $dateVersion

	$utf8WithoutBom = New-Object System.Text.UTF8Encoding($false)
	$outputFile = Join-Path (Convert-Path $outputDir) $config

	$sw = New-Object System.IO.StreamWriter($outputFile, $false, $utf8WithoutBom)
	$xml.Save( $sw )
	$sw.Close()
}

function New-EmptyFolder($dir)
{
	# remove folder
	if (Test-Path $dir)
	{
		Remove-Item $dir -Recurse
		Start-Sleep -m 100
	}

	# make folder
	$temp = New-Item $dir -ItemType Directory
}

function New-PackageAppend($packageDir, $packageAppendDir)
{
	New-EmptyFolder $packageAppendDir

	# configure customize
	New-ConfigForMsi $packageDir "${product}.exe.config" $packageAppendDir

	# generate README.html
	New-Readme $packageAppendDir "en-us" ".msi"
	New-Readme $packageAppendDir "ja-jp" ".msi"

	# icons
	Copy-Item "$projectDir\Resources\App.ico" $packageAppendDir
}

# remove Msi
function Remove-Msi($packageAppendDir, $packageMsi)
{
	if (Test-Path $packageMsi)
	{
		Remove-Item $packageMsi
	}

	if (Test-Path $packageAppxDir_x64)
	{
		Remove-Item $packageAppxDir_x64 -Recurse
	}
}

# Msi
function New-Msi($arch, $packageDir, $packageAppendDir, $packageMsi) {

    # Require Wix toolset (e.g.; version 5.0.2)
    # > dotnet tool install --global wix -version 5.0.2
    #
    # Use WiX UI Extension
    # > wix extension add --global WixToolset.UI.wixext/5.0.2

    $wisubstg = "$Win10SDK\wisubstg.vbs"
    $wilangid = "$Win10SDK\wilangid.vbs"

    function New-MsiSub($arch, $culture, $packageMsi) {

        $properties = @(
            "-d", "AppVersion=$appVersion",
            "-d", "ProductVersion=$version",
            "-b", "Contents=$(Convert-Path $packageDir)",
            "-b", "Append=$(Convert-Path $packageAppendDir)",
            "-ext", "WixToolset.UI.wixext"
        )

        Write-Host "[wix] build $packageMsi -culture $culture" -fore Cyan 
        & wix.exe build -arch $arch -out $packageMsi -culture $culture @properties WixSource\*.wx?
        if ($? -ne $true) {
            throw "wix build error"
        }
    }

    $1033Msi = "$packageAppendDir\1033.msi"
    $1041Msi = "$packageAppendDir\1041.msi"
    $1041Mst = "$packageAppendDir\1041.mst"

    New-MsiSub $arch "en-us" $1033Msi
    New-MsiSub $arch "ja-jp" $1041Msi 

    Write-Host "[wix] msi transform $1041Mst" -fore Cyan
    & wix.exe msi transform -p -t language $1033Msi $1041Msi -out $1041Mst
    if ($? -ne $true) {
        throw "wix msi transform error"
    }

    Copy-Item $1033Msi $packageMsi

    Write-Host "[msi] wisubstg $packageMsi" -fore Cyan
    & cscript "$wisubstg" "$packageMsi" $1041Mst 1041
    if ($? -ne $true) {
        throw "$wisubstg error"
    }

    Write-Host "[msi] wilangid $packageMsi" -fore Cyan
    & cscript "$wilangid" "$packageMsi" Package 1033,1041
    if ($? -ne $true) {
        throw "$wilangid error"
    }
}

# Appx remove
function Remove-Appx($packageAppendDir, $appx)
{
	if (Test-Path $appx)
	{
		Remove-Item $appx
	}

	if (Test-Path $packageAppxDir_x64)
	{
		Remove-Item $packageAppxDir_x64 -Recurse
	}
}

# Appx 
function New-Appx($arch, $packageDir, $packageAppendDir, $appx)
{
	$packgaeFilesDir = "$packageAppendDir/PackageFiles"
	$contentDir = "$packgaeFilesDir/$product"

	# copy package base files
	Copy-Item "Appx\Resources" $packgaeFilesDir -Recurse -Force

	# copy resources.pri
	Copy-Item "Appx\resources.pri" $packgaeFilesDir

	# update assembly
	Copy-Item $packageDir $contentDir -Recurse -Force
	New-ConfigForAppx $packageDir "${product}.exe.config" $contentDir

	# generate README.html
	New-Readme $contentDir "en-us" ".appx"
	New-Readme $contentDir "ja-jp" ".appx"

	$param = Get-Content -Raw $env:CersPath/_$product.Parameter.json | ConvertFrom-Json
	$appxName = $param.name
	$appxPublisher = $param.publisher

	# generate AppManifest
	$content = Get-Content "Appx\AppxManifest.xml"
	$content = $content -replace "%NAME%","$appxName"
	$content = $content -replace "%PUBLISHER%","$appxPublisher"
	$content = $content -replace "%VERSION%","$assemblyVersion"
	$content = $content -replace "%ARCH%", "$arch"
	$content | Out-File -Encoding UTF8 "$packgaeFilesDir\AppxManifest.xml"

	# re-package
	& "$Win10SDK\makeappx.exe" pack /l /d "$packgaeFilesDir" /p "$appx"
	if ($? -ne $true)
	{
		throw "makeappx.exe error"
	}

	# signing
	& "$Win10SDK\signtool.exe" sign -f "$env:CersPath/_NeeLaboratory.pfx" -fd SHA256 -v "$appx"
	if ($? -ne $true)
	{
		throw "signtool.exe error"
	}
}

# archive to Canary.ZIP
function Remove-Canary()
{
	if (Test-Path $packageCanary)
	{
		Remove-Item $packageCanary
	}

	if (Test-Path $packageCanaryDir)
	{
		Remove-Item $packageCanaryDir -Recurse
	}
}

function New-Canary($packageDir)
{
	New-DevPackage $packageDir $packageCanaryDir $packageCanary ".canary"
}

# archive to Beta.ZIP
function Remove-Beta()
{
	if (Test-Path $packageBeta)
	{
		Remove-Item $packageBeta
	}

	if (Test-Path $packageBetaDir)
	{
		Remove-Item $packageBetaDir -Recurse
	}
}

function New-Beta($packageDir)
{
	New-DevPackage $packageDir $packageBetaDir $packageBeta ".beta"
}

# archive to Canary/Beta.ZIP
function New-DevPackage($packageDir, $devPackageDir, $devPackage, $target)
{
	# update assembly
	Copy-Item $packageDir $devPackageDir -Recurse
	New-ConfigForDevPackage $packageDir "${product}.exe.config" $target $devPackageDir

	# generate README.html
	New-Readme $devPackageDir "en-us" $target
	New-Readme $devPackageDir "ja-jp" $target

	Compress-Archive $devPackageDir -DestinationPath $devPackage
}

# remove build objects
function Remove-BuildObjects
{
	Get-ChildItem -Directory "$packagePrefix*" | Remove-Item -Recurse

	Get-ChildItem -File "$packagePrefix*.*" | Remove-Item

	if (Test-Path $publishDir)
	{
		Remove-Item $publishDir -Recurse
	}
	if (Test-Path $packageCanaryDir)
	{
		Remove-Item $packageCanaryDir -Recurse -Force
	}
	if (Test-Path $packageBetaDir)
	{
		Remove-Item $packageBetaDir -Recurse -Force
	}
	if (Test-Path $packageCanaryWild)
	{
		Remove-Item $packageCanaryWild
	}
	if (Test-Path $packageBetaWild)
	{
		Remove-Item $packageBetaWild
	}

	Start-Sleep -m 100
}

function Build-Clear
{
	# clear
	Write-Host "`n[Clear] ...`n" -fore Cyan
	Remove-BuildObjects
}

function Build-PackageSorce-x64
{
	if (Test-Path $publishDir_x64) { return }

	# build
	Write-Host "`n[Build] ...`n" -fore Cyan
	Build-ProjectSelfContained "x64"
	
	# create package source
	Write-Host "`n[Package] ...`n" -fore Cyan
	New-Package "x64" $product $publishDir_x64 $packageDir_x64
}

function Build-Zip-x64
{
	Write-Host "`[Zip] ...`n" -fore Cyan

	Remove-Zip $packageZip_x64
	New-Zip $packageDir_x64 $packageZip_x64
	Write-Host "`nExport $packageZip_x64 successed.`n" -fore Green
}

function Build-Installer-x64
{
	Write-Host "`n[Installer] ...`n" -fore Cyan
	
	Remove-Msi $packageAppendDir_x64 $packageMsi_x64
	New-PackageAppend $packageDir_x64 $packageAppendDir_x64
	New-Msi "x64" $packageDir_x64 $packageAppendDir_x64 $packageMsi_x64
	Write-Host "`nExport $packageMsi_x64 successed.`n" -fore Green
}

function Build-Appx-x64
{
	Write-Host "`n[Appx] ...`n" -fore Cyan

	if (Test-Path "$env:CersPath\_Parameter.ps1")
	{
		Remove-Appx $packageAppxDir_x64 $packageX64Appx
		New-Appx "x64" $packageDir_x64 $packageAppxDir_x64 $packageX64Appx
		Write-Host "`nExport $packageX64Appx successed.`n" -fore Green
	}
	else
	{
		Write-Host "`nWarning: not exist make appx envionment. skip!`n" -fore Yellow
	}
}

function Build-Canary
{
	Write-Host "`n[Canary] ...`n" -fore Cyan
	Remove-Canary
	New-Canary $packageDir_x64
	Write-Host "`nExport $packageCanary successed.`n" -fore Green
}

function Build-Beta
{
	Write-Host "`n[Beta] ...`n" -fore Cyan
	Remove-Beta
	New-Beta $packageDir_x64
	Write-Host "`nExport $packageBeta successed.`n" -fore Green
}

function Export-Current($packageDir)
{
	Write-Host "`n[Current] ...`n" -fore Cyan
	if (Test-Path $packageDir)
	{
		if (-not (Test-Path $product))
		{
			New-Item $product -ItemType Directory
		}
		Copy-Item "$packageDir\*" "$product\" -Recurse -Force
	}
	else
	{
		Write-Host "`nWarning: not exist $packageDir. skip!`n" -fore Yellow
	}
}

function Update-Version {
	Write-Host "`n`[Update NeeRuler Version] ...`n" -fore Cyan
	$versionSuffix = switch ( $Target ) {
		"Dev" { 'dev' }
		"Canary" { 'canary' }
		"Beta" { 'beta' }
		default { '' }
	}
	..\CreateVersionProps.ps1 -suffix $versionSuffix
}


#======================
# main
#======================

# variables
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionDir = Convert-Path "$scriptPath\.."
$solution = "$solutionDir\$product.sln"
$projectDir = "$solutionDir\$product"
$project = "$projectDir\$product.csproj"
$versionProps = "$solutionDir\_Version.props"

Update-Version

# versions
$version = Get-Version $versionProps
$appVersion = Get-AppVersion $version
$assemblyVersion = "$version.0"
$revision = (& git rev-parse --short HEAD).ToString()
$dateVersion = (Get-Date).ToString("MMdd") + $versionPostfix

$publishDir = "Publish"
$publishDir_x64 = "$publishDir\$product-x64"
$packagePrefix = "$product$appVersion"
$packageDir_x64 = "$product$appVersion-x64"
$packageAppendDir_x64 = "$packageDir_x64.append"
$packageZip_x64 = "${product}${appVersion}.zip"
$packageMsi_x64 = "${product}${appVersion}.msi"
$packageAppxDir_x64 = "${product}${appVersion}-appx-x64"
$packageX64Appx = "${product}${appVersion}.msix"
$packageCanaryDir = "${product}Canary"
$packageCanary = "${product}Canary${dateVersion}.zip"
$packageCanaryWild = "${product}Canary*.zip"
$packageBetaDir = "${product}Beta"
$packageBeta = "${product}Beta${dateVersion}.zip"
$packageBetaWild = "${product}Beta*.zip"


if (-not $continue)
{
	Build-Clear
}

if (($Target -eq "All") -or ($Target -eq "Zip"))
{
	Build-PackageSorce-x64
	Build-Zip-x64

}

if (($Target -eq "All") -or ($Target -eq "Installer"))
{
	Build-PackageSorce-x64
	Build-Installer-x64
}

if (($Target -eq "All") -or ($Target -eq "Appx"))
{
	Build-PackageSorce-x64
	Build-Appx-x64
}

# no use Canary and Beta
<#
if (($Target -eq "All") -or ($Target -eq "Canary"))
{
	Build-PackageSorce-x64
	Build-Canary
}

if (($Target -eq "All") -or ($Target -eq "Beta"))
{
	Build-PackageSorce-x64
	Build-Beta
}
#>

if (-not $continue)
{
	Build-PackageSorce-x64
	Export-Current $packageDir_x64
}


# Finish.
Write-Host "`nBuild $version All done.`n" -fore Green





