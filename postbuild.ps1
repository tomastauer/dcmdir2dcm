param (
    [string]$solutionDir = $null,
    [string]$targetPath = $null,
    [string]$platformName = $null,
    [string]$targetName = $null,
    [string]$targetFileName = $null
)

$targetName = $targetName.Split('.')[0]

$targetVersion = (Get-Item $targetPath).VersionInfo.ProductVersion.Split('.')
$version = [string]::Format("{0}.{1}.{2}", $targetVersion[0], $targetVersion[1], $targetVersion[2])
$versionPath = $($solutionDir) + "release\" + $targetName + "_" + $version
$xmlPath = [System.IO.Path]::GetFileNameWithoutExtension($targetPath) + ".xml"

New-Item -ItemType Directory -Force -Path $versionPath
New-Item -ItemType Directory -Force -Path ($versionPath + "\" + $platformName)

Copy-Item $targetPath ($versionPath + "\" + $platformName + "\" + $targetName + [System.IO.Path]::GetExtension($targetFileName))
if(Test-Path $xmlPath)
{
    Copy-Item $xmlPath ($versionPath + "\" + $platformName + "\" + $targetName + ".xml")
}

if($platformName -eq "x86" -and $targetFileName.EndsWith("dll"))
{
    Copy-Item $targetPath ($($solutionDir) + "nuget\lib\net46\" + $targetFileName)
    if(Test-Path $xmlPath)
    {
        Copy-Item $xmlPath ($($solutionDir) + "nuget\lib\net46\" + [System.IO.Path]::GetFileNameWithoutExtension($targetFileName) + ".xml")
    }
    
}

Add-Type -Assembly System.IO.Compression.FileSystem
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
$zipPath = ($versionPath + ".zip")

If (Test-Path $zipPath){
	Remove-Item $zipPath
}

[System.IO.Compression.ZipFile]::CreateFromDirectory($versionPath, $zipPath, $compressionLevel, $false)