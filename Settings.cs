using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;

namespace NewWorldAfkPreventer
{
    public class AppSettings
    {
        private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

        public Keys Hotkey { get; set; } = Keys.F12;
        public Keys HotkeyModifier { get; set; } = Keys.Control;
        public int MinInterval { get; set; } = 180000;
        public int MaxInterval { get; set; } = 480000;
        public bool StartMinimized { get; set; } = true;
        public bool ShowNotifications { get; set; } = true;
        public bool AlwaysOnTop { get; set; }

        private static string GetSettingsPath()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NewWorldAfkPreventer");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "settings.json");
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(this, SerializerOptions);
                File.WriteAllText(GetSettingsPath(), json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static AppSettings Load()
        {
            try
            {
                string path = GetSettingsPath();
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load settings: {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return new AppSettings();
        }
    }

    public class HotkeyRecorder : Form
    {
        private readonly Label lblInstruction;
        private readonly Label lblCurrentKeys;
        private readonly Button btnOk;
        private readonly Button btnCancel;
        private Keys recordedKey = Keys.None;
        private Keys recordedModifier = Keys.None;

        public Keys Hotkey { get; private set; } = Keys.None;
        public Keys Modifier { get; private set; } = Keys.None;

        public HotkeyRecorder()
        {
            this.lblInstruction = new Label();
            this.lblCurrentKeys = new Label();
            this.btnOk = new Button();
            this.btnCancel = new Button();
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
            this.FormClosing += HotkeyRecorder_FormClosing; // Add FormClosing event handler
            this.ActiveControl = null; // Ensure form receives key events
            this.TopMost = true; // Ensure the recorder stays on top
            this.Shown += (sender, e) => { this.Focus(); }; // Explicitly set focus when shown
        }

        private void HotkeyRecorder_FormClosing(object? sender, FormClosingEventArgs e)
        {
            Debug.WriteLine($"HotkeyRecorder FormClosing: {e.CloseReason}");
        }

        private void InitializeComponent()
        {
            this.Text = "Record Hotkey";
            this.Size = new System.Drawing.Size(350, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblInstruction.Text = "Press the key combination you want to use as hotkey:";
            lblInstruction.Location = new System.Drawing.Point(20, 20);
            lblInstruction.Size = new System.Drawing.Size(310, 20);

            lblCurrentKeys.Text = "Waiting for input...";
            lblCurrentKeys.Location = new System.Drawing.Point(20, 50);
            lblCurrentKeys.Size = new System.Drawing.Size(310, 30);
            lblCurrentKeys.Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold);
            lblCurrentKeys.ForeColor = System.Drawing.Color.Blue;

            btnOk.Text = "OK";
            btnOk.Location = new System.Drawing.Point(100, 120);
            btnOk.Size = new System.Drawing.Size(80, 30);
            btnOk.Enabled = false;
            btnOk.Click += BtnOk_Click;

            btnCancel.Text = "Cancel";
            btnCancel.Location = new System.Drawing.Point(190, 120);
            btnCancel.Size = new System.Drawing.Size(80, 30);
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] { lblInstruction, lblCurrentKeys, btnOk, btnCancel });
        }

        protected void OnKeyDown(object? sender, KeyEventArgs e)
        {
            // e.Handled = true; // Removed to allow propagation

            // Check for modifier keys
            Keys modifiers = Keys.None;
            if (e.Control) modifiers |= Keys.Control;
            if (e.Alt) modifiers |= Keys.Alt;
            if (e.Shift) modifiers |= Keys.Shift;

            Keys key = e.KeyCode;

            // Don't allow modifier keys by themselves
            if (key == Keys.ControlKey || key == Keys.Menu || key == Keys.ShiftKey ||
                key == Keys.LControlKey || key == Keys.RControlKey ||
                key == Keys.LMenu || key == Keys.RMenu ||
                key == Keys.LShiftKey || key == Keys.RShiftKey)
            {
                return;
            }

            recordedKey = key;
            recordedModifier = modifiers;

            string keyText = "";
            if (modifiers != Keys.None)
            {
                if ((modifiers & Keys.Control) == Keys.Control) keyText += "Ctrl + ";
                if ((modifiers & Keys.Alt) == Keys.Alt) keyText += "Alt + ";
                if ((modifiers & Keys.Shift) == Keys.Shift) keyText += "Shift + ";
            }
            keyText += key.ToString();

            lblCurrentKeys.Text = keyText;
            btnOk.Enabled = true;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (recordedKey != Keys.None)
            {
                Hotkey = recordedKey;
                Modifier = recordedModifier;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
