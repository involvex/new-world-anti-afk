@echo off
echo Building portable single-file version...

REM Restore NuGet packages
dotnet restore

REM Clean previous publish output
dotnet clean -c Release

REM .NET 8.0 Self-contained, Single-file Publish
dotnet publish NewWorldAfkPreventer.csproj -c Release -r win-x64 --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:IncludeAllContentForSelfExtract=true ^
    /p:EnableCompressionInSingleFile=true

if errorlevel 1 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Portable build successful!
echo Output:
echo - .\bin\Release\net8.0-windows\win-x64\publish\NewWorldAfkPreventer.exe
echo.
pause