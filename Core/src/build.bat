cd ./Runic.Core
dotnet restore
dotnet build
nuget.exe pack Runic.Core.nuspec
dir
xcopy "*.nupkg" "c:/code/NugetSources" /y /i
del "*.nupkg"
dir