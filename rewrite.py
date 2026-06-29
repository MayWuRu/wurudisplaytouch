import codecs
import re

with codecs.open('App.cs', 'r', 'utf-8') as f:
    content = f.read()

loc_class = """
    public static class Loc
    {
        public static string Lang { get; set; } = "EN";

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
            { "Note", new[] { "*Note: If Auto mode causes messy scaling (especially on 2K monitors),\\n please manually lock the resolution to match your physical monitor.", "*หมายเหตุ: หากใช้ Auto แล้วสเกลภาพเพี้ยน (โดยเฉพาะจอ 2K)\\n แนะนำให้เลือกล็อคความละเอียดให้ตรงกับสเปคจอเอง" } },
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
"""
content = content.replace('    static class Program', loc_class + '\n    static class Program')

main_old = """        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayApplicationContext());
        }"""
main_new = """        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Loc.Lang = Settings.Language;
            Application.Run(new TrayApplicationContext());
        }"""
content = content.replace(main_old, main_new)

settings_lang = """        public static string Language
        {
            get { return GetGlobalRegistryStringValue("Language", "EN"); }
            set { SetGlobalRegistryStringValue("Language", value); }
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
"""
content = content.replace('        public static bool TouchAutoMode', settings_lang + '\n        public static bool TouchAutoMode')

content = content.replace('string type = IsInternal ? "(จอติดเครื่อง)" : "(จอนอก)";', 'string type = IsInternal ? Loc.Get("Internal") : Loc.Get("External");')
content = content.replace('if (IsAuto) return "Auto (ดึงค่าสูงสุดที่จอรับได้)";', 'if (IsAuto) return Loc.Get("AutoRes");')
content = content.replace('if (IsAuto) return "Auto (สูงสุดที่รองรับ)";', 'if (IsAuto) return Loc.Get("AutoRef");')

tray_old = """        public TrayApplicationContext()
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

            trayIcon = new NotifyIcon()
            {
                Icon = appIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("เปิด Touchscreen (Force Enable)", EnableTouch),
                    new MenuItem("ปิด Touchscreen (Force Disable)", DisableTouch),
                    new MenuItem("-"),
                    new MenuItem("การตั้งค่า (Settings)...", OpenSettings),
                    new MenuItem("-"),
                    new MenuItem("ออก (Exit)", Exit)
                }),
                Visible = true,
                Text = "WuRuDisplayTouch"
            };"""
tray_new = """
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
            };"""
content = content.replace(tray_old, tray_new)

update_lang = """        public void UpdateLanguage()
        {
            miEnable.Text = Loc.Get("ForceEnable");
            miDisable.Text = Loc.Get("ForceDisable");
            miSettings.Text = Loc.Get("Settings");
            miExit.Text = Loc.Get("Exit");
            trayIcon.Text = Loc.Get("SettingsTitle");
        }"""
content = content.replace('private void OpenSettings', update_lang + '\n\n        private void OpenSettings')

open_set_old = """                if (sf.ShowDialog() == DialogResult.OK)
                {
                    CheckDisplayAndToggleTouch();
                }"""
open_set_new = """                if (sf.ShowDialog() == DialogResult.OK)
                {
                    UpdateLanguage();
                    CheckDisplayAndToggleTouch();
                }"""
content = content.replace(open_set_old, open_set_new)
content = content.replace('string statusMsg = enable ? "เปิด Touchscreen (จอหลักทำงาน)" : "ปิด Touchscreen (ต่อจอนอก)";', 'string statusMsg = enable ? Loc.Get("TouchEnabled") : Loc.Get("TouchDisabled");')

sf_new = """    public class SettingsForm : Form
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

        private bool CheckIfStartupTaskExists()"""
content = re.sub(r'    public class SettingsForm : Form\r?\n    \{.*?private bool CheckIfStartupTaskExists\(\)', sf_new, content, flags=re.DOTALL)

with codecs.open('App.cs', 'w', 'utf-8') as f:
    f.write(content)
