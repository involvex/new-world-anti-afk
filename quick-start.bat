@echo off
echo New World AFK Preventer - Quick Start
echo ====================================
echo.

REM Check if .NET SDK is available
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ .NET SDK not found!
    echo.
    echo Please install .NET 6.0 or later:
    echo https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    echo After installation, run this script again.
    pause
    exit /b 1
)

REM Check if New World is running
tasklist /FI "IMAGENAME eq NewWorld.exe" 2>NUL | find /I /N "NewWorld.exe">NUL
if errorlevel 1 (
    echo ⚠️  New World is not running
    echo.
    echo Please start New World first, then run this script again.
    echo.
    pause
    exit /b 1
) else (
    echo ✅ New World is running
)

echo.
echo Building and starting AFK Preventer...
echo.

REM Build and run
call build-and-run.bat
