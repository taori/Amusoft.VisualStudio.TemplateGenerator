$VerbosePreference="Continue"

# see https://github.com/meziantou/Meziantou.Analyzer/build

$build = $args[0]
$projectPath = $args[1]
$variableName = $args[2]

Write-Host "Path to project: $projectPath"

# Read version from csproj
[xml]$content = Get-Content $projectPath
$version = Select-Xml -Path $projectPath -XPath //Project/PropertyGroup/PackageVersion | Select -ExpandProperty Node | Select -Expand '#text'
$version = "$version.$build"

Write-Output "##vso[task.setvariable variable=$variableName;]$version"
Write-Host "Set environment variable [$variableName] to ($version)"