@echo off
echo Icon Setup for New World AFK Preventer
echo =====================================
echo.
echo The application now automatically bundles the app.ico file with each build.
echo.
echo Current status:
if exist "app.ico" (
    echo ✅ app.ico found - Your custom icon will be used in the tray
) else (
    echo ⚠️  app.ico not found - Application will use default system icon
)
echo.
echo To add a custom icon:
echo 1. Create or download an icon file (recommended: 16x16, 32x32, or 48x48 pixels)
echo 2. Save it as 'app.ico' in the project root directory
echo 3. Rebuild the application
echo.
echo The icon will automatically appear in:
echo - bin\Release\net8.0-windows\win-x64\app.ico
echo - System tray when the application runs
echo.
echo Icon sources:
echo - Online converters: convertio.co, cloudconvert.com
echo - Free icons: icons8.com, flaticon.com
echo - Image editors: GIMP, Paint.NET, Photoshop
echo.
pause
