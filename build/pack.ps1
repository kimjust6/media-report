Param([string] $configuration = "Release")
$workingDirectory = Get-Location
$zip = "$workingDirectory\build\7za.exe"

# Set location to the Solution directory
(Get-Item $PSScriptRoot).Parent.FullName | Push-Location

# Version
[xml] $versionFile = Get-Content ".\build\version.props"
$versionNode = $versionFile.SelectSingleNode("Project/PropertyGroup/VersionPrefix")
$version = $versionNode.InnerText

[xml] $dependenciesFile = Get-Content ".\build\dependencies.props"
# CMS dependency
$cmsUINode = $dependenciesFile.SelectSingleNode("Project/PropertyGroup/CmsUIVersion")
$cmsUIVersion = $cmsUINode.InnerText
$cmsUIParts = $cmsUIVersion.Split(".")
$cmsUIMajor = [int]::Parse($cmsUIParts[0]) + 1
$cmsUINextMajorVersion = ($cmsUIMajor.ToString() + ".0.0")

# Runtime compilation dependency
$runtimeNode = $dependenciesFile.SelectSingleNode("Project/PropertyGroup/RuntimeCompilationVersion")
$runtimeVersion = $runtimeNode.InnerText
$runtimeParts = $runtimeVersion.Split(".")
$runtimeMajor = [int]::Parse($runtimeParts[0]) + 1
$runtimeNextMajorVersion = ($runtimeMajor.ToString() + ".0.0")

#cleanup all by dtk folder which is used by tests
Get-ChildItem -Path out\ -Exclude dtk | Remove-Item -Recurse -Force

#copy assets approval reviews
Copy-Item -Path MediaReport\ClientResources\ -Destination out\Alloy.MediaReport\$version\ClientResources -recurse -Force
Copy-Item MediaReport\module.config out\Alloy.MediaReport
((Get-Content -Path out\Alloy.MediaReport\module.config -Raw).TrimEnd() -Replace '=""', "=`"$version`"" ) | Set-Content -Path out\Alloy.MediaReport\module.config
Set-Location $workingDirectory\out\Alloy.MediaReport
Start-Process -NoNewWindow -Wait -FilePath $zip -ArgumentList "a", "Alloy.MediaReport.zip", "$version", "module.config"
Set-Location $workingDirectory

# Packaging public packages
dotnet pack -c $configuration /p:PackageVersion=$version /p:CmsUIVersion=$cmsUIVersion /p:CmsUINextMajorVersion=$cmsUINextMajorVersion /p:RuntimeVersion=$runtimeVersion /p:RuntimeNextMajorVersion=$runtimeNextMajorVersion MediaReport.sln

Pop-Location
