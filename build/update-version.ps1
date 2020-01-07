$VerbosePreference="Continue"

# see https://github.com/meziantou/Meziantou.Analyzer/build

$version = $args[0]
$analyzerProjectPath = $args[1]
$vsixSourceManifestPath = $args[2]

if (!$version) {
    $version = "0.0.0"
}

Write-Host "Version: $version"
Write-Host "Path to analyzer project: $analyzerProjectPath"
Write-Host "Path to vsix project manifest file: $vsixSourceManifestPath"

# Update NuGet package version
$FullPath = $analyzerProjectPath
Write-Host $FullPath
[xml]$content = Get-Content $FullPath
Write-Host $content
$content.Project.PropertyGroup.PackageVersion = $version
$content.Save($FullPath)

# Update VSIX version
$FullPath = $vsixSourceManifestPath
Write-Host $FullPath
[xml]$content = Get-Content $FullPath
$content.PackageManifest.Metadata.Identity.Version = $version
$content.Save($FullPath)