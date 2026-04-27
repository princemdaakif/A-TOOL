using AAKIFTools.Core;

namespace AAKIFTools.Modules;

public static class PerformanceModule
{
    public static void Run()
    {
        while (true)
        {
            ConsoleUI.PrintBanner();
            ConsoleUI.SectionHeader("⚡  Performance & Tweaks");

            ConsoleUI.PrintMenu(new[]
            {
                "Speed up animations  (reduce transition scale)",
                "Disable animations  (fastest feel)",
                "Restore default animations",
                "Enable USB debugging automatically",
                "Increase touch sensitivity",
                "Force 4x MSAA  (better GPU rendering)",
                "Set RAM expansion (ZRAM)",
                "Optimize battery settings",
                "Change DPI  (screen density)",
                "Change font scale",
                "Apply all recommended tweaks",
            });

            int choice = ConsoleUI.ReadMenuChoice(11);
            switch (choice)
            {
                case 0: return;
                case 1: SpeedUpAnimations(); break;
                case 2: DisableAnimations(); break;
                case 3: RestoreAnimations(); break;
                case 4: EnableUsbDebugging(); break;
                case 5: TouchSensitivity(); break;
                case 6: ForceMsaa(); break;
                case 7: SetZram(); break;
                case 8: BatteryOptimize(); break;
                case 9: ChangeDpi(); break;
                case 10: ChangeFontScale(); break;
                case 11: ApplyAllTweaks(); break;
            }
        }
    }

    private static void SpeedUpAnimations()
    {
        ConsoleUI.Spinner("Setting animations to 0.5x speed", () =>
        {
            AdbHelper.SetSetting("global", "window_animation_scale", "0.5");
            AdbHelper.SetSetting("global", "transition_animation_scale", "0.5");
            AdbHelper.SetSetting("global", "animator_duration_scale", "0.5");
        });
        ConsoleUI.Success("Animations set to 0.5x");
        ConsoleUI.Pause();
    }

    private static void DisableAnimations()
    {
        ConsoleUI.Spinner("Disabling all animations", () =>
        {
            AdbHelper.SetSetting("global", "window_animation_scale", "0");
            AdbHelper.SetSetting("global", "transition_animation_scale", "0");
            AdbHelper.SetSetting("global", "animator_duration_scale", "0");
        });
        ConsoleUI.Success("All animations disabled — phone will feel instant");
        ConsoleUI.Pause();
    }

    private static void RestoreAnimations()
    {
        ConsoleUI.Spinner("Restoring default animations (1.0x)", () =>
        {
            AdbHelper.SetSetting("global", "window_animation_scale", "1");
            AdbHelper.SetSetting("global", "transition_animation_scale", "1");
            AdbHelper.SetSetting("global", "animator_duration_scale", "1");
        });
        ConsoleUI.Success("Animations restored to default");
        ConsoleUI.Pause();
    }

    private static void EnableUsbDebugging()
    {
        ConsoleUI.Spinner("Enabling USB debugging via ADB", () =>
        {
            AdbHelper.SetSetting("global", "development_settings_enabled", "1");
            AdbHelper.SetSetting("global", "adb_enabled", "1");
        });
        ConsoleUI.Success("USB debugging enabled");
        ConsoleUI.Pause();
    }

    private static void TouchSensitivity()
    {
        ConsoleUI.Info("Increasing pointer speed...");
        ConsoleUI.Spinner("Setting pointer speed to max", () =>
        {
            AdbHelper.SetSetting("system", "pointer_speed", "7");
        });
        ConsoleUI.Success("Touch/pointer speed increased");
        ConsoleUI.Pause();
    }

    private static void ForceMsaa()
    {
        ConsoleUI.Spinner("Forcing 4x MSAA", () =>
        {
            AdbHelper.SetSetting("global", "debug.egl.force_msaa", "true");
            AdbHelper.Shell("setprop debug.egl.force_msaa true");
        });
        ConsoleUI.Success("4x MSAA forced — may improve GPU rendering quality");
        ConsoleUI.Pause();
    }

