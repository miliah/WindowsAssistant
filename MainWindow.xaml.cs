using System;
using System.Drawing; // اضافه کردن فضای نام برای استفاده از NotifyIcon
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Threading;
using System.Management;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms; // اضافه کردن فضای نام برای NotifyIcon

namespace WindowsAssistant
{
    public partial class MainWindow : Window
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private DispatcherTimer timer;
        private NotifyIcon trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSystemInfo();
            InitializeTrayIcon();
        }

        private void InitializeSystemInfo()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            // Set up a timer to update the system info every second
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (sender, args) => UpdateSystemInfo();
            timer.Start();
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = true,
                Text = "Windows Assistant"
            };

            // Double-clicking on tray icon restores the main window
            trayIcon.DoubleClick += (sender, args) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            // Hide the window when minimized
            this.StateChanged += (sender, args) =>
            {
                if (this.WindowState == WindowState.Minimized)
                {
                    this.Hide();
                }
            };
        }

        private void UpdateSystemInfo()
        {
            // CPU Usage
            float cpuUsage = cpuCounter.NextValue();
            CPUUsageText.Text = $"بار سی‌پی‌یو: {cpuUsage:F2}%";

            // RAM Usage
            float availableRAM = ramCounter.NextValue();
            RAMUsageText.Text = $"بار RAM: {availableRAM:F2}MB";

            // Internet Status
            string ipAddress = GetLocalIPAddress();
            IPAddressText.Text = $"آی‌پی محلی: {ipAddress}";
            InternetStatusText.Text = $"وضعیت اینترنت: {(NetworkInterface.GetIsNetworkAvailable() ? "متصل" : "قطع")}";

            // System Uptime
            TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount);
            UptimeText.Text = $"مدت روشن بودن سیستم: {uptime.Days} روز, {uptime.Hours} ساعت, {uptime.Minutes} دقیقه";

            // Date and Time in Persian
            DateTime currentDate = DateTime.Now;
            PersianCalendar persianCalendar = new PersianCalendar();
            string persianDate = $"{persianCalendar.GetYear(currentDate)}/{persianCalendar.GetMonth(currentDate):00}/{persianCalendar.GetDayOfMonth(currentDate):00}";
            DateTimeText.Text = $"تاریخ شمسی: {persianDate}";

            // Update the analog clock (just show current time for simplicity)
            ClockText.Text = $"ساعت: {currentDate.ToString("HH:mm:ss")}";
        }

        private string GetLocalIPAddress()
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return unicastAddress.Address.ToString();
                        }
                    }
                }
            }
            return "IP not found";
        }

        // Open Calculator
        private void OpenCalculator_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("calc.exe");
        }
    }
}
