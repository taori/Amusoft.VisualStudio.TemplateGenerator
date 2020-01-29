$VerbosePreference="Continue"

# see https://github.com/meziantou/Meziantou.Analyzer/build

function NullCoalesc {
    param (
        [Parameter(ValueFromPipeline=$true)]$Value,
        [Parameter(Position=0)]$Default
    )

    if ($Value) { $Value } else { $Default }
}

Set-Alias -Name "??" -Value NullCoalesc

$build = $args[0]
$projectPath = $args[1]
$variableName = $args[2]

Write-Host "Path to project: $projectPath"

# Read version from csproj
[xml]$content = Get-Content $projectPath
$version = Select-Xml -Path $projectPath -XPath //Project/PropertyGroup/PackageVersion | Select -ExpandProperty Node | Select -Expand '#text'
$versionPrefix = Select-Xml -Path $projectPath -XPath //Project/PropertyGroup/VersionPrefix | Select -ExpandProperty Node | Select -Expand '#text'
$version = $version |?? $versionPrefix
$version = "$version.$build"

Write-Output "##vso[task.setvariable variable=$variableName;]$version"
Write-Host "Set environment variable [$variableName] to ($version)"