@echo off
echo GitHub Repository Setup for New World AFK Preventer
echo ==================================================
echo.

REM Check if Git is available
git --version >nul 2>&1
if errorlevel 1 (
    echo ❌ Git is not installed
    echo    Please install Git from https://git-scm.com/
    pause
    exit /b 1
)

echo This script will help you set up a GitHub repository for releases.
echo.

REM Check if already in a Git repository
git rev-parse --git-dir >nul 2>&1
if errorlevel 1 (
    echo Setting up Git repository...
    echo.

    REM Initialize Git repository
    git init
    if errorlevel 1 (
        echo ❌ Failed to initialize Git repository
        pause
        exit /b 1
    )

    echo ✅ Git repository initialized
    echo.

    REM Create .gitignore for .NET project
    echo Creating .gitignore...
    (
        echo # Build outputs
        echo bin/
        echo obj/
        echo.
        echo # User-specific files
        echo *.user
        echo *.suo
        echo.
        echo # IDE files
        echo .vscode/
        echo .idea/
        echo.
        echo # OS files
        echo .DS_Store
        echo Thumbs.db
        echo.
        echo # Settings file (user-specific)
        echo settings.json
        echo.
        echo # Screenshots (if any)
        echo screenshot.png
    ) > .gitignore

    echo ✅ .gitignore created
    echo.
) else (
    echo ✅ Git repository already exists
    echo.
)

REM Check if remote origin exists
git remote get-url origin >nul 2>&1
if errorlevel 1 (
    echo No remote repository configured.
    echo.
    echo To set up GitHub repository:
    echo.
    echo 1. Create a new repository on GitHub:
    echo    https://github.com/new
    echo.
    echo 2. Copy the repository URL (e.g., https://github.com/username/repo.git)
    echo.
    echo 3. Add it as remote origin:
    echo    git remote add origin https://github.com/username/repo.git
    echo.
    echo 4. Push initial commit:
    echo    git push -u origin main
    echo.
    echo 5. Enable GitHub Actions in repository settings
    echo.
    echo After setup, you can create releases with: create-release.bat
    echo.
) else (
    echo ✅ Remote repository configured
    echo.

    REM Get remote URL
    for /f "delims=" %%i in ('git remote get-url origin') do set "REMOTE_URL=%%i"
    echo Remote repository: %REMOTE_URL%
    echo.

    REM Check if we need to push commits
    git log --oneline -1 >nul 2>&1
    if errorlevel 1 (
        echo No commits found. Creating initial commit...
        echo.

        git add .
        git commit -m "Initial commit: New World AFK Preventer v1.2.0"
        if errorlevel 1 (
            echo ❌ Failed to create initial commit
            pause
            exit /b 1
        )

        echo ✅ Initial commit created
        echo.

        echo Pushing to remote repository...
        git push -u origin main
        if errorlevel 1 (
            echo ❌ Failed to push to remote
            echo    You may need to push manually: git push -u origin main
        ) else (
            echo ✅ Pushed to remote repository
        )
    ) else (
        echo ✅ Repository has commits
        echo.

        REM Check if we need to push
        git diff --quiet HEAD origin/main 2>nul
        if errorlevel 1 (
            echo You have local changes. Pushing...
            git push origin main
            if errorlevel 1 (
                echo ❌ Failed to push changes
            ) else (
                echo ✅ Changes pushed to remote
            )
        ) else (
            echo ✅ Repository is up to date
        )
    )

    echo.
    echo Ready for releases! Use create-release.bat to create a new release.
    echo.
)

pause
