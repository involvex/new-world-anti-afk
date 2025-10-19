using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace NewWorldAfkPreventer
{
    public class AppSettings
    {
        public Keys Hotkey { get; set; } = Keys.F12;
        public Keys HotkeyModifier { get; set; } = Keys.Control;
        public int MinInterval { get; set; } = 180000;
        public int MaxInterval { get; set; } = 480000;
        public bool StartMinimized { get; set; } = true;
        public bool ShowNotifications { get; set; } = true;
        public bool AlwaysOnTop { get; set; } = false;

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
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
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
        private Label lblInstruction;
        private Label lblCurrentKeys;
        private Button btnOk;
        private Button btnCancel;
        private Keys recordedKey = Keys.None;
        private Keys recordedModifier = Keys.None;

        public Keys Hotkey { get; private set; } = Keys.None;
        public Keys Modifier { get; private set; } = Keys.None;
        public bool Success { get; private set; } = false;

        public HotkeyRecorder()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += HotkeyRecorder_KeyDown;
        }

        private void InitializeComponent()
        {
            this.Text = "Record Hotkey";
            this.Size = new System.Drawing.Size(350, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblInstruction = new Label
            {
                Text = "Press the key combination you want to use as hotkey:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(310, 20)
            };

            lblCurrentKeys = new Label
            {
                Text = "Waiting for input...",
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(310, 30),
                Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.Blue
            };

            btnOk = new Button
            {
                Text = "OK",
                Location = new System.Drawing.Point(100, 120),
                Size = new System.Drawing.Size(80, 30),
                Enabled = false
            };
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(190, 120),
                Size = new System.Drawing.Size(80, 30)
            };
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] { lblInstruction, lblCurrentKeys, btnOk, btnCancel });
        }

        private void HotkeyRecorder_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

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

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (recordedKey != Keys.None)
            {
                Hotkey = recordedKey;
                Modifier = recordedModifier;
                Success = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
