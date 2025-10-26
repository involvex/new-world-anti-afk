# New World AFK Script
# Press Shift+Right Arrow to start/pause
# Press Q to quit

param([switch]$TriggerOnce)

# Add required .NET assembly
Add-Type -AssemblyName System.Windows.Forms

# Win32 API declarations for key state checking
$Win32Code = @"
using System;
using System.Runtime.InteropServices;

public class Win32 {
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);
    
    [DllImport("user32.dll")]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
}
"@

# Compile the Win32 API code (only if not already loaded)
try { [Win32] | Out-Null } catch {
    Add-Type -TypeDefinition $Win32Code -Language CSharp
}

# Virtual Key Codes
$VK_SHIFT = 0x10
$VK_RIGHT = 0x27
$VK_Q = 0x51

# Initialize variables
$ispressed = $false
$process = $null
$hwnd = [IntPtr]::Zero

function Get-NewWorldProcess {
    $process = Get-Process -Name "NewWorld" -ErrorAction SilentlyContinue
    if ($null -eq $process) {
        Write-Output "NewWorld.exe not found. Please start the game first."
        return $null
    }
    return $process
}

function Test-KeyCombo {
    param(
        [int]$key1,
        [int]$key2
    )
    $state1 = [Win32]::GetAsyncKeyState($key1)
    $state2 = [Win32]::GetAsyncKeyState($key2)
    return ($state1 -band 0x8000) -ne 0 -and ($state2 -band 0x8000) -ne 0
}

function Test-KeyPressed {
    param([int]$key)
    $state = [Win32]::GetAsyncKeyState($key)
    return ($state -band 0x8000) -ne 0
}

function Send-KeyWithDelay {
    param(
        [string]$key,
        [int]$delayMs = 2000,
        [bool]$release = $true
    )
    try {
        [System.Windows.Forms.SendKeys]::SendWait($key)
        Start-Sleep -Milliseconds $delayMs
        if ($release) {
            if ($key -eq ' ') {
                [System.Windows.Forms.SendKeys]::SendWait("{SPACE up}")
            } else {
                [System.Windows.Forms.SendKeys]::SendWait("{$key up}")
            }
        }
    }
    catch {
        Write-Warning "Error sending key $key : $_"
    }
}

function Invoke-AFKSequence {
    param(
        [int]$minDelay = 10000,
        [int]$maxDelay = 50000,
        [switch]$InteractiveWait = $false
    )
    
    $process = Get-NewWorldProcess
    if ($null -eq $process) { 
        Write-Warning "New World process not found"
        return $false 
    }
    
    $hwnd = $process.MainWindowHandle
    if ($hwnd -eq [IntPtr]::Zero -or $hwnd -eq $null) { 
        Write-Warning "Could not get valid window handle for New World"
        return $false 
    }
    
    try {
        Write-Output "Attempting to focus New World window..."
        $result = [Win32]::SetForegroundWindow($hwnd)
        if (-not $result) {
            Write-Warning "Failed to set foreground window, but continuing..."
        }
    }
    catch {
        Write-Warning "Error setting foreground window: $_"
        Write-Warning "Continuing with key sending anyway..."
    }
    
    $random = [System.Random]::new()
    $delay = $random.Next($minDelay, $maxDelay)
    $delaySec = [math]::Round($delay / 1000, 1)
    Write-Output "AFK sequence starting in $delaySec seconds..."
    
    if ($InteractiveWait) {
        # Wait for delay, but check for cancel every second
        for ($i = 0; $i -lt ($delay / 1000); $i++) {
            if (Test-KeyCombo -key1 $VK_SHIFT -key2 $VK_RIGHT) {
                Write-Output "AFK sequence cancelled by user"
                return $false
            }
            Start-Sleep -Seconds 1
        }
    } else {
        Start-Sleep -Milliseconds $delay
    }
    
    # Execute movement sequence
    Send-KeyWithDelay -key "w" -delayMs 2000
    Send-KeyWithDelay -key "s" -delayMs 2000
    Send-KeyWithDelay -key "a" -delayMs 2000
    Send-KeyWithDelay -key "d" -delayMs 2000
    Send-KeyWithDelay -key " " -delayMs 500
    Send-KeyWithDelay -key " " -delayMs 500
    
    return $true
}

if ($TriggerOnce) {
    Invoke-AFKSequence -minDelay 100 -maxDelay 1000
    exit
}

# Main loop
Write-Output "New World AFK Script started"
Write-Output "Press Shift+Right Arrow to start/pause"
Write-Output "Press Q to quit"

try {
    while ($true) {
        if ($ispressed) {
            $success = Invoke-AFKSequence -InteractiveWait
            if (-not $success) {
                $ispressed = $false
                Start-Sleep -Seconds 1
                continue
            }
        }
        else {
            # Check for key combinations
            if (Test-KeyCombo -key1 $VK_SHIFT -key2 $VK_RIGHT) {
                $ispressed = -not $ispressed
                Write-Output "AFK Script $(if ($ispressed) { 'started' } else { 'paused' })"
                Start-Sleep -Milliseconds 500  # Debounce
            }
            elseif (Test-KeyPressed -key $VK_Q) {
                Write-Output "Quitting..."
                break
            }
            Start-Sleep -Milliseconds 100
        }
    }
}
catch {
    Write-Error "An error occurred: $_"
}
finally {
    Write-Output "Script ended"
}
