using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;

namespace NewWorldAfkPreventer
{
    public class SettingsForm : Form
    {
        private readonly AppSettings settings;
        private TabControl tabControl = null!;
        private TabPage tabHotkey = null!;
        private TabPage tabTiming = null!;
        private TabPage tabGeneral = null!;

        // Hotkey tab controls
        private Label lblCurrentHotkey = null!;
        private Button btnChangeHotkey = null!;
        private Button btnResetHotkey = null!;

        // Timing tab controls
        private Label lblMinInterval = null!;
        private NumericUpDown nudMinInterval = null!;
        private Label lblMaxInterval = null!;
        private NumericUpDown nudMaxInterval = null!;
        private Label lblMinutes1 = null!;
        private Label lblMinutes2 = null!;

        // General tab controls
        private CheckBox chkStartMinimized = null!;
        private CheckBox chkShowNotifications = null!;
        private CheckBox chkAlwaysOnTop = null!;

        // Common controls
        private Button btnSave = null!;
        private TabPage About = null!;
        private RichTextBox richTextBox1 = null!;
        private StatusStrip statusStrip1 = null!;
        private ToolStripStatusLabel toolStripStatusLabel1 = null!;
        private ToolStripStatusLabel toolStripStatusScript = null!;
        private ToolStripStatusLabel toolStripStatusNewWorld = null!;
        private System.Windows.Forms.Timer timer1 = null!;
        private System.ComponentModel.IContainer components = null!;
        private Button button1 = null!;
        private Button btnCancel = null!;

        public SettingsForm(AppSettings settings)
        {
            this.settings = settings;
            InitializeComponent();
            InitializeTimingTabControls();
            SetupHotkeyTab();
            SetupTimingTab();
            SetupGeneralTab();
            LoadSettings();
            timer1.Start();
        }

