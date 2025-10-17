# New World AFK Prevention Script with Ctrl+F12 Toggle
# This script prevents AFK kicks by sending smart key presses to the New World window

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms

# C# code to access Win32 API for window management and input simulation
$code = @"
using System;
using System.Runtime.InteropServices;
using System.Threading;

public class Win32 {
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

    public const int KEYEVENTF_KEYUP = 0x0002;
    public const byte VK_F12 = 0x7B;
    public const byte VK_CONTROL = 0x11;
    public const byte VK_W = 0x57;
    public const byte VK_A = 0x41;
    public const byte VK_S = 0x53;
    public const byte VK_D = 0x44;
    public const byte VK_SPACE = 0x20;
    public const byte VK_SHIFT = 0x10;
}
"@

Add-Type -TypeDefinition $code -Language CSharp

# Global variables for the AFK prevention system
$global:isRunning = $false
$global:toggleHotkey = $null
$global:timer = $null
$global:random = New-Object System.Random

# Function to check if New World window exists and is active
function Test-NewWorldWindow {
    $process = Get-Process -Name "NewWorld" -ErrorAction SilentlyContinue
    if ($null -eq $process) {
        return $false
    }

    $hwnd = $process.MainWindowHandle
    if ($hwnd -eq [IntPtr]::Zero) {
        return $false
    }

    # Check if window is visible and not minimized
    $rect = New-Object Win32+Rect
    [Win32]::GetWindowRect($hwnd, [ref]$rect) | Out-Null

    $width = $rect.right - $rect.left
    $height = $rect.bottom - $rect.top

    return ($width -gt 0 -and $height -gt 0)
}

# Function to get New World window handle
function Get-NewWorldWindowHandle {
    $process = Get-Process -Name "NewWorld" -ErrorAction SilentlyContinue
    if ($null -eq $process) {
        return [IntPtr]::Zero
    }
    return $process.MainWindowHandle
}

# Function to send a key press to New World window
function Send-KeyPress {
    param([byte]$key)

    $hwnd = Get-NewWorldWindowHandle
    if ($hwnd -eq [IntPtr]::Zero) {
        Write-Warning "New World window not found"
        return
    }

    # Bring window to foreground first
    [Win32]::SetForegroundWindow($hwnd) | Out-Null
    Start-Sleep -Milliseconds 100

    # Send key press
    [Win32]::keybd_event($key, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 50
    [Win32]::keybd_event($key, 0, [Win32]::KEYEVENTF_KEYUP, [UIntPtr]::Zero)
}

# Smart button press algorithm - sends random movement keys
function Send-SmartKeyPress {
    $keys = @(
        [Win32]::VK_W,  # W - Forward
        [Win32]::VK_A,  # A - Left
        [Win32]::VK_S,  # S - Backward
        [Win32]::VK_D,  # D - Right
        [Win32]::VK_SPACE  # Space - Jump
    )

    # Randomly select a key (weighted towards movement keys)
    $randomKey = $keys[$global:random.Next(0, $keys.Length)]

    # Send the key press
    Send-KeyPress -key $randomKey

    Write-Host "Sent key press to New World window"
}

# Function to start the AFK prevention
function Start-AfkPrevention {
    if ($global:isRunning) {
        Write-Host "AFK prevention is already running"
        return
    }

    if (-not (Test-NewWorldWindow)) {
        Write-Warning "New World window not found or minimized. Please make sure New World is running and visible."
        return
    }

    $global:isRunning = $true
    Write-Host "Starting AFK prevention..."

    # Start the timer that will send key presses at random intervals
    $global:timer = New-Object System.Windows.Forms.Timer
    $global:timer.Interval = Get-RandomInterval
    $global:timer.Add_Tick({
        if ($global:isRunning -and (Test-NewWorldWindow)) {
            Send-SmartKeyPress
            $global:timer.Interval = Get-RandomInterval
        } else {
            Stop-AfkPrevention
        }
    })
    $global:timer.Start()
}

# Function to stop the AFK prevention
function Stop-AfkPrevention {
    if (-not $global:isRunning) {
        Write-Host "AFK prevention is not running"
        return
    }

    $global:isRunning = $false
    Write-Host "Stopping AFK prevention..."

    if ($null -ne $global:timer) {
        $global:timer.Stop()
        $global:timer.Dispose()
        $global:timer = $null
    }
}

# Function to get random interval between key presses (3-8 minutes)
function Get-RandomInterval {
    return $global:random.Next(180000, 480000)  # 3-8 minutes in milliseconds
}

# Function to toggle AFK prevention
function Toggle-AfkPrevention {
    if ($global:isRunning) {
        Stop-AfkPrevention
    } else {
        Start-AfkPrevention
    }
}

# Set up global hotkey handler for Ctrl+F12
function Register-Hotkey {
    # Create a simple message loop to listen for hotkeys
    $signature = @"
    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    public static extern IntPtr GetMessage(IntPtr lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    public const uint WM_HOTKEY = 0x0312;
    public const uint MOD_CONTROL = 0x0002;
    public const uint VK_F12 = 0x7B;
"@

    # Note: In a real implementation, you might need a Windows Forms application
    # to properly handle hotkeys. For now, we'll use a simple approach.
}

# Main execution
Write-Host "==========================================="
Write-Host "New World AFK Prevention Script"
Write-Host "==========================================="
Write-Host ""
Write-Host "NOTE: This PowerShell script provides basic functionality."
Write-Host "For the full experience with global hotkeys, use the C# application instead."
Write-Host ""
Write-Host "To use the C# version:"
Write-Host "1. Run 'build-and-run.bat' to build and start the application"
Write-Host "2. Use Ctrl+F12 globally to toggle AFK prevention"
Write-Host ""
Write-Host "PowerShell script controls:"
Write-Host "- Type 'Start-AfkPrevention' to start"
Write-Host "- Type 'Stop-AfkPrevention' to stop"
Write-Host "- Type 'Toggle-AfkPrevention' to toggle"
Write-Host ""

# Initial check
if (Test-NewWorldWindow) {
    Write-Host "✓ New World window detected successfully"
    Write-Host "  You can now use the functions above to control AFK prevention"
} else {
    Write-Host "⚠ New World window not found. Please start New World first."
}

Write-Host ""
Write-Host "Script is ready. Use the functions above to control AFK prevention."
Write-Host "For better hotkey support, use the C# application instead."
Write-Host ""

# Show available functions
Write-Host "Available functions:"
Write-Host "- Test-NewWorldWindow (checks if window exists)"
Write-Host "- Get-NewWorldWindowHandle (gets window handle)"
Write-Host "- Send-KeyPress -key <key> (sends a key press)"
Write-Host "- Send-SmartKeyPress (sends random smart key press)"
Write-Host "- Start-AfkPrevention (starts the timer)"
Write-Host "- Stop-AfkPrevention (stops the timer)"
Write-Host "- Toggle-AfkPrevention (toggles start/stop)"
Write-Host "- Get-RandomInterval (gets random interval for next press)"
Write-Host ""
