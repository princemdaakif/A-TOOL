using AAKIFTools.Core;

namespace AAKIFTools.Modules;

// ═══════════════════════════════════════════════════════════
//  App Manager — install, uninstall, disable, enable
// ═══════════════════════════════════════════════════════════
public static class AppModule
{
    public static void Run()
    {
        while (true)
        {
            ConsoleUI.PrintBanner();
            ConsoleUI.SectionHeader("📱  App Manager");

            ConsoleUI.PrintMenu(new[]
            {
                "Install APK(s) from PC",
                "Uninstall selected apps",
                "Disable selected apps",
                "Enable selected apps",
                "Clear app data / cache",
                "Force-stop selected apps",
                "List installed apps",
            });

            int choice = ConsoleUI.ReadMenuChoice(7);
            switch (choice)
            {
                case 0: return;
                case 1: InstallApks(); break;
                case 2: UninstallApps(); break;
                case 3: DisableApps(); break;
                case 4: EnableApps(); break;
                case 5: ClearAppData(); break;
                case 6: ForceStopApps(); break;
                case 7: ListApps(); break;
            }
        }
    }

    private static void InstallApks()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("📦  Install APK");

        ConsoleUI.Info("Enter APK paths one per line. Leave blank and press Enter when done.");
        Console.WriteLine();

        var apks = new List<string>();
        while (true)
        {
            string path = ConsoleUI.ReadInput($"APK path {apks.Count + 1}");
            if (string.IsNullOrEmpty(path)) break;
            if (File.Exists(path)) apks.Add(path);
            else ConsoleUI.Error($"File not found: {path}");
        }

        if (apks.Count == 0) { ConsoleUI.Warning("No APKs added."); ConsoleUI.Pause(); return; }

        Console.WriteLine();
        int ok = 0;
        foreach (string apk in apks)
        {
            string name = Path.GetFileName(apk);
            ConsoleUI.Spinner($"Installing {name}", () =>
            {
                bool success = AdbHelper.InstallApk(apk);
                if (success) ok++;
                else ConsoleUI.Error($"Failed: {name}");
            });
            ConsoleUI.Success($"Installed: {name}");
        }

        Console.WriteLine();
        ConsoleUI.Info($"Installed {ok}/{apks.Count} APKs.");
        ConsoleUI.Pause();
    }

    private static void UninstallApps()
    {
        var packages = AdbHelper.GetPackages(true);
        var indices = ConsoleUI.MultiSelect(packages, "Select apps to UNINSTALL");
        if (indices == null || indices.Count == 0) { ConsoleUI.Warning("Cancelled."); ConsoleUI.Pause(); return; }

        Console.WriteLine();
        foreach (int i in indices)
        {
            string pkg = packages[i];
            bool ok = AdbHelper.UninstallPackage(pkg);
            if (ok) ConsoleUI.Success($"Uninstalled: {pkg}");
            else    ConsoleUI.Error($"Failed: {pkg}");
        }
        ConsoleUI.Pause();
    }

    private static void DisableApps()
    {
        var packages = AdbHelper.GetPackages(false);
        var indices = ConsoleUI.MultiSelect(packages, "Select apps to DISABLE");
        if (indices == null || indices.Count == 0) { ConsoleUI.Warning("Cancelled."); ConsoleUI.Pause(); return; }

        foreach (int i in indices)
        {
            string pkg = packages[i];
            bool ok = AdbHelper.DisablePackage(pkg);
            if (ok) ConsoleUI.Success($"Disabled: {pkg}");
            else    ConsoleUI.Error($"Failed: {pkg}");
        }
        ConsoleUI.Pause();
    }

    private static void EnableApps()
    {
        string raw = AdbHelper.Shell("pm list packages -d");
        var disabled = raw.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(l => l.TrimStart().StartsWith("package:"))
            .Select(l => l.Trim().Replace("package:", "").Trim())
            .OrderBy(p => p).ToList();

        if (disabled.Count == 0) { ConsoleUI.Success("No disabled packages found."); ConsoleUI.Pause(); return; }

        var indices = ConsoleUI.MultiSelect(disabled, "Select apps to ENABLE");
        if (indices == null || indices.Count == 0) { ConsoleUI.Warning("Cancelled."); ConsoleUI.Pause(); return; }

        foreach (int i in indices)
        {
            string pkg = disabled[i];
            bool ok = AdbHelper.EnablePackage(pkg);
            if (ok) ConsoleUI.Success($"Enabled: {pkg}");
            else    ConsoleUI.Error($"Failed: {pkg}");
        }
        ConsoleUI.Pause();
    }

    private static void ClearAppData()
    {
        var packages = AdbHelper.GetPackages(true);
        var indices = ConsoleUI.MultiSelect(packages, "Select apps to CLEAR DATA");
        if (indices == null || indices.Count == 0) { ConsoleUI.Warning("Cancelled."); ConsoleUI.Pause(); return; }

        if (!ConsoleUI.Confirm($"This will erase ALL data for {indices.Count} app(s). Confirm?")) return;

        foreach (int i in indices)
        {
            string pkg = packages[i];
            string result = AdbHelper.Shell($"pm clear {pkg}");
            if (result.Contains("Success")) ConsoleUI.Success($"Cleared: {pkg}");
            else ConsoleUI.Error($"Failed: {pkg}");
        }
        ConsoleUI.Pause();
    }

    private static void ForceStopApps()
    {
        var packages = AdbHelper.GetPackages(true);
        var indices = ConsoleUI.MultiSelect(packages, "Select apps to FORCE STOP");
        if (indices == null || indices.Count == 0) { ConsoleUI.Warning("Cancelled."); ConsoleUI.Pause(); return; }

        foreach (int i in indices)
        {
            string pkg = packages[i];
            AdbHelper.Shell($"am force-stop {pkg}");
            ConsoleUI.Success($"Stopped: {pkg}");
        }
        ConsoleUI.Pause();
    }

    private static void ListApps()
    {
        ConsoleUI.Info("Loading installed apps...");
        var packages = AdbHelper.GetPackages(true);
        Console.WriteLine();
        foreach (var p in packages) ConsoleUI.Dim($"  • {p}");
        Console.WriteLine();
        ConsoleUI.Info($"Total: {packages.Count} user-installed apps");
        ConsoleUI.Pause();
    }
}

