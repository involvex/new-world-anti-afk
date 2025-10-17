@echo off
echo New World AFK Preventer - Create Release
echo =======================================
echo.

REM Check if Git is available
git --version >nul 2>&1
if errorlevel 1 (
    echo ❌ Git is not installed or not in PATH
    echo    Please install Git from https://git-scm.com/
    pause
    exit /b 1
)

REM Check if we're in a Git repository
git rev-parse --git-dir >nul 2>&1
if errorlevel 1 (
    echo ❌ Not in a Git repository
    echo    Please initialize Git and set up remote repository first
    pause
    exit /b 1
)

REM Check if we have uncommitted changes
git diff --quiet HEAD
if errorlevel 1 (
    echo ⚠️  You have uncommitted changes
    echo    Please commit or stash changes before creating a release
    echo.
    choice /C YN /M "Continue anyway?"
    if errorlevel 2 exit /b 1
)

echo Building project...
echo.

REM Build the project
dotnet build --configuration Release
if errorlevel 1 (
    echo ❌ Build failed!
    pause
    exit /b 1
)

echo ✅ Build successful
echo.

REM Get current version from project file
for /f "tokens=2 delims==" %%i in ('findstr "AssemblyVersion" NewWorldAfkPreventer.csproj') do (
    set "VERSION=%%i"
)
set "VERSION=%VERSION:"=%"
set "VERSION=%VERSION: =%"
set "TAG_VERSION=v%VERSION%"

echo Current version: %VERSION%
echo Release tag will be: %TAG_VERSION%
echo.

REM Check if tag already exists
git tag -l "%TAG_VERSION%" | findstr "%TAG_VERSION%" >nul
if not errorlevel 1 (
    echo ⚠️  Tag %TAG_VERSION% already exists
    echo.
    choice /C YN /M "Overwrite existing tag?"
    if errorlevel 2 (
        echo Release creation cancelled
        pause
        exit /b 1
    )
    echo Deleting existing tag...
    git tag -d "%TAG_VERSION%"
)

echo.
echo Creating release steps:
echo 1. Create and push tag: %TAG_VERSION%
echo 2. GitHub Actions will automatically build and create release
echo.

choice /C YN /M "Proceed with release creation?"
if errorlevel 2 (
    echo Release creation cancelled
    pause
    exit /b 0
)

echo.
echo Creating tag %TAG_VERSION%...
git tag "%TAG_VERSION%"
if errorlevel 1 (
    echo ❌ Failed to create tag
    pause
    exit /b 1
)

echo.
echo Pushing tag to trigger release workflow...
git push origin "%TAG_VERSION%"
if errorlevel 1 (
    echo ❌ Failed to push tag
    echo    The tag was created locally but not pushed
    echo    You can push it manually with: git push origin %TAG_VERSION%
    pause
    exit /b 1
)

echo.
echo ✅ Release process started!
echo.
echo What happens next:
echo 1. GitHub receives the tag push
echo 2. GitHub Actions workflow triggers automatically
echo 3. Project builds on Windows runner
echo 4. Release is created with build artifacts
echo 5. Check Actions tab for progress
echo.
echo Release URL will be available at:
echo https://github.com/%USERNAME%/%REPOSITORY%/releases/tag/%TAG_VERSION%
echo.
echo Note: You may need to enable GitHub Actions in your repository settings
echo.

pause
