# New World AFK Preventer

A smart AFK prevention tool for New World that sends random key presses to prevent being kicked for inactivity.

## Features

- **Smart Key Algorithm**: Sends random movement keys (WASD, Space) at random intervals (3-8 minutes)
- **Configurable Hotkey**: Customize your own hotkey combination with hotkey recording
- **Window Detection**: Automatically detects if New World is running and visible
- **Tray Icon**: Minimizes to system tray with custom icon and menu options
- **Settings System**: Full settings management with JSON storage
- **Auto-Stop**: Automatically stops if New World window is closed or minimized
- **Bundled Icon**: Custom application icon automatically included in builds

## Requirements

- Windows 10/11
- .NET 6.0 or later (automatically checked by build script)
- New World game running

## Installation & Usage

### Quick Start (Recommended)

1. **Build and run the application:**
   ```batch
   build-and-run.bat
   ```

2. **Start New World** and make sure it's visible on screen

3. **Access settings (optional):**
   - Right-click the tray icon and select "Settings"
   - Configure hotkey, timing, and other preferences
   - Settings are saved automatically

4. **Toggle AFK prevention:**
   - Press your configured hotkey (default: **Ctrl+F12**) to start/stop
   - Or right-click the tray icon and select "Toggle AFK Prevention"
   - Or double-click the tray icon

5. **The application will:**
   - Send random key presses every 3-8 minutes (configurable)
   - Show notifications when starting/stopping (if enabled)
   - Display status in tray icon tooltip

### Manual Build

If you prefer to build manually:

```bash
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

## Settings & Configuration

Access the settings by right-clicking the tray icon and selecting "Settings". The settings window has three tabs:

### Hotkey Tab
- **Current Hotkey**: Shows the currently configured hotkey combination
- **Change Hotkey**: Opens a hotkey recorder to set a new key combination
- **Reset to Default**: Resets hotkey back to Ctrl+F12

### Timing Tab
- **Minimum Interval**: Set the minimum time between key presses (1-60 minutes)
- **Maximum Interval**: Set the maximum time between key presses (1-120 minutes)
- **Note**: The actual interval is randomized between these values

### General Tab
- **Start Minimized**: Application starts minimized to tray
- **Show Notifications**: Enable/disable balloon tip notifications

All settings are automatically saved to `settings.json` and persist between sessions.

## How It Works

1. **Window Detection**: Continuously monitors for the New World process
2. **Smart Timing**: Uses random intervals between min/max settings to avoid detection
3. **Key Selection**: Randomly selects from W, A, S, D, and Space keys
4. **Window Focus**: Brings New World to foreground before sending keys
5. **Auto-Management**: Automatically stops if New World closes

## Safety Features

- **Non-intrusive**: Only sends single key presses, doesn't interfere with gameplay
- **Random timing**: Varies intervals to appear more human-like
- **Window validation**: Only sends keys when New World is actually visible
- **Easy toggle**: Can be instantly stopped with hotkey or tray menu

## Troubleshooting

### Application won't start
- Make sure .NET 6.0+ is installed
- Run `dotnet --version` to check

### Hotkey not working
- Try clicking on the console window first
- Make sure no other application is using Ctrl+F12

### Keys not being sent
- Ensure New World is running and visible
- Check that the game window isn't minimized
- Look for tray icon notifications

### Build errors
- Update to .NET 6.0 or later
- Clean and rebuild: `dotnet clean && dotnet restore`

## Project Structure

```
├── NewWorldAfkPreventer.cs    # Main C# application
├── Settings.cs                # Settings management
├── SettingsForm.cs            # Settings UI form
├── NewWorldAfkPreventer.csproj # .NET project file
├── build-and-run.bat          # Build script
├── quick-start.bat            # Quick start script
├── create-icon.bat            # Icon creation helper
├── fokusnewworldscreenshot.ps1 # Legacy PowerShell script
├── settings.json              # Settings file (created automatically)
├── app.ico                    # Custom tray icon (bundled with application)
└── README.md                  # This file
```

## Technical Details

- **Language**: C#
- **Framework**: .NET 6.0 Windows Forms
- **Input Simulation**: Win32 API keybd_event
- **Hotkey Registration**: Win32 RegisterHotKey
- **Process Detection**: System.Diagnostics.Process

## License

This project is provided as-is for educational purposes.

## Contributing

Feel free to improve the algorithm, add features, or fix issues:

1. Fork the project
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## Changelog

### v1.2.0
- **Bundled Icon**: Custom app.ico now automatically included in all builds
- **Enhanced Icon System**: Application uses custom icon from executable resources
- **Improved Build Process**: Icon is copied to output directory automatically

### v1.1.0
- **Settings System**: Complete settings management with JSON storage
- **Configurable Hotkey**: Custom hotkey combinations with recording functionality
- **Enhanced Tray Menu**: Added settings option and improved menu structure
- **Timing Configuration**: Adjustable minimum and maximum intervals
- **Custom Icon Support**: Support for custom tray icons (app.ico)
- **General Options**: Start minimized and notification preferences

### v1.0.0
- Initial release
- Basic AFK prevention with smart key algorithm
- Global hotkey support (Ctrl+F12)
- Tray icon with status indicator
- Window detection and validation