// ═══════════════════════════════════════════════════════════
//  Device Info
// ═══════════════════════════════════════════════════════════
public static class DeviceModule
{
    public static void Run()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("📊  Device Information");

        var info = new Dictionary<string, string>
        {
            ["Model"]           = AdbHelper.GetProp("ro.product.model"),
            ["Manufacturer"]    = AdbHelper.GetProp("ro.product.manufacturer"),
            ["Brand"]           = AdbHelper.GetProp("ro.product.brand"),
            ["Device"]          = AdbHelper.GetProp("ro.product.device"),
            ["Android Version"] = AdbHelper.GetProp("ro.build.version.release"),
            ["SDK Level"]       = AdbHelper.GetProp("ro.build.version.sdk"),
            ["Build Number"]    = AdbHelper.GetProp("ro.build.display.id"),
            ["Serial"]          = AdbHelper.GetDeviceSerial(),
            ["CPU ABI"]         = AdbHelper.GetProp("ro.product.cpu.abi"),
            ["Screen Density"]  = AdbHelper.Shell("wm density").Trim(),
            ["Screen Size"]     = AdbHelper.Shell("wm size").Trim(),
            ["Battery"]         = AdbHelper.Shell("dumpsys battery | grep level").Trim(),
            ["Total RAM"]       = AdbHelper.Shell("cat /proc/meminfo | grep MemTotal").Trim(),
        };

        foreach (var kv in info)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"  {kv.Key,-20}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(kv.Value);
        }

        Console.ResetColor();
        Console.WriteLine();
        ConsoleUI.Pause();
    }
}

// ═══════════════════════════════════════════════════════════
//  Screenshot & Screen Record
// ═══════════════════════════════════════════════════════════
public static class ScreenModule
{
    public static void Run()
    {
        while (true)
        {
            ConsoleUI.PrintBanner();
            ConsoleUI.SectionHeader("📸  Screenshot & Screen Record");

            ConsoleUI.PrintMenu(new[] { "Take screenshot", "Record screen (30 sec)", "Record screen (custom duration)" });
            int choice = ConsoleUI.ReadMenuChoice(3);
            switch (choice)
            {
                case 0: return;
                case 1: TakeScreenshot(); break;
                case 2: RecordScreen(30); break;
                case 3:
                    string s = ConsoleUI.ReadInput("Duration in seconds", "60");
                    RecordScreen(int.TryParse(s, out int d) ? d : 60);
                    break;
            }
        }
    }

    private static void TakeScreenshot()
    {
        string dir = AppDomain.CurrentDomain.BaseDirectory;
        string file = Path.Combine(dir, $"screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");

        ConsoleUI.Spinner("Capturing screenshot", () => AdbHelper.Screenshot(file));

        if (File.Exists(file)) ConsoleUI.Success($"Saved: {file}");
        else ConsoleUI.Error("Screenshot failed.");
        ConsoleUI.Pause();
    }

