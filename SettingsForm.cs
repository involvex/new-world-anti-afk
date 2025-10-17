using System;
using System.Windows.Forms;
using System.Drawing;

namespace NewWorldAfkPreventer
{
    public class SettingsForm : Form
    {
        private AppSettings settings;
        private TabControl tabControl;
        private TabPage tabHotkey;
        private TabPage tabTiming;
        private TabPage tabGeneral;

        // Hotkey tab controls
        private Label lblCurrentHotkey;
        private Button btnChangeHotkey;
        private Button btnResetHotkey;

        // Timing tab controls
        private Label lblMinInterval;
        private NumericUpDown nudMinInterval;
        private Label lblMaxInterval;
        private NumericUpDown nudMaxInterval;
        private Label lblMinutes1;
        private Label lblMinutes2;

        // General tab controls
        private CheckBox chkStartMinimized;
        private CheckBox chkShowNotifications;

        // Common controls
        private Button btnSave;
        private Button btnCancel;

        public SettingsForm(AppSettings settings)
        {
            this.settings = settings;
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Settings - New World AFK Preventer";
            this.Size = new System.Drawing.Size(450, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Tab control
            tabControl = new TabControl
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(415, 260)
            };

            // Hotkey tab
            tabHotkey = new TabPage("Hotkey");
            SetupHotkeyTab();

            // Timing tab
            tabTiming = new TabPage("Timing");
            SetupTimingTab();

            // General tab
            tabGeneral = new TabPage("General");
            SetupGeneralTab();

            tabControl.TabPages.AddRange(new TabPage[] { tabHotkey, tabTiming, tabGeneral });
            this.Controls.Add(tabControl);

            // Buttons
            btnSave = new Button
            {
                Text = "Save",
                Location = new System.Drawing.Point(200, 290),
                Size = new System.Drawing.Size(100, 30),
                BackColor = Color.Green,
                ForeColor = Color.White
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(310, 290),
                Size = new System.Drawing.Size(100, 30)
            };
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private void SetupHotkeyTab()
        {
            lblCurrentHotkey = new Label
            {
                Text = "Current hotkey: Ctrl + F12",
                Location = new System.Drawing.Point(20, 30),
                Size = new System.Drawing.Size(200, 20)
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
                Size = new System.Drawing.Size(120, 30)
            };
            btnResetHotkey.Click += BtnResetHotkey_Click;

            tabHotkey.Controls.AddRange(new Control[] { lblCurrentHotkey, btnChangeHotkey, btnResetHotkey });
        }

        private void SetupTimingTab()
        {
            lblMinInterval = new Label
            {
                Text = "Minimum interval (minutes):",
                Location = new System.Drawing.Point(20, 30),
                Size = new System.Drawing.Size(180, 20)
            };

            nudMinInterval = new NumericUpDown
            {
                Location = new System.Drawing.Point(200, 28),
                Size = new System.Drawing.Size(80, 20),
                Minimum = 1,
                Maximum = 60,
                Value = 3
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

            nudMaxInterval = new NumericUpDown
            {
                Location = new System.Drawing.Point(200, 68),
                Size = new System.Drawing.Size(80, 20),
                Minimum = 1,
                Maximum = 120,
                Value = 8
            };

            lblMinutes2 = new Label
            {
                Text = "min",
                Location = new System.Drawing.Point(285, 70),
                Size = new System.Drawing.Size(30, 20)
            };

            tabTiming.Controls.AddRange(new Control[] {
                lblMinInterval, nudMinInterval, lblMinutes1,
                lblMaxInterval, nudMaxInterval, lblMinutes2
            });
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

            tabGeneral.Controls.AddRange(new Control[] { chkStartMinimized, chkShowNotifications });
        }

        private void LoadSettings()
        {
            lblCurrentHotkey.Text = $"Current hotkey: {settings.HotkeyModifier} + {settings.Hotkey}";
            nudMinInterval.Value = settings.MinInterval / 60000; // Convert to minutes
            nudMaxInterval.Value = settings.MaxInterval / 60000; // Convert to minutes
            chkStartMinimized.Checked = settings.StartMinimized;
            chkShowNotifications.Checked = settings.ShowNotifications;
        }

        private void BtnChangeHotkey_Click(object sender, EventArgs e)
        {
            using (HotkeyRecorder recorder = new HotkeyRecorder())
            {
                if (recorder.ShowDialog() == DialogResult.OK)
                {
                    settings.Hotkey = recorder.Hotkey;
                    settings.HotkeyModifier = recorder.Modifier;
                    lblCurrentHotkey.Text = $"Current hotkey: {settings.HotkeyModifier} + {settings.Hotkey}";
                }
            }
        }

        private void BtnResetHotkey_Click(object sender, EventArgs e)
        {
            settings.Hotkey = Keys.F12;
            settings.HotkeyModifier = Keys.Control;
            lblCurrentHotkey.Text = $"Current hotkey: {settings.HotkeyModifier} + {settings.Hotkey}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Update settings from form
            settings.MinInterval = (int)nudMinInterval.Value * 60000; // Convert minutes to milliseconds
            settings.MaxInterval = (int)nudMaxInterval.Value * 60000; // Convert minutes to milliseconds
            settings.StartMinimized = chkStartMinimized.Checked;
            settings.ShowNotifications = chkShowNotifications.Checked;

            // Save settings
            settings.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
