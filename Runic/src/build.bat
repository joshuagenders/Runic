cd ./Runic
dotnet restore
dotnet build
nuget.exe pack Runic.nuspec
dir
xcopy "*.nupkg" "c:/code/NugetSources" /y /i
del "*.nupkg"
dir