        private void InitializeTimingTabControls()
        {
            nudMinInterval = new NumericUpDown
            {
                Location = new System.Drawing.Point(200, 28),
                Size = new System.Drawing.Size(80, 20),
                Minimum = 1,
                Maximum = 60,
                Value = 3
            };

            nudMaxInterval = new NumericUpDown
            {
                Location = new System.Drawing.Point(200, 68),
                Size = new System.Drawing.Size(80, 20),
                Minimum = 1,
                Maximum = 120,
                Value = 8
            };
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            tabControl = new TabControl();
            tabHotkey = new TabPage();
            tabTiming = new TabPage();
            tabGeneral = new TabPage();
            About = new TabPage();
            richTextBox1 = new RichTextBox();
            btnSave = new Button();
            btnCancel = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusScript = new ToolStripStatusLabel();
            toolStripStatusNewWorld = new ToolStripStatusLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            button1 = new Button();
            tabControl.SuspendLayout();
            About.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.Controls.Add(tabHotkey);
            tabControl.Controls.Add(tabTiming);
            tabControl.Controls.Add(tabGeneral);
            tabControl.Controls.Add(About);
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(433, 240);
            tabControl.TabIndex = 0;
            // 
            // tabHotkey
            // 
            tabHotkey.BackColor = SystemColors.ActiveCaptionText;
            tabHotkey.Location = new Point(4, 26);
            tabHotkey.Name = "tabHotkey";
            tabHotkey.Size = new Size(425, 210);
            tabHotkey.TabIndex = 0;
            tabHotkey.Text = "Hotkey";
            // 
            // tabTiming
            // 
            tabTiming.BackColor = SystemColors.ActiveCaptionText;
            tabTiming.Location = new Point(4, 26);
            tabTiming.Name = "tabTiming";
            tabTiming.Size = new Size(425, 210);
            tabTiming.TabIndex = 1;
            tabTiming.Text = "Timing";
            // 
            // tabGeneral
            // 
            tabGeneral.BackColor = SystemColors.ActiveCaptionText;
            tabGeneral.Location = new Point(4, 26);
            tabGeneral.Name = "tabGeneral";
            tabGeneral.Size = new Size(425, 210);
            tabGeneral.TabIndex = 2;
            tabGeneral.Text = "General";
            // 
            // About
            // 
            About.AccessibleName = "About";
            About.Controls.Add(richTextBox1);
            About.Location = new Point(4, 26);
            About.Name = "About";
            About.Padding = new Padding(3);
            About.Size = new Size(425, 210);
            About.TabIndex = 3;
            About.Text = "About";
            About.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = SystemColors.ActiveCaptionText;
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.ForeColor = Color.Chartreuse;
            richTextBox1.Location = new Point(3, 3);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(419, 204);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = resources.GetString("richTextBox1.Text", System.Globalization.CultureInfo.CurrentCulture) ?? string.Empty;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Location = new Point(272, 250);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save";
            btnSave.Click += BtnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(352, 250);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.Click += BtnCancel_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = SystemColors.ActiveCaptionText;
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusScript, toolStripStatusNewWorld });
            statusStrip1.Location = new Point(0, 298);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RenderMode = ToolStripRenderMode.Professional;
            statusStrip1.Size = new Size(434, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(42, 17);
            toolStripStatusLabel1.Text = "Status:";
            // 
            // toolStripStatusScript
            // 
            toolStripStatusScript.Name = "toolStripStatusScript";
            toolStripStatusScript.Size = new Size(117, 17);
            toolStripStatusScript.Text = "Script is not running.";
            // 
            // toolStripStatusNewWorld
            // 
            toolStripStatusNewWorld.Name = "toolStripStatusNewWorld";
            toolStripStatusNewWorld.Size = new Size(139, 17);
            toolStripStatusNewWorld.Text = "New World not detected.";
            // 
            // timer1
            // 
            timer1.Interval = 2000;
            timer1.Tick += timer1_Tick;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ActiveCaptionText;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(10, 259);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 4;
            button1.Text = "Exit";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // SettingsForm
            // 
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(434, 320);
            Controls.Add(button1);
            Controls.Add(statusStrip1);
            Controls.Add(tabControl);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Font = new Font("0xProto Nerd Font", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.Chartreuse;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (resources.GetObject("$this.Icon", System.Globalization.CultureInfo.CurrentCulture) as Icon) ?? SystemIcons.Application;
            MaximizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings - New World AFK Preventer";
            tabControl.ResumeLayout(false);
            About.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetupHotkeyTab()
        {
            lblCurrentHotkey = new Label
            {
                Text = "Current hotkey: Ctrl + F12",
                Location = new System.Drawing.Point(20, 30),
                Size = new System.Drawing.Size(300, 20)
            };

            btnChangeHotkey = new Button
            {
                Text = "Change Hotkey",
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(120, 30)
            };
            btnChangeHotkey.Click += BtnChangeHotkey_Click;

            btnResetHotkey = new Button
            {
                Text = "Reset to Default",
                Location = new System.Drawing.Point(150, 70),
                Size = new System.Drawing.Size(180, 30)
            };
            btnResetHotkey.Click += BtnResetHotkey_Click;

            tabHotkey?.Controls.AddRange(new Control[] { lblCurrentHotkey, btnChangeHotkey, btnResetHotkey });
        }

        private void SetupTimingTab()
        {
            lblMinInterval = new Label
            {
                Text = "Minimum interval (minutes):",
                Location = new System.Drawing.Point(20, 30),
                Size = new System.Drawing.Size(180, 20)
            };

            lblMinutes1 = new Label
            {
                Text = "min",
                Location = new System.Drawing.Point(285, 30),
                Size = new System.Drawing.Size(30, 20)
            };

            lblMaxInterval = new Label
            {
                Text = "Maximum interval (minutes):",
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(180, 20)
            };

            lblMinutes2 = new Label
            {
                Text = "min",
                Location = new System.Drawing.Point(285, 70),
                Size = new System.Drawing.Size(30, 20)
            };

            if (lblMinInterval != null && nudMinInterval != null && lblMinutes1 != null &&
                lblMaxInterval != null && nudMaxInterval != null && lblMinutes2 != null)
            {
                tabTiming?.Controls.AddRange(new Control[] {
                    lblMinInterval, nudMinInterval, lblMinutes1,
                    lblMaxInterval, nudMaxInterval, lblMinutes2
                });
            }
        }

        private void SetupGeneralTab()
        {
            chkStartMinimized = new CheckBox
            {
                Text = "Start minimized to tray",
                Location = new System.Drawing.Point(20, 30),
                Size = new System.Drawing.Size(200, 20)
            };

            chkShowNotifications = new CheckBox
            {
                Text = "Show notification balloons",
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(200, 20),
                Checked = true
            };

            chkAlwaysOnTop = new CheckBox
            {
                Text = "Always on Top",
                Location = new System.Drawing.Point(20, 90),
                Size = new System.Drawing.Size(200, 20),
                Checked = settings.AlwaysOnTop
            };
            chkAlwaysOnTop.CheckedChanged += ChkAlwaysOnTop_CheckedChanged;

            tabGeneral?.Controls.AddRange(new Control[] { chkStartMinimized, chkShowNotifications, chkAlwaysOnTop });
        }

        private void ChkAlwaysOnTop_CheckedChanged(object? sender, EventArgs e)
        {
            // Optionale sofortige Anwendung im SettingsForm selbst
            this.TopMost = chkAlwaysOnTop?.Checked ?? false;
        }

        private void LoadSettings()
        {
            if (settings != null)
            {
                // Update existing label instead of recreating it so it stays visible in the tab
                if (lblCurrentHotkey != null)
                {
                    lblCurrentHotkey.Text = $"Current hotkey: {settings.HotkeyModifier} + {settings.Hotkey}";
                }
                else
                {
                    lblCurrentHotkey = new Label
                    {
                        Text = $"Current hotkey: {settings.HotkeyModifier} + {settings.Hotkey}",
                        Location = new System.Drawing.Point(20, 30),
                        Size = new System.Drawing.Size(300, 20)
                    };
                    tabHotkey?.Controls.Add(lblCurrentHotkey);
                }

                // Ensure numeric up/downs have been created
                if (nudMinInterval != null)
                    nudMinInterval.Value = Math.Max(nudMinInterval.Minimum, Math.Min(nudMinInterval.Maximum, settings.MinInterval / 60000)); // Convert to minutes
                if (nudMaxInterval != null)
                    nudMaxInterval.Value = Math.Max(nudMaxInterval.Minimum, Math.Min(nudMaxInterval.Maximum, settings.MaxInterval / 60000)); // Convert to minutes

                if (chkStartMinimized != null) chkStartMinimized.Checked = settings.StartMinimized;
                if (chkShowNotifications != null) chkShowNotifications.Checked = settings.ShowNotifications;
                if (chkAlwaysOnTop != null) chkAlwaysOnTop.Checked = settings.AlwaysOnTop;
                this.TopMost = settings.AlwaysOnTop;
            }
            else
            {
                if (lblCurrentHotkey != null)
                {
                    lblCurrentHotkey.Text = "Current hotkey: Not set";
                }
                else
                {
                    lblCurrentHotkey = new Label
                    {
                        Text = "Current hotkey: Not set",
                        Location = new System.Drawing.Point(20, 30),
                        Size = new System.Drawing.Size(300, 20)
                    };
                    tabHotkey?.Controls.Add(lblCurrentHotkey);
                }

                // Set default values for the controls when settings is null
                if (nudMinInterval != null) nudMinInterval.Value = 1; // Set a default value
                if (nudMaxInterval != null) nudMaxInterval.Value = 10; // Set a default value
                if (chkStartMinimized != null) chkStartMinimized.Checked = false; // Set a default value
                if (chkShowNotifications != null) chkShowNotifications.Checked = false; // Set a default value
                if (chkAlwaysOnTop != null) chkAlwaysOnTop.Checked = false; // Set a default value
                this.TopMost = false;
            }
        }

        private void BtnChangeHotkey_Click(object? sender, EventArgs e)
        {
            using (HotkeyRecorder recorder = new HotkeyRecorder())
            {
                if (recorder.ShowDialog(this) == DialogResult.OK)
                {
                    if (settings != null)
                    {
                        settings.Hotkey = recorder.Hotkey;
                        settings.HotkeyModifier = recorder.Modifier;
                        // Update visible label
                        if (lblCurrentHotkey != null)
                            lblCurrentHotkey.Text = $"Current hotkey: {settings.HotkeyModifier} + {settings.Hotkey}";
                    }
                }
            }
        }

        private void BtnResetHotkey_Click(object? sender, EventArgs e)
        {
            if (settings != null)
            {
                settings.Hotkey = Keys.F12;
                settings.HotkeyModifier = Keys.Control;
                if (lblCurrentHotkey != null)
                    lblCurrentHotkey.Text = $"Current hotkey: {settings.HotkeyModifier} + {settings.Hotkey}";
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Update settings from form
            if (nudMinInterval != null && nudMaxInterval != null && chkStartMinimized != null &&
                chkShowNotifications != null && chkAlwaysOnTop != null)
            {
                settings.MinInterval = (int)nudMinInterval.Value * 60000; // Convert minutes to milliseconds
                settings.MaxInterval = (int)nudMaxInterval.Value * 60000; // Convert minutes to milliseconds
                settings.StartMinimized = chkStartMinimized.Checked;
                settings.ShowNotifications = chkShowNotifications.Checked;
                settings.AlwaysOnTop = chkAlwaysOnTop.Checked;
            }

            // Save settings
            settings.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void timer1_Tick(object? sender, EventArgs e)
        {
            bool newWorldRunning = IsNewWorldRunning();
            if (toolStripStatusNewWorld != null)
                toolStripStatusNewWorld.Text = newWorldRunning ? "New World detected." : "New World not detected.";

            // Find the main AfkPreventer form instance
            AfkPreventer? mainForm = Application.OpenForms.OfType<AfkPreventer>().FirstOrDefault();

            if (toolStripStatusScript != null)
            {
                toolStripStatusScript.Text = (mainForm != null && mainForm.IsRunning && newWorldRunning)
                    ? "Script is running."
                    : "Script is not running.";
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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, char[] lpString, int nMaxCount);

        private void button1_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