    private static void RecordScreen(int seconds)
    {
        string localFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"screenrecord_{DateTime.Now:HH-mm-ss}.mp4");
        string remoteFile = "/sdcard/atool_record.mp4";

        ConsoleUI.Warning($"Recording for {seconds} seconds... Press Ctrl+C to stop early.");
        ConsoleUI.Spinner($"Recording ({seconds}s)", () =>
        {
            AdbHelper.Shell($"screenrecord --time-limit {seconds} {remoteFile}");
        });
        ConsoleUI.Spinner("Pulling file to PC", () => AdbHelper.PullFile(remoteFile, localFile));
        AdbHelper.Shell($"rm {remoteFile}");

        if (File.Exists(localFile)) ConsoleUI.Success($"Saved: {localFile}");
        else ConsoleUI.Error("Recording failed.");
        ConsoleUI.Pause();
    }
}

// ═══════════════════════════════════════════════════════════
//  Wireless ADB
// ═══════════════════════════════════════════════════════════
public static class WirelessModule
{
    public static void Run()
    {
        while (true)
        {
            ConsoleUI.PrintBanner();
            ConsoleUI.SectionHeader("📡  Wireless ADB");

            ConsoleUI.PrintMenu(new[]
            {
                "Enable wireless mode  (USB required first)",
                "Connect to device by IP",
                "Disconnect all wireless",
                "Pair new device  (Android 11+)",
            });

            int choice = ConsoleUI.ReadMenuChoice(4);
            switch (choice)
            {
                case 0: return;
                case 1: EnableWireless(); break;
                case 2: ConnectByIp(); break;
                case 3: DisconnectAll(); break;
                case 4: PairDevice(); break;
            }
        }
    }

    private static void EnableWireless()
    {
        string port = ConsoleUI.ReadInput("Port", "5555");
        ConsoleUI.Spinner($"Starting ADB over TCP/IP on port {port}", () =>
        {
            AdbHelper.StartAdbOverTcpip(port);
        });
        ConsoleUI.Success($"ADB wireless started on port {port}");
        ConsoleUI.Info("Unplug USB, then use 'Connect to device by IP' option.");
        ConsoleUI.Pause();
    }

    private static void ConnectByIp()
    {
        string ip   = ConsoleUI.ReadInput("Device IP address");
        string port = ConsoleUI.ReadInput("Port", "5555");

        ConsoleUI.Spinner($"Connecting to {ip}:{port}", () => Thread.Sleep(500));
        bool ok = AdbHelper.ConnectWireless(ip, port);

        if (ok) ConsoleUI.Success($"Connected to {ip}:{port}");
        else    ConsoleUI.Error("Connection failed. Check IP and that wireless ADB is enabled.");
        ConsoleUI.Pause();
    }

    private static void DisconnectAll()
    {
        ConsoleUI.Spinner("Disconnecting all wireless connections", () => AdbHelper.Disconnect());
        ConsoleUI.Success("Disconnected.");
        ConsoleUI.Pause();
    }

    private static void PairDevice()
    {
        ConsoleUI.Info("On your device: Developer Options → Wireless debugging → Pair device with pairing code");
        Console.WriteLine();
        string ip   = ConsoleUI.ReadInput("Pairing IP:port (e.g. 192.168.1.5:37581)");
        string code = ConsoleUI.ReadInput("6-digit pairing code");

        ConsoleUI.Spinner("Pairing device", () =>
        {
            AdbHelper.Run($"pair {ip} {code}", 15000);
        });
        ConsoleUI.Success("Pairing attempted. Check device screen for confirmation.");
        ConsoleUI.Pause();
    }
}

// ═══════════════════════════════════════════════════════════
//  Reboot Options
// ═══════════════════════════════════════════════════════════
public static class RebootModule
{
    public static void Run()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("🔄  Reboot");

        ConsoleUI.PrintMenu(new[] { "Reboot normally", "Reboot to recovery", "Reboot to bootloader / fastboot", "Reboot to safe mode" });
        int choice = ConsoleUI.ReadMenuChoice(4);
        if (choice == 0) return;

        string[] modes = { "", "recovery", "bootloader", "" };
        string mode = modes[choice - 1];

        string label = new[] { "normal", "recovery", "bootloader", "safe mode" }[choice - 1];
        if (!ConsoleUI.Confirm($"Reboot to {label}?")) return;

        if (choice == 4)
        {
            // Safe mode via key injection
            AdbHelper.Shell("am broadcast -a android.intent.action.REBOOT_SAFEMODE_ENABLED");
            AdbHelper.Reboot();
        }
        else
        {
            AdbHelper.Reboot(mode);
        }

        ConsoleUI.Success("Reboot command sent.");
        ConsoleUI.Pause();
    }
}
