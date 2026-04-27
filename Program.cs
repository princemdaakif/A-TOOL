using AAKIFTools.Core;
using AAKIFTools.Modules;

namespace AAKIFTools;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "A-TOOL — Android ADB Utility";
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Handle CLI args (e.g. --debloat manual)
        if (args.Length > 0)
        {
            HandleCliArgs(args);
            return;
        }

        // Wait for device on startup
        ConsoleUI.PrintBanner();
        CheckDevice();

        // Main loop
        while (true)
        {
            ConsoleUI.PrintBanner();
            ShowDeviceStatus();
            ShowMainMenu();
        }
    }

    static void CheckDevice()
    {
        if (!AdbHelper.IsDeviceConnected())
        {
            ConsoleUI.Warning("No device detected.");
            ConsoleUI.Dim("  Connect your Android phone via USB with USB Debugging enabled.");
            ConsoleUI.Dim("  Or use Wireless ADB from the menu.");
            Console.WriteLine();
            ConsoleUI.PrintMenu(new[] { "Retry detection", "Continue anyway (wireless)" });
            int c = ConsoleUI.ReadMenuChoice(2);
            if (c == 1) CheckDevice();
        }
    }

    static void ShowDeviceStatus()
    {
        if (AdbHelper.IsDeviceConnected())
        {
            string model  = AdbHelper.GetProp("ro.product.model");
            string android = AdbHelper.GetProp("ro.build.version.release");
            string serial = AdbHelper.GetDeviceSerial();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✔  Connected: {model}  •  Android {android}  •  {serial}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ✘  No device connected");
            Console.ResetColor();
        }
        Console.WriteLine();
    }

    static void ShowMainMenu()
    {
        ConsoleUI.PrintMenu(new[]
        {
            "📊  Device Information",
            "📦  Debloat Manager",
            "💾  Backup & Restore",
            "⚡  Performance & Tweaks",
            "📱  App Manager",
            "🔴  ReVanced Installer",
            "📸  Screenshot & Screen Record",
            "📡  Wireless ADB",
            "🔄  Reboot Options",
        }, hint: "Select a module to continue");

        int choice = ConsoleUI.ReadMenuChoice(9);
        switch (choice)
        {
            case 0:
                if (ConsoleUI.Confirm("Exit A-TOOL?")) Environment.Exit(0);
                break;
            case 1: DeviceModule.Run(); break;
            case 2: DebloatModule.Run(); break;
            case 3: BackupModule.Run(); break;
            case 4: PerformanceModule.Run(); break;
            case 5: AppModule.Run(); break;
            case 6: ReVancedModule.Run(); break;
            case 7: ScreenModule.Run(); break;
            case 8: WirelessModule.Run(); break;
            case 9: RebootModule.Run(); break;
        }
    }

    static void HandleCliArgs(string[] args)
    {
        switch (args[0].ToLower())
        {
            case "--debloat":
                if (args.Length > 1 && args[1] == "manual")
                    DebloatModule.Run();
                break;
            case "--device":
                DeviceModule.Run();
                break;
            case "--help":
                PrintHelp();
                break;
            default:
                Console.WriteLine($"Unknown argument: {args[0]}");
                PrintHelp();
                break;
        }
    }

    static void PrintHelp()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("📖  CLI Usage");
        ConsoleUI.Info("AAKIFTools.exe [option]");
        Console.WriteLine();
        ConsoleUI.Dim("  (no args)       Launch interactive menu");
        ConsoleUI.Dim("  --debloat manual  Open manual debloat selector directly");
        ConsoleUI.Dim("  --device          Show device info and exit");
        ConsoleUI.Dim("  --help            Show this help");
        Console.WriteLine();
    }
}
