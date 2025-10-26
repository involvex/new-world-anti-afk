using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace NewWorldAfkPreventer
{
    public class AfkPreventer : Form
    {
        private NotifyIcon? trayIcon;
        private System.ComponentModel.Container? components;
        private bool isRunning;
        private System.Windows.Forms.Timer? timer;
        private readonly Random random = new();
        private AppSettings settings = AppSettings.Load();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left && trayIcon?.ContextMenuStrip != null)
                {
                    Debug.WriteLine("Left click on tray icon, showing context menu");
                    // Get the ShowContextMenu method using reflection
                    var mi = typeof(NotifyIcon).GetMethod("ShowContextMenu",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    mi?.Invoke(trayIcon, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error handling tray icon click: {ex.Message}");
            }
        }

        // Win32 API constants and imports
        private const uint WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, char[] lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        public AfkPreventer()
        {
            // Initialize form
            InitializeComponent();
            this.Load += AfkPreventer_Load;
            this.FormClosing += AfkPreventer_FormClosing;
            this.Resize += AfkPreventer_Resize;

            // Setup tray icon and hotkey
            SetupTrayIcon();
            bool reg = RegisterHotkey();
            UpdateTrayHotkeyStatus(reg);

            // Apply settings
            this.TopMost = settings.AlwaysOnTop;

            // Initial form state
            if (settings.StartMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
        }

        private void AfkPreventer_Load(object? sender, EventArgs e)
        {
            string settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NewWorldAfkPreventer");
            string settingsPath = Path.Combine(settingsDir, "settings.json");

            if (!File.Exists(settingsPath))
            {
                using (SettingsForm settingsForm = new(settings))
                {
                    if (settingsForm.ShowDialog() == DialogResult.OK)
                    {
                        settings = AppSettings.Load();
                        this.TopMost = settings.AlwaysOnTop;
                        ReRegisterHotkey();
                    }
                }
            }

            // Ensure tray icon is visible after form load
            if (trayIcon != null)
            {
                trayIcon.Visible = true;
            }
        }

        private void AfkPreventer_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
        }

        private void AfkPreventer_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                EnsureTrayIconVisible();
            }
            else
            {
                this.ShowInTaskbar = true;
            }
        }

        private void EnsureTrayIconVisible()
        {
            if (trayIcon == null || !trayIcon.Visible)
            {
                Debug.WriteLine("Tray icon not visible, recreating...");
                SetupTrayIcon();
                return;
            }

            try
            {
                trayIcon.Visible = true;
                Debug.WriteLine("Ensuring tray icon visibility");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error ensuring tray visibility: {ex.Message}");
                SetupTrayIcon(); // Recreate if there was an error
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Main timer for AFK prevention
            this.timer = new System.Windows.Forms.Timer(this.components);
            if (this.timer != null)
            {
                this.timer.Interval = GetRandomInterval();
                this.timer.Tick += TimerTick;
            }

            // Timer to ensure tray icon stays visible
            var trayCheckTimer = new System.Windows.Forms.Timer(this.components)
            {
                Interval = 5000 // Check every 5 seconds
            };
            trayCheckTimer.Tick += (s, e) => EnsureTrayIconVisible();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Size = new System.Drawing.Size(600, 400);
        }

        private void SetupTrayIcon()
        {
            try
            {
                Debug.WriteLine("Starting tray icon setup");

                // Ensure we have a component container
                if (components == null)
                {
                    components = new System.ComponentModel.Container();
                    Debug.WriteLine("Created new Container for components");
                }

                // Clean up existing tray icon if any
                if (trayIcon != null)
                {
                    Debug.WriteLine("Cleaning up existing tray icon");
                    try
                    {
                        trayIcon.Visible = false;
                        trayIcon.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error cleaning up old tray icon: {ex.Message}");
                    }
                }

                // Create new tray icon
                Debug.WriteLine("Creating new NotifyIcon");
                trayIcon = new NotifyIcon(components)
                {
                    Text = "New World AFK Preventer",
                    Visible = false // Start invisible, set to true after full setup
                };

                // Set up context menu first
                Debug.WriteLine("Creating context menu");
                var contextMenu = new ContextMenuStrip(components);
                contextMenu.Items.Add("Toggle AFK Prevention", null, OnToggleAfkPrevention);
                contextMenu.Items.Add("Settings", null, OnSettings);
                contextMenu.Items.Add("-");
                contextMenu.Items.Add("Exit", null, OnExit);
                trayIcon.ContextMenuStrip = contextMenu;

                // Set up icon
                try
                {
                    using var stream = new FileStream("logo.png", FileMode.Open);
                    if (stream != null)
                    {
                        Debug.WriteLine("Loading custom icon from logo.png");
                        Bitmap bitmap = new Bitmap(stream);
                        trayIcon.Icon = Icon.FromHandle(bitmap.GetHicon());
                    }
                    else
                    {
                        Debug.WriteLine("Custom icon not found, using system icon");
                        trayIcon.Icon = SystemIcons.Application;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading icon: {ex.Message}");
                    trayIcon.Icon = SystemIcons.Application;
                }

                // Add event handlers
                trayIcon.DoubleClick += OnToggleAfkPrevention;
                trayIcon.MouseClick += TrayIcon_MouseClick;

                // Finally make it visible
                Debug.WriteLine("Making tray icon visible");
                trayIcon.Visible = true;

                Debug.WriteLine("Tray icon setup complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting up tray icon: {ex.Message}");
                MessageBox.Show($"Error setting up tray icon: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static uint ModifiersFromKeys(Keys modifiers)
        {
            uint mask = 0;
            if (modifiers.HasFlag(Keys.Control)) mask |= 0x0002; // MOD_CONTROL
            if (modifiers.HasFlag(Keys.Alt)) mask |= 0x0001; // MOD_ALT
            if (modifiers.HasFlag(Keys.Shift)) mask |= 0x0004; // MOD_SHIFT
            return mask;
        }

        private bool TryRegisterCombination(Keys key, Keys modifier)
        {
            uint modifiers = ModifiersFromKeys(modifier);
            try
            {
                UnregisterHotKey(this.Handle, 0); // Clean up any existing registration
                return RegisterHotKey(this.Handle, 0, modifiers, (uint)key);
            }
            catch
            {
                return false;
            }
        }

        private bool RegisterHotkey()
        {
            Keys originalKey = settings.Hotkey;
            Keys originalModifier = settings.HotkeyModifier;

            // Try original combination first
            if (TryRegisterCombination(originalKey, originalModifier))
            {
                ShowNotification($"Hotkey registered: {originalModifier} + {originalKey}");
                return true;
            }

            ShowNotification("Failed to register hotkey. Using default Ctrl+F12.");
            settings.Hotkey = Keys.F12;
            settings.HotkeyModifier = Keys.Control;
            settings.Save();
            return TryRegisterCombination(Keys.F12, Keys.Control);
        }

        private void UpdateTrayHotkeyStatus(bool registered)
        {
            if (trayIcon == null) return;

            string text = $"New World AFK Preventer - {(registered ? "Hotkey active" : "Hotkey inactive")}";
            text += $"\nHotkey: {settings.HotkeyModifier} + {settings.Hotkey}";
            text += $"\nStatus: {(isRunning ? "Running" : "Stopped")}";

            if (text.Length > 63) text = text[..63];
            trayIcon.Text = text;
        }

        public void ReRegisterHotkey()
        {
            UnregisterHotKey(this.Handle, 0);
            bool success = RegisterHotkey();
            UpdateTrayHotkeyStatus(success);

            if (!success)
            {
                ShowNotification("Warning: Could not register hotkey");
            }

            if (isRunning && timer != null)
            {
                timer.Interval = GetRandomInterval();
            }
            UpdateTrayText();
        }

        private int GetRandomInterval()
        {
            return random.Next(settings.MinInterval, settings.MaxInterval);
        }

        private void ShowNotification(string message)
        {
            if (settings.ShowNotifications && trayIcon != null)
            {
                trayIcon.ShowBalloonTip(3000, "New World AFK Preventer", message, ToolTipIcon.Info);
            }
        }

        private void OnSettings(object? sender, EventArgs e)
        {
            using SettingsForm settingsForm = new(settings);
            if (settingsForm.ShowDialog(this) == DialogResult.OK)
            {
                settings = AppSettings.Load();
                this.TopMost = settings.AlwaysOnTop;
                ReRegisterHotkey();

                if (isRunning && timer != null)
                {
                    timer.Interval = GetRandomInterval();
                }
            }
        }

        private static bool IsNewWorldRunning()
        {
            try
            {
                Debug.WriteLine("Checking for New World process...");

                // First try by process name
                Process[] processes = Process.GetProcessesByName("NewWorld");
                Debug.WriteLine($"Found {processes.Length} processes named 'NewWorld'");

                if (processes.Length == 0)
                {
                    Debug.WriteLine("Searching all processes for New World window...");
                    // Get all processes
                    processes = Process.GetProcesses();
                    Debug.WriteLine($"Found {processes.Length} total processes");

                    // Filter processes with visible windows
                    processes = processes.Where(p =>
                    {
                        try
                        {
                            if (p.HasExited || p.MainWindowHandle == IntPtr.Zero)
                                return false;

                            char[] text = new char[256];
                            int length = GetWindowText(p.MainWindowHandle, text, text.Length);

                            if (length > 0 && IsWindowVisible(p.MainWindowHandle))
                            {
                                string windowTitle = new string(text, 0, length);
                                Debug.WriteLine($"Found window: {windowTitle} (Process: {p.ProcessName})");
                                return windowTitle.Contains("New World", StringComparison.OrdinalIgnoreCase);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error checking process {p.ProcessName}: {ex.Message}");
                        }
                        return false;
                    }).ToArray();

                    Debug.WriteLine($"Found {processes.Length} processes with New World in title");
                }

                // Check each process in detail
                foreach (Process process in processes)
                {
                    try
                    {
                        Debug.WriteLine($"Checking process: {process.ProcessName}");

                        if (process.HasExited)
                        {
                            Debug.WriteLine("Process has exited, skipping");
                            continue;
                        }

                        IntPtr handle = process.MainWindowHandle;
                        if (handle == IntPtr.Zero)
                        {
                            Debug.WriteLine("Process has no main window, skipping");
                            continue;
                        }

                        if (!IsWindowVisible(handle))
                        {
                            Debug.WriteLine("Window is not visible, skipping");
                            continue;
                        }

                        char[] text = new char[256];
                        int length = GetWindowText(handle, text, text.Length);
                        if (length > 0)
                        {
                            string windowTitle = new string(text, 0, length);
                            Debug.WriteLine($"Window title: {windowTitle}");

                            if (windowTitle.Contains("New World", StringComparison.OrdinalIgnoreCase))
                            {
                                Debug.WriteLine("New World found!");
                                return true;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Window has no title");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error checking process: {ex.Message}");
                    }
                }

                Debug.WriteLine("New World not found");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in IsNewWorldRunning: {ex.Message}");
            }
            return false;
        }



        public void TimerTick(object? sender, EventArgs e)
        {
            if (isRunning && IsNewWorldRunning())
            {
                ExecutePowerShellScript("anti-afk.ps1", "-TriggerOnce");
                if (timer != null)
                {
                    timer.Interval = GetRandomInterval();
                }
            }
            else if (isRunning && !IsNewWorldRunning())
            {
                StopAfkPrevention();
            }
        }

        private void StartAfkPrevention()
        {
            if (isRunning) return;

            if (!IsNewWorldRunning())
            {
                ShowNotification("New World is not running!");
                return;
            }

            isRunning = true;
            ShowNotification("AFK Prevention started");
            if (timer != null)
            {
                timer.Interval = GetRandomInterval();
                timer.Start();
            }
            UpdateTrayText();
        }

        private void StopAfkPrevention()
        {
            if (!isRunning) return;

            isRunning = false;
            ShowNotification("AFK Prevention stopped");
            if (timer != null)
            {
                timer.Stop();
            }
            UpdateTrayText();
        }

        private void ExecutePowerShellScript(string scriptName, string arguments)
        {
            try
            {
                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptName);
                if (!File.Exists(scriptPath))
                {
                    Debug.WriteLine($"Script not found at: {scriptPath}");
                    ShowNotification($"Error: {scriptName} not found!");
                    return;
                }

                ProcessStartInfo startInfo = new()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\" {arguments}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using Process process = new() { StartInfo = startInfo };
                process.Start();

                // Asynchronously read the output
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.OutputDataReceived += (sender, args) => { if (args.Data != null) Debug.WriteLine($"PS Output: {args.Data}"); };
                process.ErrorDataReceived += (sender, args) => { if (args.Data != null) Debug.WriteLine($"PS Error: {args.Data}"); };

                // Don't wait for exit in this case, let it run in the background
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing PowerShell script: {ex.Message}");
                ShowNotification("Error running AFK script.");
            }
        }

        private void ToggleAfkPrevention()
        {
            if (isRunning)
                StopAfkPrevention();
            else
                StartAfkPrevention();
        }

        private void UpdateTrayText()
        {
            if (trayIcon != null)
            {
                string status = isRunning ? "Running" : "Stopped";
                trayIcon.Text = $"New World AFK Preventer - {status}";
            }
        }

        private void OnToggleAfkPrevention(object? sender, EventArgs e)
        {
            ToggleAfkPrevention();
        }

        private void OnExit(object? sender, EventArgs e)
        {
            StopAfkPrevention();
            UnregisterHotKey(this.Handle, 0);
            CleanupAndExit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.WriteLine("Disposing AfkPreventer form");
                try
                {
                    if (trayIcon != null)
                    {
                        Debug.WriteLine("Disposing tray icon");
                        trayIcon.Visible = false;
                        trayIcon.Dispose();
                        trayIcon = null;
                    }

                    if (timer != null)
                    {
                        Debug.WriteLine("Disposing timer");
                        timer.Stop();
                        timer.Dispose();
                        timer = null;
                    }

                    if (components != null)
                    {
                        Debug.WriteLine("Disposing components");
                        components.Dispose();
                        components = null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during dispose: {ex.Message}");
                }
            }
            base.Dispose(disposing);
        }

        private void CleanupAndExit()
        {
            try
            {
                Debug.WriteLine("Starting cleanup and exit");
                this.Dispose();
                Application.Exit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during cleanup: {ex.Message}");
                // Force exit if cleanup fails
                Environment.Exit(1);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && (int)m.WParam == 0)
            {
                ToggleAfkPrevention();
                return;
            }
            base.WndProc(ref m);
        }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new AfkPreventer());
        }

        public bool IsRunning { get => isRunning; set => isRunning = value; }
    }
}