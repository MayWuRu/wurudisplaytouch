using System;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace WuRuDisplayTouch
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayApplicationContext());
        }
    }

    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private MenuItem autoModeMenuItem;
        private MenuItem startupMenuItem;
        private bool isAutoMode = true;
        private string taskName = "WuRuDisplayTouch_Startup";

        public TrayApplicationContext()
        {
            Icon appIcon;
            try
            {
                appIcon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("WuRuDisplayTouch.icon.ico"));
            }
            catch
            {
                appIcon = SystemIcons.Shield; // สำรองกรณีโหลดไอคอนไม่ได้
            }

            trayIcon = new NotifyIcon()
            {
                Icon = appIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("เปิด Touchscreen (Force Enable)", EnableTouch),
                    new MenuItem("ปิด Touchscreen (Force Disable)", DisableTouch),
                    new MenuItem("-"),
                    autoModeMenuItem = new MenuItem("ระบบอัตโนมัติ (Auto Mode)", ToggleAutoMode) { Checked = true },
                    startupMenuItem = new MenuItem("เปิดพร้อม Windows (Run on Startup)", ToggleStartup) { Checked = CheckIfStartupTaskExists() },
                    new MenuItem("-"),
                    new MenuItem("ออก (Exit)", Exit)
                }),
                Visible = true,
                Text = "WuRuDisplayTouch"
            };

            SystemEvents.DisplaySettingsChanged += DisplaySettingsChanged;
            
            CheckDisplayAndToggleTouch();
        }

        private void ToggleAutoMode(object sender, EventArgs e)
        {
            isAutoMode = !isAutoMode;
            autoModeMenuItem.Checked = isAutoMode;
            if (isAutoMode) CheckDisplayAndToggleTouch();
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

        private void ToggleStartup(object sender, EventArgs e)
        {
            bool isCurrentlyEnabled = startupMenuItem.Checked;
            string exePath = Application.ExecutablePath;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "schtasks";
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;

                if (isCurrentlyEnabled)
                {
                    psi.Arguments = string.Format("/delete /tn \"{0}\" /f", taskName);
                }
                else
                {
                    psi.Arguments = string.Format("/create /tn \"{0}\" /tr \"\\\"{1}\\\"\" /sc onlogon /rl highest /f", taskName, exePath);
                }

                var p = Process.Start(psi);
                if (p != null) p.WaitForExit();

                if (p.ExitCode == 0)
                {
                    startupMenuItem.Checked = !isCurrentlyEnabled;
                    trayIcon.ShowBalloonTip(2000, "WuRuDisplayTouch", 
                        !isCurrentlyEnabled ? "ตั้งค่าเปิดโปรแกรมพร้อม Windows แล้ว" : "ยกเลิกการเปิดโปรแกรมพร้อม Windows แล้ว", 
                        ToolTipIcon.Info);
                }
                else
                {
                    MessageBox.Show("ไม่สามารถตั้งค่า Task Scheduler ได้ กรุณาตรวจสอบว่าโปรแกรมถูกเปิดด้วยสิทธิ์ Admin", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (isAutoMode)
            {
                CheckDisplayAndToggleTouch();
            }
        }

        [DllImport("user32.dll")]
        static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceString;
            public uint StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceKey;
        }

        private void CheckDisplayAndToggleTouch()
        {
            bool internalDisplayActive = false;
            try
            {
                List<string> activeDeviceIds = new List<string>();
                uint devNum = 0;
                DISPLAY_DEVICE d = new DISPLAY_DEVICE();
                d.cb = Marshal.SizeOf(d);

                while (EnumDisplayDevices(null, devNum, ref d, 0))
                {
                    if ((d.StateFlags & 0x1) == 0x1) // ATTACHED_TO_DESKTOP
                    {
                        DISPLAY_DEVICE mon = new DISPLAY_DEVICE();
                        mon.cb = Marshal.SizeOf(mon);
                        if (EnumDisplayDevices(d.DeviceName, 0, ref mon, 0))
                        {
                            activeDeviceIds.Add(mon.DeviceID);
                        }
                    }
                    devNum++;
                }

                var searcher = new ManagementObjectSearcher("root\\wmi", "SELECT * FROM WmiMonitorConnectionParams");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    string instanceName = queryObj["InstanceName"] != null ? queryObj["InstanceName"].ToString() : "";
                    uint tech = 0;
                    try { tech = Convert.ToUInt32(queryObj["VideoOutputTechnology"]); } catch { }

                    // 11 = eDP, 7 = LVDS, 2147483648 (0x80000000) = Internal
                    if (tech == 11 || tech == 7 || tech == 2147483648)
                    {
                        foreach (string activeId in activeDeviceIds)
                        {
                            string[] parts = activeId.Split('\\');
                            if (parts.Length > 1)
                            {
                                string pnpId = parts[1];
                                if (instanceName.Contains(pnpId))
                                {
                                    internalDisplayActive = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            if (Screen.AllScreens.Length > 1)
            {
                internalDisplayActive = true;
            }

            SetTouchscreenState(internalDisplayActive);
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
                
                string statusMsg = enable ? "เปิด Touchscreen (จอหลักทำงาน)" : "ปิด Touchscreen (ต่อจอนอก)";
                trayIcon.ShowBalloonTip(2000, "WuRuDisplayTouch", statusMsg, ToolTipIcon.Info);
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