    private static void SetZram()
    {
        ConsoleUI.Info("Current ZRAM:");
        string current = AdbHelper.Shell("cat /proc/swaps");
        ConsoleUI.Dim(current);
        Console.WriteLine();

        string[] options = { "512 MB", "1 GB", "2 GB", "Disable ZRAM" };
        ConsoleUI.PrintMenu(options);
        int choice = ConsoleUI.ReadMenuChoice(4);
        if (choice == 0) return;

        string[] sizes = { "536870912", "1073741824", "2147483648", "0" };
        string size = sizes[choice - 1];

        ConsoleUI.Spinner($"Setting ZRAM to {options[choice - 1]}", () =>
        {
            AdbHelper.Shell($"swapoff /dev/block/zram0 2>/dev/null; echo {size} > /sys/block/zram0/disksize; mkswap /dev/block/zram0; swapon /dev/block/zram0");
        });

        ConsoleUI.Warning("ZRAM changes may require root or a reboot to take full effect.");
        ConsoleUI.Pause();
    }

    private static void BatteryOptimize()
    {
        ConsoleUI.Spinner("Applying battery optimizations", () =>
        {
            // Disable 'always on' location
            AdbHelper.SetSetting("secure", "location_mode", "1");
            // Aggressive doze
            AdbHelper.Shell("dumpsys deviceidle force-idle");
            // Background process limit
            AdbHelper.SetSetting("global", "background_process_limit", "4");
        });
        ConsoleUI.Success("Battery optimizations applied");
        ConsoleUI.Pause();
    }

    private static void ChangeDpi()
    {
        string current = AdbHelper.Shell("wm density").Trim();
        ConsoleUI.Info($"Current DPI: {current}");
        Console.WriteLine();
        ConsoleUI.Dim("Typical values: 320 (normal), 360 (medium-high), 420 (large), 480 (xlarge)");
        Console.WriteLine();

        string newDpi = ConsoleUI.ReadInput("Enter new DPI value (or press Enter to reset)");

        if (string.IsNullOrEmpty(newDpi))
        {
            ConsoleUI.Spinner("Resetting DPI to default", () => AdbHelper.Shell("wm density reset"));
            ConsoleUI.Success("DPI reset to default.");
        }
        else
        {
            ConsoleUI.Spinner($"Setting DPI to {newDpi}", () => AdbHelper.Shell($"wm density {newDpi}"));
            ConsoleUI.Success($"DPI set to {newDpi}. Reboot recommended.");
        }
        ConsoleUI.Pause();
    }

    private static void ChangeFontScale()
    {
        string current = AdbHelper.GetSetting("system", "font_scale");
        ConsoleUI.Info($"Current font scale: {current}");
        Console.WriteLine();
        ConsoleUI.Dim("Values: 0.85 (small), 1.0 (normal), 1.15 (large), 1.30 (largest)");
        Console.WriteLine();

        string val = ConsoleUI.ReadInput("Enter font scale", "1.0");
        ConsoleUI.Spinner($"Setting font scale to {val}", () =>
        {
            AdbHelper.SetSetting("system", "font_scale", val);
        });
        ConsoleUI.Success($"Font scale set to {val}");
        ConsoleUI.Pause();
    }

    private static void ApplyAllTweaks()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("⚡  Apply All Recommended Tweaks");
        ConsoleUI.Warning("This will apply the following:");
        ConsoleUI.Dim("  • Animations disabled (0x)");
        ConsoleUI.Dim("  • Background process limit = 4");
        ConsoleUI.Dim("  • Aggressive doze battery mode");
        ConsoleUI.Dim("  • 4x MSAA enabled");
        Console.WriteLine();

        if (!ConsoleUI.Confirm("Proceed?")) return;

        ConsoleUI.Spinner("Disabling animations", () =>
        {
            AdbHelper.SetSetting("global", "window_animation_scale", "0");
            AdbHelper.SetSetting("global", "transition_animation_scale", "0");
            AdbHelper.SetSetting("global", "animator_duration_scale", "0");
        });

        ConsoleUI.Spinner("Setting background process limit", () =>
        {
            AdbHelper.SetSetting("global", "background_process_limit", "4");
        });

        ConsoleUI.Spinner("Enabling aggressive doze", () =>
        {
            AdbHelper.Shell("dumpsys deviceidle force-idle");
        });

        ConsoleUI.Spinner("Forcing 4x MSAA", () =>
        {
            AdbHelper.SetSetting("global", "debug.egl.force_msaa", "true");
        });

        Console.WriteLine();
        ConsoleUI.Success("All tweaks applied successfully!");
        ConsoleUI.Pause();
    }
}
