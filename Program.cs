using System;
using System.Drawing;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BrightnessControl
{
    public partial class BrightnessControlApp : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private TrackBar brightnessTrackBar;
        private Label brightnessLabel;
        private System.Windows.Forms.Timer brightnessTimer;
        private int currentBrightness = 100;
        private const string REGISTRY_KEY = @"SOFTWARE\BrightnessControl";
        private const string BRIGHTNESS_VALUE = "BrightnessLevel";

        public BrightnessControlApp()
        {
            InitializeComponent();
            LoadBrightnessFromRegistry();
            SetupTrayIcon();
            SetupTimer();
            SetBrightness(currentBrightness);

            // Hide the form initially
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form setup
            this.ClientSize = new Size(300, 120);
            this.Text = "Brightness Control";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Icon = CreateBrightnessIcon();

            // Brightness label
            brightnessLabel = new Label
            {
                Text = $"Brightness: {currentBrightness}%",
                Location = new Point(20, 20),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };
            this.Controls.Add(brightnessLabel);

            // Brightness trackbar
            brightnessTrackBar = new TrackBar
            {
                Location = new Point(20, 50),
                Size = new Size(260, 45),
                Minimum = 0,
                Maximum = 100,
                Value = currentBrightness,
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 5
            };
            brightnessTrackBar.Scroll += BrightnessTrackBar_Scroll;
            this.Controls.Add(brightnessTrackBar);

            this.ResumeLayout(false);
        }

        private void SetupTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Show Settings", null, ShowSettings_Click);
            trayMenu.Items.Add("-");
            trayMenu.Items.Add("Exit", null, Exit_Click);

            trayIcon = new NotifyIcon
            {
                Text = "Brightness Control",
                Icon = CreateBrightnessIcon(),
                ContextMenuStrip = trayMenu,
                Visible = true
            };
            trayIcon.DoubleClick += ShowSettings_Click;
        }

        private void SetupTimer()
        {
            brightnessTimer = new System.Windows.Forms.Timer
            {
                Interval = 60000, // 1 minute
                Enabled = true
            };
            brightnessTimer.Tick += BrightnessTimer_Tick;
        }

        private Icon CreateBrightnessIcon()
        {
            // Create a simple brightness icon
            Bitmap bitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                g.FillEllipse(Brushes.Yellow, 2, 2, 12, 12);
                g.DrawEllipse(Pens.Orange, 2, 2, 12, 12);

                // Add some rays
                g.DrawLine(Pens.Orange, 8, 0, 8, 2);
                g.DrawLine(Pens.Orange, 8, 14, 8, 16);
                g.DrawLine(Pens.Orange, 0, 8, 2, 8);
                g.DrawLine(Pens.Orange, 14, 8, 16, 8);
            }
            return Icon.FromHandle(bitmap.GetHicon());
        }

        private void BrightnessTrackBar_Scroll(object sender, EventArgs e)
        {
            currentBrightness = brightnessTrackBar.Value;
            brightnessLabel.Text = $"Brightness: {currentBrightness}%";
            SetBrightness(currentBrightness);
            SaveBrightnessToRegistry();
            UpdateTrayIconTooltip();
        }

        private void BrightnessTimer_Tick(object sender, EventArgs e)
        {
            // Reapply brightness every minute to ensure it stays consistent
            SetBrightness(currentBrightness);
        }

        private void SetBrightness(int brightness)
        {
            try
            {
                ManagementScope scope = new ManagementScope("root\\WMI");
                SelectQuery query = new SelectQuery("WmiMonitorBrightnessMethods");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    using (ManagementObjectCollection objectCollection = searcher.Get())
                    {
                        foreach (ManagementObject mObject in objectCollection)
                        {
                            mObject.InvokeMethod("WmiSetBrightness", new object[] { 1, brightness });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show message box to avoid interrupting user
                System.Diagnostics.Debug.WriteLine($"Error setting brightness: {ex.Message}");
            }
        }

        private void LoadBrightnessFromRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(BRIGHTNESS_VALUE);
                        if (value != null && int.TryParse(value.ToString(), out int savedBrightness))
                        {
                            currentBrightness = Math.Max(0, Math.Min(100, savedBrightness));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading brightness from registry: {ex.Message}");
            }
        }

        private void SaveBrightnessToRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_KEY))
                {
                    key?.SetValue(BRIGHTNESS_VALUE, currentBrightness);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving brightness to registry: {ex.Message}");
            }
        }

        private void UpdateTrayIconTooltip()
        {
            trayIcon.Text = $"Brightness Control - {currentBrightness}%";
        }

        private void ShowSettings_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();

            // Update trackbar to current brightness
            brightnessTrackBar.Value = currentBrightness;
            brightnessLabel.Text = $"Brightness: {currentBrightness}%";
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
            if (!value)
            {
                this.ShowInTaskbar = false;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                trayIcon?.Dispose();
                brightnessTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Ensure only one instance runs
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "BrightnessControlApp", out createdNew))
            {
                if (createdNew)
                {
                    Application.Run(new BrightnessControlApp());
                }
                else
                {
                    MessageBox.Show("Brightness Control is already running.", "Already Running",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
