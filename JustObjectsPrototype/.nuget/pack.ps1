$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\JustObjectsPrototype\bin\Release\JustObjectsPrototype.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\.nuget\JustObjectsPrototype.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\.nuget\JustObjectsPrototype.compiled.nuspec

& $root\.nuget\NuGet.exe pack $root\.nuget\JustObjectsPrototype.compiled.nuspec