using System.Diagnostics;

namespace AAKIFTools.Core;

public static class AdbHelper
{
    private static string AdbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adb.exe");
    private static string FastbootPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fastboot.exe");

    public static string Run(string args, int timeoutMs = 15000)
    {
        try
        {
            var psi = new ProcessStartInfo(AdbPath, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi)!;
            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();
            proc.WaitForExit(timeoutMs);
            return string.IsNullOrWhiteSpace(output) ? error : output;
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }

    public static string RunFastboot(string args)
    {
        try
        {
            var psi = new ProcessStartInfo(FastbootPath, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi)!;
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit(10000);
            return output;
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }

    public static string Shell(string command) => Run($"shell {command}");

    public static bool IsDeviceConnected()
    {
        string result = Run("devices");
        var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return lines.Any(l => l.Contains("\tdevice") && !l.StartsWith("List"));
    }

    public static string GetDeviceSerial()
    {
        string result = Run("devices");
        var line = result.Split('\n').FirstOrDefault(l => l.Contains("\tdevice") && !l.StartsWith("List"));
        return line?.Split('\t')[0].Trim() ?? "unknown";
    }

    public static List<string> GetPackages(bool thirdPartyOnly = false)
    {
        string flag = thirdPartyOnly ? "-3" : "";
        string result = Shell($"pm list packages {flag}");
        return result
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(l => l.TrimStart().StartsWith("package:"))
            .Select(l => l.Trim().Replace("package:", "").Trim())
            .OrderBy(p => p)
            .ToList();
    }

    public static bool UninstallPackage(string packageName, bool keepData = false)
    {
        string flag = keepData ? "-k" : "";
        string result = Shell($"pm uninstall {flag} --user 0 {packageName}");
        return result.Contains("Success");
    }

    public static bool DisablePackage(string packageName)
    {
        string result = Shell($"pm disable-user --user 0 {packageName}");
        return result.Contains("disabled");
    }

    public static bool EnablePackage(string packageName)
    {
        string result = Shell($"pm enable {packageName}");
        return result.Contains("enabled");
    }

    public static bool InstallApk(string apkPath)
    {
        string result = Run($"install -r \"{apkPath}\"", 60000);
        return result.Contains("Success");
    }

    public static string GetProp(string prop) => Shell($"getprop {prop}").Trim();

    public static void SetSetting(string namespace_, string key, string value)
        => Shell($"settings put {namespace_} {key} {value}");

    public static string GetSetting(string namespace_, string key)
        => Shell($"settings get {namespace_} {key}").Trim();

    public static void StartAdbOverTcpip(string port = "5555") => Run($"tcpip {port}");

    public static bool ConnectWireless(string ip, string port = "5555")
    {
        string result = Run($"connect {ip}:{port}", 8000);
        return result.Contains("connected");
    }

    public static void Disconnect() => Run("disconnect");

    public static void Reboot(string mode = "") => Run($"reboot {mode}".Trim());

    public static string PullFile(string remotePath, string localPath)
        => Run($"pull \"{remotePath}\" \"{localPath}\"", 120000);

    public static string PushFile(string localPath, string remotePath)
        => Run($"push \"{localPath}\" \"{remotePath}\"", 120000);

    public static string Screenshot(string localPath)
    {
        Shell("screencap -p /sdcard/screenshot_atool.png");
        return PullFile("/sdcard/screenshot_atool.png", localPath);
    }

    public static string StartScreenRecord(string remotePath, int durationSec = 180)
        => Shell($"screenrecord --time-limit {durationSec} {remotePath}");
}
