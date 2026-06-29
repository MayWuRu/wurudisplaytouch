using System;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace WuRuDisplayTouch
{
    public static class Loc
    {
        public static string Lang { get; set; }

        static Loc()
        {
            Lang = "EN";
        }

        private static Dictionary<string, string[]> dict = new Dictionary<string, string[]>()
        {
            { "SettingsTitle", new[] { "WuRuDisplayTouch Settings", "ตั้งค่า WuRuDisplayTouch" } },
            { "RunOnStartup", new[] { "Run on Startup", "เปิดโปรแกรมพร้อม Windows" } },
            { "TouchAutoMode", new[] { "Touchscreen Auto Mode", "เปิด/ปิด Touchscreen อัตโนมัติ" } },
            { "GlobalEnforcer", new[] { "Global Display Enforcer", "ระบบปรับตั้งค่าหน้าจออัตโนมัติ" } },
            { "PerMonitorSettings", new[] { "Per-Monitor Settings", "การตั้งค่าแยกตามจอภาพ" } },
            { "SelectMonitor", new[] { "Select Monitor:", "เลือกหน้าจอ:" } },
            { "EnableForThisMonitor", new[] { "Enable Display Enforcer for this monitor", "เปิดใช้งานบังคับตั้งค่าเฉพาะหน้าจอนี้" } },
            { "Resolution", new[] { "Resolution:", "ความละเอียด:" } },
            { "RefreshRate", new[] { "Refresh Rate:", "อัตรารีเฟรช:" } },
            { "Save", new[] { "Save", "บันทึก" } },
            { "Cancel", new[] { "Cancel", "ยกเลิก" } },
            { "AutoRes", new[] { "Auto (Max supported)", "Auto (ดึงค่าสูงสุดที่จอรับได้)" } },
            { "AutoRef", new[] { "Auto (Max supported)", "Auto (สูงสุดที่รองรับ)" } },
            { "NoMonitors", new[] { "No Monitors Found", "ไม่พบหน้าจอใดๆ" } },
            { "Internal", new[] { "(Internal)", "(จอติดเครื่อง)" } },
            { "External", new[] { "(External)", "(จอนอก)" } },
            { "ForceEnable", new[] { "Force Enable Touchscreen", "เปิด Touchscreen" } },
            { "ForceDisable", new[] { "Force Disable Touchscreen", "ปิด Touchscreen" } },
            { "Settings", new[] { "Settings...", "การตั้งค่า..." } },
            { "Exit", new[] { "Exit", "ออก" } },
            { "TouchEnabled", new[] { "Touchscreen Enabled (Internal display active)", "เปิด Touchscreen (จอหลักทำงาน)" } },
            { "TouchDisabled", new[] { "Touchscreen Disabled (External display active)", "ปิด Touchscreen (ต่อจอนอก)" } },
            { "Note", new[] { "*Note: If Auto mode causes messy scaling (especially on 2K monitors),\n please manually lock the resolution to match your physical monitor.", "*หมายเหตุ: หากใช้ Auto แล้วสเกลภาพเพี้ยน (โดยเฉพาะจอ 2K)\n แนะนำให้เลือกล็อคความละเอียดให้ตรงกับสเปคจอเอง" } },
            { "Language", new[] { "Language / ภาษา:", "Language / ภาษา:" } }
        };

        public static string Get(string key)
        {
            if (dict.ContainsKey(key))
            {
                return Lang == "TH" ? dict[key][1] : dict[key][0];
            }
            return key;
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Loc.Lang = Settings.Language;
            Application.Run(new TrayApplicationContext());
        }
    }

    public static class Settings
    {
        private const string GlobalRegistryKeyPath = @"Software\WuRuDisplayTouch";
        private const string BaseRegistryKeyPath = @"Software\WuRuDisplayTouch\Displays";

        public static string Language
        {
            get { return GetGlobalRegistryStringValue("Language", "EN"); }
            set { SetGlobalRegistryStringValue("Language", value); }
        }

        public static bool TouchAutoMode
        {
            get { return GetGlobalRegistryValue("TouchAutoMode", 1) == 1; }
            set { SetGlobalRegistryValue("TouchAutoMode", value ? 1 : 0); }
        }

        public static bool EnableGlobalDisplayEnforcer
        {
            get { return GetGlobalRegistryValue("EnableGlobalDisplayEnforcer", 1) == 1; }
            set { SetGlobalRegistryValue("EnableGlobalDisplayEnforcer", value ? 1 : 0); }
        }

        private static int GetGlobalRegistryValue(string name, int defaultValue)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(GlobalRegistryKeyPath))
                {
                    if (key != null)
                    {
                        object val = key.GetValue(name);
                        if (val != null) return (int)val;
                    }
                }
            }
            catch { }
            return defaultValue;
        }

        private static void SetGlobalRegistryValue(string name, int value)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(GlobalRegistryKeyPath))
                {
                    key.SetValue(name, value, RegistryValueKind.DWord);
                }
            }
            catch { }
        }

        private static string GetGlobalRegistryStringValue(string name, string defaultValue)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(GlobalRegistryKeyPath))
                {
                    if (key != null)
                    {
                        object val = key.GetValue(name);
                        if (val != null) return val.ToString();
                    }
                }
            }
            catch { }
            return defaultValue;
        }

        private static void SetGlobalRegistryStringValue(string name, string value)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(GlobalRegistryKeyPath))
                {
                    key.SetValue(name, value, RegistryValueKind.String);
                }
            }
            catch { }
        }

        public static bool GetEnable(string monitorId)
        {
            return GetRegistryValue(monitorId, "EnableDisplayEnforcer", 0) == 1;
        }

        public static void SetEnable(string monitorId, bool value)
        {
            SetRegistryValue(monitorId, "EnableDisplayEnforcer", value ? 1 : 0);
        }

        public static int GetWidth(string monitorId)
        {
            return GetRegistryValue(monitorId, "TargetWidth", 0);
        }

        public static void SetWidth(string monitorId, int value)
        {
            SetRegistryValue(monitorId, "TargetWidth", value);
        }

        public static int GetHeight(string monitorId)
        {
            return GetRegistryValue(monitorId, "TargetHeight", 0);
        }

        public static void SetHeight(string monitorId, int value)
        {
            SetRegistryValue(monitorId, "TargetHeight", value);
        }

        public static int GetRefreshRate(string monitorId)
        {
            return GetRegistryValue(monitorId, "TargetRefreshRate", 0);
        }

        public static void SetRefreshRate(string monitorId, int value)
        {
            SetRegistryValue(monitorId, "TargetRefreshRate", value);
        }

        private static string GetSafeMonitorId(string monitorId)
        {
            if (string.IsNullOrEmpty(monitorId)) return "Unknown";
            return monitorId.Replace("\\", "_").Replace("/", "_");
        }

        private static int GetRegistryValue(string monitorId, string name, int defaultValue)
        {
            try
            {
                string path = BaseRegistryKeyPath + "\\" + GetSafeMonitorId(monitorId);
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        object val = key.GetValue(name);
                        if (val != null) return (int)val;
                    }
                }
            }
            catch { }
            return defaultValue;
        }

        private static void SetRegistryValue(string monitorId, string name, int value)
        {
            try
            {
                string path = BaseRegistryKeyPath + "\\" + GetSafeMonitorId(monitorId);
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(path))
                {
                    key.SetValue(name, value, RegistryValueKind.DWord);
                }
            }
            catch { }
        }
    }

    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int ENUM_REGISTRY_SETTINGS = -2;
        public const uint CDS_UPDATEREGISTRY = 0x01;
        public const int DISP_CHANGE_SUCCESSFUL = 0;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceString;
            public uint StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceKey;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
    }

    public class MonitorInfo
    {
        public string AdapterName { get; set; }
        public string DeviceID { get; set; }
        public string DisplayName { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsInternal { get; set; }
        
        public string UIString 
        {
            get 
            {
                string type = IsInternal ? Loc.Get("Internal") : Loc.Get("External");
                string niceAdapter = AdapterName.Replace("\\\\.\\", "");
                return string.Format("{0} {1} - {2}", niceAdapter, type, DisplayName);
            }
        }

        public override string ToString()
        {
            return UIString;
        }
    }

    public class ResolutionItem
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsAuto { get; set; }

        public override string ToString()
        {
            if (IsAuto) return Loc.Get("AutoRes");
            return string.Format("{0} x {1}", Width, Height);
        }

        public override bool Equals(object obj)
        {
            var item = obj as ResolutionItem;
            if (item == null) return false;
            return Width == item.Width && Height == item.Height && IsAuto == item.IsAuto;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode() ^ IsAuto.GetHashCode();
        }
    }

    public class RefreshRateItem
    {
        public int RefreshRate { get; set; }
        public bool IsAuto { get; set; }

        public override string ToString()
        {
            if (IsAuto) return Loc.Get("AutoRef");
            return string.Format("{0} Hz", RefreshRate);
        }
        
        public override bool Equals(object obj)
        {
            var item = obj as RefreshRateItem;
            if (item == null) return false;
            return RefreshRate == item.RefreshRate && IsAuto == item.IsAuto;
        }

        public override int GetHashCode()
        {
            return RefreshRate.GetHashCode() ^ IsAuto.GetHashCode();
        }
    }

    public class MonitorSettings
    {
        public bool Enabled;
        public int Width;
        public int Height;
        public int RefreshRate;
    }

    public class SettingsForm : Form
    {
        private Label lblLang;
        private ComboBox cmbLang;
        private CheckBox chkRunStartup;
        private CheckBox chkTouchAuto;
        private CheckBox chkGlobalEnforcer;
        private GroupBox grpMonitor;
        private Label lblMonitor;
        private ComboBox cmbMonitors;
        private CheckBox chkEnableEnforcer;
        private Label lblRes;
        private ComboBox cmbResolution;
        private Label lblRefresh;
        private ComboBox cmbRefreshRate;
        private Label lblNote;
        private Button btnSave;
        private Button btnCancel;
        
        private List<MonitorInfo> monitors;
        private Dictionary<string, MonitorSettings> pendingSettings = new Dictionary<string, MonitorSettings>();
        
        private bool isUpdatingUI = false;
        private string taskName = "WuRuDisplayTouch_Startup";

        public SettingsForm(List<MonitorInfo> allMonitors)
        {
            this.monitors = allMonitors;
            InitializeComponent();
            UpdateTexts();
            LoadInitialSettings();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(420, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            int yPos = 15;

            lblLang = new Label();
            lblLang.AutoSize = true;
            lblLang.Location = new Point(20, yPos + 3);
            this.Controls.Add(lblLang);

            cmbLang = new ComboBox();
            cmbLang.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLang.Items.Add("English (EN)");
            cmbLang.Items.Add("ภาษาไทย (TH)");
            cmbLang.SelectedIndex = (Loc.Lang == "TH") ? 1 : 0;
            cmbLang.Location = new Point(140, yPos);
            cmbLang.Width = 150;
            cmbLang.SelectedIndexChanged += CmbLang_SelectedIndexChanged;
            this.Controls.Add(cmbLang);

            yPos += 35;
            chkRunStartup = new CheckBox();
            chkRunStartup.AutoSize = true;
            chkRunStartup.Location = new Point(20, yPos);
            this.Controls.Add(chkRunStartup);

            yPos += 30;
            chkTouchAuto = new CheckBox();
            chkTouchAuto.AutoSize = true;
            chkTouchAuto.Location = new Point(20, yPos);
            this.Controls.Add(chkTouchAuto);

            yPos += 30;
            chkGlobalEnforcer = new CheckBox();
            chkGlobalEnforcer.AutoSize = true;
            chkGlobalEnforcer.Location = new Point(20, yPos);
            chkGlobalEnforcer.CheckedChanged += (s, e) => { grpMonitor.Enabled = chkGlobalEnforcer.Checked; };
            this.Controls.Add(chkGlobalEnforcer);

            yPos += 35;
            grpMonitor = new GroupBox();
            grpMonitor.Size = new Size(365, 260);
            grpMonitor.Location = new Point(20, yPos);
            this.Controls.Add(grpMonitor);

            int gyPos = 25;
            lblMonitor = new Label();
            lblMonitor.AutoSize = true;
            lblMonitor.Location = new Point(15, gyPos);
            grpMonitor.Controls.Add(lblMonitor);

            gyPos += 20;
            cmbMonitors = new ComboBox();
            cmbMonitors.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMonitors.Location = new Point(15, gyPos);
            cmbMonitors.Width = 335;
            cmbMonitors.SelectedIndexChanged += CmbMonitors_SelectedIndexChanged;
            grpMonitor.Controls.Add(cmbMonitors);

            gyPos += 35;
            chkEnableEnforcer = new CheckBox();
            chkEnableEnforcer.AutoSize = true;
            chkEnableEnforcer.Location = new Point(15, gyPos);
            chkEnableEnforcer.CheckedChanged += (s, e) => { 
                cmbResolution.Enabled = chkEnableEnforcer.Checked;
                cmbRefreshRate.Enabled = chkEnableEnforcer.Checked;
                SaveCurrentMonitorToPending();
            };
            grpMonitor.Controls.Add(chkEnableEnforcer);

            gyPos += 35;
            lblRes = new Label();
            lblRes.AutoSize = true;
            lblRes.Location = new Point(15, gyPos);
            grpMonitor.Controls.Add(lblRes);

            gyPos += 20;
            cmbResolution = new ComboBox();
            cmbResolution.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbResolution.Location = new Point(15, gyPos);
            cmbResolution.Width = 335;
            cmbResolution.SelectedIndexChanged += CmbResolution_SelectedIndexChanged;
            grpMonitor.Controls.Add(cmbResolution);

            gyPos += 35;
            lblRefresh = new Label();
            lblRefresh.AutoSize = true;
            lblRefresh.Location = new Point(15, gyPos);
            grpMonitor.Controls.Add(lblRefresh);

            gyPos += 20;
            cmbRefreshRate = new ComboBox();
            cmbRefreshRate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRefreshRate.Location = new Point(15, gyPos);
            cmbRefreshRate.Width = 335;
            cmbRefreshRate.SelectedIndexChanged += (s, e) => SaveCurrentMonitorToPending();
            grpMonitor.Controls.Add(cmbRefreshRate);

            gyPos += 28;
            lblNote = new Label();
            lblNote.AutoSize = true;
            lblNote.ForeColor = Color.DimGray;
            lblNote.Location = new Point(15, gyPos);
            grpMonitor.Controls.Add(lblNote);

            btnSave = new Button();
            btnSave.Location = new Point(200, yPos + 275);
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button();
            btnCancel.Location = new Point(290, yPos + 275);
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void CmbLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            Loc.Lang = cmbLang.SelectedIndex == 1 ? "TH" : "EN";
            Settings.Language = Loc.Lang;
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            this.Text = Loc.Get("SettingsTitle");
            lblLang.Text = Loc.Get("Language");
            chkRunStartup.Text = Loc.Get("RunOnStartup");
            chkTouchAuto.Text = Loc.Get("TouchAutoMode");
            chkGlobalEnforcer.Text = Loc.Get("GlobalEnforcer");
            grpMonitor.Text = Loc.Get("PerMonitorSettings");
            lblMonitor.Text = Loc.Get("SelectMonitor");
            chkEnableEnforcer.Text = Loc.Get("EnableForThisMonitor");
            lblRes.Text = Loc.Get("Resolution");
            lblRefresh.Text = Loc.Get("RefreshRate");
            lblNote.Text = Loc.Get("Note");
            btnSave.Text = Loc.Get("Save");
            btnCancel.Text = Loc.Get("Cancel");

            if (cmbMonitors.Items.Count > 0 && cmbMonitors.Items[0] is string)
            {
                cmbMonitors.Items[0] = Loc.Get("NoMonitors");
            }
            else if (cmbMonitors.Items.Count > 0)
            {
                int selectedIdx = cmbMonitors.SelectedIndex;
                var items = new List<MonitorInfo>();
                foreach (var item in cmbMonitors.Items) items.Add((MonitorInfo)item);
                cmbMonitors.Items.Clear();
                foreach (var m in items) cmbMonitors.Items.Add(m);
                cmbMonitors.SelectedIndex = selectedIdx;
            }
            
            if (cmbResolution.Items.Count > 0) {
                int selectedIdx = cmbResolution.SelectedIndex;
                var items = new List<ResolutionItem>();
                foreach (var item in cmbResolution.Items) items.Add((ResolutionItem)item);
                cmbResolution.Items.Clear();
                foreach (var m in items) cmbResolution.Items.Add(m);
                cmbResolution.SelectedIndex = selectedIdx;
            }

            if (cmbRefreshRate.Items.Count > 0) {
                int selectedIdx = cmbRefreshRate.SelectedIndex;
                var items = new List<RefreshRateItem>();
                foreach (var item in cmbRefreshRate.Items) items.Add((RefreshRateItem)item);
                cmbRefreshRate.Items.Clear();
                foreach (var m in items) cmbRefreshRate.Items.Add(m);
                cmbRefreshRate.SelectedIndex = selectedIdx;
            }
        }

        private bool CheckIfStartupTaskExists()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("schtasks", string.Format("/query /tn \"{0}\"", taskName))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                var p = Process.Start(psi);
                if (p != null) p.WaitForExit();
                return p.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private void ToggleStartupTask(bool enable)
        {
            try
            {
                bool exists = CheckIfStartupTaskExists();
                if (enable && !exists)
                {
                    string exePath = Application.ExecutablePath;
                    ProcessStartInfo psi = new ProcessStartInfo("schtasks", string.Format("/create /tn \"{0}\" /tr \"\\\"{1}\\\"\" /sc onlogon /rl highest /f", taskName, exePath))
                    { CreateNoWindow = true, UseShellExecute = false };
                    var p = Process.Start(psi);
                    if (p != null) p.WaitForExit();
                }
                else if (!enable && exists)
                {
                    ProcessStartInfo psi = new ProcessStartInfo("schtasks", string.Format("/delete /tn \"{0}\" /f", taskName))
                    { CreateNoWindow = true, UseShellExecute = false };
                    var p = Process.Start(psi);
                    if (p != null) p.WaitForExit();
                }
            }
            catch { }
        }

        private void LoadInitialSettings()
        {
            chkRunStartup.Checked = CheckIfStartupTaskExists();
            chkTouchAuto.Checked = Settings.TouchAutoMode;
            chkGlobalEnforcer.Checked = Settings.EnableGlobalDisplayEnforcer;
            grpMonitor.Enabled = chkGlobalEnforcer.Checked;

            if (monitors == null || monitors.Count == 0)
            {
                cmbMonitors.Items.Add(Loc.Get("NoMonitors"));
                cmbMonitors.SelectedIndex = 0;
                cmbMonitors.Enabled = false;
                chkEnableEnforcer.Enabled = false;
                cmbResolution.Enabled = false;
                cmbRefreshRate.Enabled = false;
                return;
            }

            foreach (var m in monitors)
            {
                pendingSettings[m.DeviceID] = new MonitorSettings
                {
                    Enabled = Settings.GetEnable(m.DeviceID),
                    Width = Settings.GetWidth(m.DeviceID),
                    Height = Settings.GetHeight(m.DeviceID),
                    RefreshRate = Settings.GetRefreshRate(m.DeviceID)
                };
                cmbMonitors.Items.Add(m);
            }

            cmbMonitors.SelectedIndex = 0; 
        }

        private void CmbMonitors_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUIForSelectedMonitor();
        }

        private void UpdateUIForSelectedMonitor()
        {
            MonitorInfo mInfo = cmbMonitors.SelectedItem as MonitorInfo;
            if (mInfo == null) return;

            isUpdatingUI = true;

            var currentSettings = pendingSettings[mInfo.DeviceID];
            chkEnableEnforcer.Checked = currentSettings.Enabled;
            
            cmbResolution.Enabled = currentSettings.Enabled;
            cmbRefreshRate.Enabled = currentSettings.Enabled;

            cmbResolution.Items.Clear();
            cmbRefreshRate.Items.Clear();

            var resolutions = new List<ResolutionItem>();
            resolutions.Add(new ResolutionItem { IsAuto = true });

            NativeMethods.DEVMODE vDevMode = new NativeMethods.DEVMODE();
            vDevMode.dmSize = (short)Marshal.SizeOf(vDevMode);
            int i = 0;
            while (NativeMethods.EnumDisplaySettings(mInfo.AdapterName, i, ref vDevMode))
            {
                if (vDevMode.dmBitsPerPel >= 32)
                {
                    var res = new ResolutionItem { Width = vDevMode.dmPelsWidth, Height = vDevMode.dmPelsHeight, IsAuto = false };
                    if (!resolutions.Contains(res))
                    {
                        resolutions.Add(res);
                    }
                }
                i++;
            }

            resolutions.Sort((a, b) =>
            {
                if (a.IsAuto) return -1;
                if (b.IsAuto) return 1;
                if (a.Width != b.Width) return b.Width.CompareTo(a.Width);
                return b.Height.CompareTo(a.Height);
            });

            foreach (var r in resolutions)
            {
                cmbResolution.Items.Add(r);
            }

            int resIndex = 0;
            if (currentSettings.Width > 0 && currentSettings.Height > 0)
            {
                for (int j = 0; j < resolutions.Count; j++)
                {
                    if (resolutions[j].Width == currentSettings.Width && resolutions[j].Height == currentSettings.Height)
                    {
                        resIndex = j;
                        break;
                    }
                }
            }
            cmbResolution.SelectedIndex = resIndex;

            isUpdatingUI = false;
            UpdateRefreshRateDropdown();
        }

        private void CmbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingUI) return;
            UpdateRefreshRateDropdown();
            SaveCurrentMonitorToPending();
        }

        private void UpdateRefreshRateDropdown()
        {
            isUpdatingUI = true;

            MonitorInfo mInfo = cmbMonitors.SelectedItem as MonitorInfo;
            ResolutionItem selectedRes = cmbResolution.SelectedItem as ResolutionItem;

            if (mInfo == null || selectedRes == null)
            {
                isUpdatingUI = false;
                return;
            }

            var currentSettings = pendingSettings[mInfo.DeviceID];
            
            cmbRefreshRate.Items.Clear();
            var refreshRates = new List<RefreshRateItem>();
            refreshRates.Add(new RefreshRateItem { IsAuto = true });

            NativeMethods.DEVMODE vDevMode = new NativeMethods.DEVMODE();
            vDevMode.dmSize = (short)Marshal.SizeOf(vDevMode);
            int i = 0;
            
            int filterW = selectedRes.IsAuto ? -1 : selectedRes.Width;
            int filterH = selectedRes.IsAuto ? -1 : selectedRes.Height;

            while (NativeMethods.EnumDisplaySettings(mInfo.AdapterName, i, ref vDevMode))
            {
                if (vDevMode.dmBitsPerPel >= 32)
                {
                    bool match = false;
                    if (filterW == -1) 
                    {
                        match = true; 
                    }
                    else if (vDevMode.dmPelsWidth == filterW && vDevMode.dmPelsHeight == filterH)
                    {
                        match = true;
                    }

                    if (match)
                    {
                        var rr = new RefreshRateItem { RefreshRate = vDevMode.dmDisplayFrequency, IsAuto = false };
                        if (!refreshRates.Contains(rr))
                        {
                            refreshRates.Add(rr);
                        }
                    }
                }
                i++;
            }

            refreshRates.Sort((a, b) =>
            {
                if (a.IsAuto) return -1;
                if (b.IsAuto) return 1;
                return b.RefreshRate.CompareTo(a.RefreshRate);
            });

            foreach (var rr in refreshRates)
            {
                cmbRefreshRate.Items.Add(rr);
            }

            int rrIndex = 0;
            if (currentSettings.RefreshRate > 0)
            {
                for (int j = 0; j < refreshRates.Count; j++)
                {
                    if (refreshRates[j].RefreshRate == currentSettings.RefreshRate)
                    {
                        rrIndex = j;
                        break;
                    }
                }
            }
            cmbRefreshRate.SelectedIndex = rrIndex;

            isUpdatingUI = false;
        }

        private void SaveCurrentMonitorToPending()
        {
            if (isUpdatingUI) return;
            
            MonitorInfo mInfo = cmbMonitors.SelectedItem as MonitorInfo;
            if (mInfo == null) return;

            var settings = pendingSettings[mInfo.DeviceID];
            settings.Enabled = chkEnableEnforcer.Checked;
            
            ResolutionItem res = cmbResolution.SelectedItem as ResolutionItem;
            if (res != null)
            {
                settings.Width = res.IsAuto ? 0 : res.Width;
                settings.Height = res.IsAuto ? 0 : res.Height;
            }

            RefreshRateItem rr = cmbRefreshRate.SelectedItem as RefreshRateItem;
            if (rr != null)
            {
                settings.RefreshRate = rr.IsAuto ? 0 : rr.RefreshRate;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentMonitorToPending();
            
            ToggleStartupTask(chkRunStartup.Checked);
            Settings.TouchAutoMode = chkTouchAuto.Checked;
            Settings.EnableGlobalDisplayEnforcer = chkGlobalEnforcer.Checked;

            foreach (var kvp in pendingSettings)
            {
                string monitorId = kvp.Key;
                MonitorSettings s = kvp.Value;
                Settings.SetEnable(monitorId, s.Enabled);
                Settings.SetWidth(monitorId, s.Width);
                Settings.SetHeight(monitorId, s.Height);
                Settings.SetRefreshRate(monitorId, s.RefreshRate);
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private MenuItem miEnable;
        private MenuItem miDisable;
        private MenuItem miSettings;
        private MenuItem miExit;

        public TrayApplicationContext()
        {
            Icon appIcon;
            try
            {
                appIcon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("WuRuDisplayTouch.icon.ico"));
            }
            catch
            {
                appIcon = SystemIcons.Shield; 
            }

            miEnable = new MenuItem(Loc.Get("ForceEnable"), EnableTouch);
            miDisable = new MenuItem(Loc.Get("ForceDisable"), DisableTouch);
            miSettings = new MenuItem(Loc.Get("Settings"), OpenSettings);
            miExit = new MenuItem(Loc.Get("Exit"), Exit);

            trayIcon = new NotifyIcon()
            {
                Icon = appIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    miEnable,
                    miDisable,
                    new MenuItem("-"),
                    miSettings,
                    new MenuItem("-"),
                    miExit
                }),
                Visible = true,
                Text = "WuRuDisplayTouch"
            };
            
            trayIcon.DoubleClick += OpenSettings;

            SystemEvents.DisplaySettingsChanged += DisplaySettingsChanged;
            
            CheckDisplayAndToggleTouch();
        }

        public void UpdateLanguage()
        {
            miEnable.Text = Loc.Get("ForceEnable");
            miDisable.Text = Loc.Get("ForceDisable");
            miSettings.Text = Loc.Get("Settings");
            miExit.Text = Loc.Get("Exit");
            trayIcon.Text = Loc.Get("SettingsTitle");
        }

        private void OpenSettings(object sender, EventArgs e)
        {
            List<MonitorInfo> allMonitors = GetAllMonitors();
            using (SettingsForm sf = new SettingsForm(allMonitors))
            {
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    UpdateLanguage();
                    CheckDisplayAndToggleTouch();
                }
            }
        }

        private void DisplaySettingsChanged(object sender, EventArgs e)
        {
            CheckDisplayAndToggleTouch();
        }

        private List<MonitorInfo> GetAllMonitors()
        {
            List<MonitorInfo> monitors = new List<MonitorInfo>();
            try
            {
                Dictionary<string, bool> wmiInternalMap = new Dictionary<string, bool>();
                try 
                {
                    var searcher = new ManagementObjectSearcher("root\\wmi", "SELECT * FROM WmiMonitorConnectionParams");
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        string instanceName = queryObj["InstanceName"] != null ? queryObj["InstanceName"].ToString() : "";
                        uint tech = 0;
                        try { tech = Convert.ToUInt32(queryObj["VideoOutputTechnology"]); } catch { }
                        bool isInternal = (tech == 11 || tech == 7 || tech == 2147483648);
                        
                        string[] parts = instanceName.Split('\\');
                        if (parts.Length > 1) 
                        {
                            wmiInternalMap[parts[1]] = isInternal; 
                        }
                    }
                } catch { }

                uint devNum = 0;
                NativeMethods.DISPLAY_DEVICE d = new NativeMethods.DISPLAY_DEVICE();
                d.cb = Marshal.SizeOf(d);

                while (NativeMethods.EnumDisplayDevices(null, devNum, ref d, 0))
                {
                    if ((d.StateFlags & 0x1) == 0x1) 
                    {
                        bool isPrimary = (d.StateFlags & 0x4) == 0x4; 
                        
                        NativeMethods.DISPLAY_DEVICE mon = new NativeMethods.DISPLAY_DEVICE();
                        mon.cb = Marshal.SizeOf(mon);
                        if (NativeMethods.EnumDisplayDevices(d.DeviceName, 0, ref mon, 0))
                        {
                            bool isInternal = false;
                            string hwId = "";
                            string[] parts = mon.DeviceID.Split('\\');
                            if (parts.Length > 1) hwId = parts[1];

                            if (wmiInternalMap.ContainsKey(hwId))
                            {
                                isInternal = wmiInternalMap[hwId];
                            }
                            else
                            {
                                isInternal = isPrimary; 
                            }

                            monitors.Add(new MonitorInfo
                            {
                                AdapterName = d.DeviceName,
                                DeviceID = mon.DeviceID,
                                DisplayName = mon.DeviceString,
                                IsPrimary = isPrimary,
                                IsInternal = isInternal
                            });
                        }
                        else 
                        {
                            monitors.Add(new MonitorInfo
                            {
                                AdapterName = d.DeviceName,
                                DeviceID = d.DeviceName, 
                                DisplayName = d.DeviceString,
                                IsPrimary = isPrimary,
                                IsInternal = isPrimary
                            });
                        }
                    }
                    devNum++;
                }
            }
            catch { }

            return monitors;
        }

        private void CheckDisplayAndToggleTouch()
        {
            List<MonitorInfo> activeMonitors = GetAllMonitors();

            if (Settings.TouchAutoMode)
            {
                bool internalDisplayActive = activeMonitors.Any(m => m.IsInternal);
                SetTouchscreenState(internalDisplayActive);
            }

            if (Settings.EnableGlobalDisplayEnforcer)
            {
                foreach (var monitor in activeMonitors)
                {
                    if (Settings.GetEnable(monitor.DeviceID))
                    {
                        EnforceExternalDisplaySettings(monitor);
                    }
                }
            }
        }

        private void EnforceExternalDisplaySettings(MonitorInfo monitor)
        {
            int targetW = Settings.GetWidth(monitor.DeviceID);
            int targetH = Settings.GetHeight(monitor.DeviceID);
            int targetR = Settings.GetRefreshRate(monitor.DeviceID);

            if (targetW == 0 || targetH == 0)
            {
                NativeMethods.DEVMODE vDevMode = new NativeMethods.DEVMODE();
                vDevMode.dmSize = (short)Marshal.SizeOf(vDevMode);
                int i = 0;
                int maxW = 0;
                int maxH = 0;
                while (NativeMethods.EnumDisplaySettings(monitor.AdapterName, i, ref vDevMode))
                {
                    if (vDevMode.dmBitsPerPel >= 32)
                    {
                        if (vDevMode.dmPelsWidth > maxW || (vDevMode.dmPelsWidth == maxW && vDevMode.dmPelsHeight > maxH))
                        {
                            maxW = vDevMode.dmPelsWidth;
                            maxH = vDevMode.dmPelsHeight;
                        }
                    }
                    i++;
                }
                if (maxW > 0 && maxH > 0)
                {
                    targetW = maxW;
                    targetH = maxH;
                }
            }

            if (targetR == 0)
            {
                NativeMethods.DEVMODE vDevMode = new NativeMethods.DEVMODE();
                vDevMode.dmSize = (short)Marshal.SizeOf(vDevMode);
                int i = 0;
                int maxR = 0;
                while (NativeMethods.EnumDisplaySettings(monitor.AdapterName, i, ref vDevMode))
                {
                    if (vDevMode.dmBitsPerPel >= 32 && vDevMode.dmPelsWidth == targetW && vDevMode.dmPelsHeight == targetH)
                    {
                        if (vDevMode.dmDisplayFrequency > maxR)
                        {
                            maxR = vDevMode.dmDisplayFrequency;
                        }
                    }
                    i++;
                }
                if (maxR > 0)
                {
                    targetR = maxR;
                }
            }

            if (targetW <= 0 || targetH <= 0 || targetR <= 0)
                return;

            NativeMethods.DEVMODE currentMode = new NativeMethods.DEVMODE();
            currentMode.dmSize = (short)Marshal.SizeOf(currentMode);
            if (NativeMethods.EnumDisplaySettings(monitor.AdapterName, NativeMethods.ENUM_CURRENT_SETTINGS, ref currentMode))
            {
                if (currentMode.dmPelsWidth == targetW && 
                    currentMode.dmPelsHeight == targetH && 
                    currentMode.dmDisplayFrequency == targetR)
                {
                    return; 
                }
            }

            NativeMethods.DEVMODE applyMode = new NativeMethods.DEVMODE();
            applyMode.dmSize = (short)Marshal.SizeOf(applyMode);
            int j = 0;
            bool foundMode = false;
            
            while (NativeMethods.EnumDisplaySettings(monitor.AdapterName, j, ref applyMode))
            {
                if (applyMode.dmPelsWidth == targetW && 
                    applyMode.dmPelsHeight == targetH && 
                    applyMode.dmDisplayFrequency == targetR &&
                    applyMode.dmBitsPerPel >= 32)
                {
                    foundMode = true;
                    break;
                }
                j++;
            }

            if (foundMode)
            {
                applyMode.dmFields = 0x00080000 | 0x00100000 | 0x00400000;
                NativeMethods.ChangeDisplaySettingsEx(monitor.AdapterName, ref applyMode, IntPtr.Zero, NativeMethods.CDS_UPDATEREGISTRY, IntPtr.Zero);
            }
        }

        private void SetTouchscreenState(bool enable)
        {
            try
            {
                string deviceId = "";
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%touch screen%' OR Name LIKE '%touch screen%' OR Caption LIKE '%หน้าจอสัมผัส%' OR Name LIKE '%หน้าจอสัมผัส%'");
                foreach (ManagementObject obj in searcher.Get())
                {
                    deviceId = obj["PNPDeviceID"].ToString();
                    break;
                }

                if (string.IsNullOrEmpty(deviceId))
                {
                    return;
                }

                string action = enable ? "/enable-device" : "/disable-device";
                ProcessStartInfo psi = new ProcessStartInfo("pnputil", string.Format("{0} \"{1}\"", action, deviceId))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                var p = Process.Start(psi);
                if (p != null) p.WaitForExit();
                
                string statusMsg = enable ? Loc.Get("TouchEnabled") : Loc.Get("TouchDisabled");
                trayIcon.ShowBalloonTip(2000, Loc.Get("SettingsTitle"), statusMsg, ToolTipIcon.Info);
            }
            catch { }
        }

        private void EnableTouch(object sender, EventArgs e) { SetTouchscreenState(true); }
        private void DisableTouch(object sender, EventArgs e) { SetTouchscreenState(false); }
        
        private void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            SystemEvents.DisplaySettingsChanged -= DisplaySettingsChanged;
            Application.Exit();
        }
    }
}
