using AAKIFTools.Core;

namespace AAKIFTools.Modules;

public static class ReVancedModule
{
    private static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

    public static void Run()
    {
        while (true)
        {
            ConsoleUI.PrintBanner();
            ConsoleUI.SectionHeader("🔴  ReVanced Installer");

            ConsoleUI.PrintMenu(new[]
            {
                "Install ReVanced YouTube",
                "Install ReVanced Manager Plus",
                "Install custom APK",
                "Check device architecture",
            });

            int choice = ConsoleUI.ReadMenuChoice(4);
            switch (choice)
            {
                case 0: return;
                case 1: InstallReVanced(); break;
                case 2: InstallReVancedManager(); break;
                case 3: InstallCustomApk(); break;
                case 4: CheckArchitecture(); break;
            }
        }
    }

    private static void InstallReVanced()
    {
        string apkPath = Path.Combine(BasePath, "revanced.apk");
        if (!File.Exists(apkPath))
        {
            ConsoleUI.Error("revanced.apk not found in tool directory.");
            ConsoleUI.Info("Place a compatible revanced.apk next to AAKIFTools.exe");
            ConsoleUI.Pause();
            return;
        }

        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("🔴  Install ReVanced YouTube");
        ConsoleUI.Warning("Make sure your base YouTube APK is already installed.");
        ConsoleUI.Dim("ReVanced is installed as a separate app and does not replace the original.");
        Console.WriteLine();

        if (!ConsoleUI.Confirm("Proceed with installation?")) return;

        bool ok = false;
        ConsoleUI.Spinner("Installing ReVanced YouTube...", () =>
        {
            ok = AdbHelper.InstallApk(apkPath);
        });

        if (ok)
        {
            ConsoleUI.Success("ReVanced YouTube installed successfully!");
            ConsoleUI.Info("Launch it from your app drawer.");
        }
        else
        {
            ConsoleUI.Error("Installation failed. Common reasons:");
            ConsoleUI.Dim("  • Wrong APK architecture (arm64 vs arm vs x86)");
            ConsoleUI.Dim("  • Base YouTube version mismatch");
            ConsoleUI.Dim("  • Device doesn't allow sideloading (check Unknown Sources)");
        }
        ConsoleUI.Pause();
    }

    private static void InstallReVancedManager()
    {
        string apkPath = Path.Combine(BasePath, "revanced_manager_plus_v3.0.16.apk");
        if (!File.Exists(apkPath))
        {
            ConsoleUI.Error("revanced_manager_plus_v3.0.16.apk not found.");
            ConsoleUI.Pause();
            return;
        }

        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("🔴  Install ReVanced Manager Plus v3.0.16");
        ConsoleUI.Info("ReVanced Manager lets you patch apps directly on your device.");
        Console.WriteLine();

        if (!ConsoleUI.Confirm("Install ReVanced Manager Plus?")) return;

        bool ok = false;
        ConsoleUI.Spinner("Installing ReVanced Manager Plus...", () =>
        {
            ok = AdbHelper.InstallApk(apkPath);
        });

        if (ok) ConsoleUI.Success("ReVanced Manager Plus installed!");
        else    ConsoleUI.Error("Installation failed.");
        ConsoleUI.Pause();
    }

    private static void InstallCustomApk()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("📦  Install Custom APK");

        string path = ConsoleUI.ReadInput("Path to APK file");
        if (!File.Exists(path))
        {
            ConsoleUI.Error("File not found."); ConsoleUI.Pause(); return;
        }

        bool ok = false;
        ConsoleUI.Spinner($"Installing {Path.GetFileName(path)}", () =>
        {
            ok = AdbHelper.InstallApk(path);
        });

        if (ok) ConsoleUI.Success("Installed successfully!");
        else    ConsoleUI.Error("Installation failed.");
        ConsoleUI.Pause();
    }

    private static void CheckArchitecture()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("🔍  Device Architecture");

        string abi    = AdbHelper.GetProp("ro.product.cpu.abi");
        string abilist = AdbHelper.GetProp("ro.product.cpu.abilist");
        string arch    = AdbHelper.Shell("uname -m").Trim();

        ConsoleUI.Info($"Primary ABI:  {abi}");
        ConsoleUI.Info($"ABI list:     {abilist}");
        ConsoleUI.Info($"Kernel arch:  {arch}");
        Console.WriteLine();

        string recommendation = abi switch
        {
            var a when a.Contains("arm64") => "✔ Use arm64-v8a APKs (recommended)",
            var a when a.Contains("armeabi") => "⚠ Use armeabi-v7a APKs",
            var a when a.Contains("x86_64") => "✔ Use x86_64 APKs",
            var a when a.Contains("x86") => "⚠ Use x86 APKs",
            _ => "Unknown architecture"
        };
        ConsoleUI.Success(recommendation);
        ConsoleUI.Pause();
    }
}
