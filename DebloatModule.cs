using AAKIFTools.Core;
using AAKIFTools.Data;

namespace AAKIFTools.Modules;

public static class DebloatModule
{
    public static void Run()
    {
        while (true)
        {
            ConsoleUI.PrintBanner();
            ConsoleUI.SectionHeader("📦  Debloat Manager");

            ConsoleUI.PrintMenu(new[]
            {
                "Preset debloat  —  Remove known OEM bloat by brand",
                "Manual selection  —  Browse ALL packages, pick what to remove",
                "Remove 3rd-party apps  —  User-installed apps only",
                "Restore disabled packages  —  Re-enable packages disabled by A-TOOL",
                "View installed packages",
            });

            int choice = ConsoleUI.ReadMenuChoice(5);
            switch (choice)
            {
                case 0: return;
                case 1: RunPreset(); break;
                case 2: RunManual(thirdPartyOnly: false); break;
                case 3: RunManual(thirdPartyOnly: true); break;
                case 4: RestoreDisabled(); break;
                case 5: ViewPackages(); break;
            }
        }
    }

    // ── Preset Debloat ────────────────────────────────────────────────────────
    private static void RunPreset()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("📦  Preset Debloat — Select Brand");

        var brands = DebloatPresets.Presets.Keys.ToList();
        for (int i = 0; i < brands.Count; i++)
        {
            string brand = brands[i];
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"    [{i + 1}]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  {brand}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  — {DebloatPresets.Descriptions[brand]}");
        }
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("    [0]  ← Back");
        Console.ResetColor();
        Console.WriteLine();

        int choice = ConsoleUI.ReadMenuChoice(brands.Count);
        if (choice == 0) return;

        string selected = brands[choice - 1];
        string[] packages = DebloatPresets.Presets[selected];

        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader($"📦  {selected}");
        ConsoleUI.Info($"Packages to remove: {packages.Length}");
        Console.WriteLine();

        // Let user review & deselect before proceeding
        var toRemove = ReviewAndSelectFromList(packages.ToList(), $"Review — {selected}");
        if (toRemove == null || toRemove.Count == 0)
        {
            ConsoleUI.Warning("No packages selected. Returning.");
            ConsoleUI.Pause();
            return;
        }

        ExecuteDebloat(toRemove);
    }

    // ── Manual Selection ──────────────────────────────────────────────────────
    private static void RunManual(bool thirdPartyOnly)
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("📦  Manual Package Selection");

        string typeLabel = thirdPartyOnly ? "3rd-party (user-installed)" : "ALL";
        ConsoleUI.Info($"Loading {typeLabel} packages from device...");

        List<string> packages = new();
        ConsoleUI.Spinner("Fetching package list", () =>
        {
            packages = AdbHelper.GetPackages(thirdPartyOnly);
        });

        if (packages.Count == 0)
        {
            ConsoleUI.Error("No packages found. Is a device connected?");
            ConsoleUI.Pause();
            return;
        }

        ConsoleUI.Info($"Found {packages.Count} packages. Launching selector...");
        Thread.Sleep(600);

        var selectedIndices = ConsoleUI.MultiSelect(
            packages,
            title: $"Select packages to REMOVE  ({typeLabel})",
            hint: "↑↓=navigate  SPACE=toggle  /=filter  A=select all  N=clear  ENTER=confirm  ESC=cancel"
        );

        if (selectedIndices == null || selectedIndices.Count == 0)
        {
            ConsoleUI.PrintBanner();
            ConsoleUI.Warning("No packages selected. Operation cancelled.");
            ConsoleUI.Pause();
            return;
        }

        var toRemove = selectedIndices.Select(i => packages[i]).ToList();
        ExecuteDebloat(toRemove);
    }

    // ── Shared Execution ──────────────────────────────────────────────────────
    private static void ExecuteDebloat(List<string> packages)
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("📦  Confirm Removal");

        ConsoleUI.Warning($"You are about to REMOVE {packages.Count} package(s):");
        Console.WriteLine();
        foreach (var pkg in packages)
            ConsoleUI.Dim($"  • {pkg}");
        Console.WriteLine();

        bool disable = ConsoleUI.Confirm("Disable instead of uninstall? (safer, reversible)");
        Console.WriteLine();

        if (!ConsoleUI.Confirm($"Confirm {(disable ? "disable" : "uninstall")} of {packages.Count} package(s)?"))
        {
            ConsoleUI.Warning("Cancelled.");
            ConsoleUI.Pause();
            return;
        }

        // Save log
        string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debloat_log.txt");
        var logLines = new List<string> { $"# A-TOOL Debloat Log — {DateTime.Now}", "" };

        Console.WriteLine();
        int success = 0, fail = 0;

        for (int i = 0; i < packages.Count; i++)
        {
            string pkg = packages[i];
            ConsoleUI.ProgressBar(i, packages.Count, pkg);

            bool ok = disable
                ? AdbHelper.DisablePackage(pkg)
                : AdbHelper.UninstallPackage(pkg);

            if (ok) { success++; logLines.Add($"[OK]   {pkg}"); }
            else    { fail++;    logLines.Add($"[FAIL] {pkg}"); }
        }

        ConsoleUI.ProgressBar(packages.Count, packages.Count, "Done");
        Console.WriteLine();
        Console.WriteLine();

        File.WriteAllLines(logPath, logLines);

        ConsoleUI.Success($"Completed: {success} removed, {fail} failed");
        ConsoleUI.Info($"Log saved: {logPath}");
        ConsoleUI.Pause();
    }

    // ── Restore Disabled ──────────────────────────────────────────────────────
    private static void RestoreDisabled()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("♻  Restore Disabled Packages");

        ConsoleUI.Info("Fetching disabled packages...");
        string raw = AdbHelper.Shell("pm list packages -d");
        var disabled = raw
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(l => l.TrimStart().StartsWith("package:"))
            .Select(l => l.Trim().Replace("package:", "").Trim())
            .OrderBy(p => p)
            .ToList();

        if (disabled.Count == 0)
        {
            ConsoleUI.Success("No disabled packages found.");
            ConsoleUI.Pause();
            return;
        }

        var selectedIndices = ConsoleUI.MultiSelect(
            disabled,
            title: "Select packages to RE-ENABLE",
            hint: "↑↓=navigate  SPACE=toggle  A=all  ENTER=confirm  ESC=cancel"
        );

        if (selectedIndices == null || selectedIndices.Count == 0)
        {
            ConsoleUI.Warning("Nothing selected."); ConsoleUI.Pause(); return;
        }

        Console.WriteLine();
        int ok = 0;
        foreach (int idx in selectedIndices)
        {
            string pkg = disabled[idx];
            bool success = AdbHelper.EnablePackage(pkg);
            if (success) { ConsoleUI.Success(pkg); ok++; }
            else          ConsoleUI.Error($"Failed: {pkg}");
        }
        Console.WriteLine();
        ConsoleUI.Info($"Re-enabled {ok}/{selectedIndices.Count} packages.");
        ConsoleUI.Pause();
    }

    // ── View Packages ─────────────────────────────────────────────────────────
    private static void ViewPackages()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("📋  Installed Packages");

        ConsoleUI.PrintMenu(new[] { "All packages", "System packages only", "User / 3rd-party only" });
        int choice = ConsoleUI.ReadMenuChoice(3);
        if (choice == 0) return;

        string flag = choice == 1 ? "" : choice == 2 ? "-s" : "-3";
        ConsoleUI.Info("Loading...");
        var packages = AdbHelper.GetPackages(choice == 3);

        Console.WriteLine();
        foreach (var pkg in packages)
            ConsoleUI.Dim(pkg);

        Console.WriteLine();
        ConsoleUI.Info($"Total: {packages.Count} packages");

        string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "packages.txt");
        if (ConsoleUI.Confirm("Save list to file?"))
        {
            File.WriteAllLines(savePath, packages);
            ConsoleUI.Success($"Saved to {savePath}");
        }
        ConsoleUI.Pause();
    }

    // ── Helper ────────────────────────────────────────────────────────────────
    private static List<string>? ReviewAndSelectFromList(List<string> items, string title)
    {
        var indices = ConsoleUI.MultiSelect(items, title);
        if (indices == null) return null;
        // Default: all selected
        if (indices.Count == 0) return items; // If nothing deselected just return all
        return indices.Select(i => items[i]).ToList();
    }
}
