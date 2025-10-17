@echo off
echo Building New World AFK Preventer...

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo Error: .NET SDK is not installed.
    echo Please install .NET 6.0 or later from https://dotnet.microsoft.com/
    pause
    exit /b 1
)

REM Restore packages
echo Restoring packages...
dotnet restore

REM Build the application
echo Building application...
dotnet build --configuration Release

if errorlevel 1 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Build successful!
echo.
echo Starting New World AFK Preventer...
echo.
echo Controls:
echo - Press Ctrl+F12 to toggle AFK prevention on/off
echo - Right-click the tray icon for menu options
echo - Double-click the tray icon to toggle
echo.
echo Press Ctrl+C in this window to stop the application
echo.

REM Run the application
dotnet run --configuration Release

pause
