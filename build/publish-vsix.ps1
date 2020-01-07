$PersonalAccessToken = $args[0]
$VsixPath = "$PSScriptRoot\..\src\Amusoft.CodeAnalysis.Analyzers.Vsix\bin\Release\Amusoft.CodeAnalysis.Analyzers.vsix"
$ManifestPath = "$PSScriptRoot\extension-manifest.json"
Write-Host "Using vsix from $VsixPath"
Write-Host "Using manifest from $ManifestPath"

$Installation = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -format json | ConvertFrom-Json
$Path = $Installation.installationPath

Write-Host $Path
$VsixPublisher = Join-Path -Path $Path -ChildPath "VSSDK\VisualStudioIntegration\Tools\Bin\VsixPublisher.exe" -Resolve

Write-Host $VsixPublisher
Test-Path $VsixPublisher

& $VsixPublisher publish -payload $VsixPath -publishManifest $ManifestPath -personalAccessToken $PersonalAccessToken -ignoreWarnings "VSIXValidatorWarning01,VSIXValidatorWarning02,VSIXValidatorWarning08"