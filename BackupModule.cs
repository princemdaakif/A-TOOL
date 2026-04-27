using AAKIFTools.Core;

namespace AAKIFTools.Modules;

public static class BackupModule
{
    private static string BackupDir =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backups",
            DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

    public static void Run()
    {
        while (true)
        {
            ConsoleUI.PrintBanner();
            ConsoleUI.SectionHeader("💾  Backup & Restore");

            ConsoleUI.PrintMenu(new[]
            {
                "Full ADB backup  (apps + data)",
                "Backup APKs only  (no data)",
                "Backup selected apps",
                "Pull entire /sdcard",
                "Restore from ADB backup (.ab file)",
                "Push files to device",
            });

            int choice = ConsoleUI.ReadMenuChoice(6);
            switch (choice)
            {
                case 0: return;
                case 1: FullBackup(); break;
                case 2: BackupApks(); break;
                case 3: BackupSelected(); break;
                case 4: PullSdcard(); break;
                case 5: RestoreBackup(); break;
                case 6: PushFiles(); break;
            }
        }
    }

    private static void FullBackup()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("💾  Full ADB Backup");
        ConsoleUI.Warning("Unlock your phone and tap 'Back up my data' when prompted.");
        Console.WriteLine();

        string dir = BackupDir;
        Directory.CreateDirectory(dir);
        string outFile = Path.Combine(dir, "full_backup.ab");

        ConsoleUI.Info($"Saving to: {outFile}");
        Console.WriteLine();

        if (!ConsoleUI.Confirm("Start backup?")) return;

        ConsoleUI.Spinner("Running ADB backup (check your phone screen)...", () =>
        {
            AdbHelper.Run($"backup -apk -shared -all -f \"{outFile}\"", 300000);
        });

        if (File.Exists(outFile) && new FileInfo(outFile).Length > 1024)
            ConsoleUI.Success($"Backup saved: {outFile}");
        else
            ConsoleUI.Error("Backup may have failed or was cancelled on device.");

        ConsoleUI.Pause();
    }

    private static void BackupApks()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("💾  Backup APKs");
        ConsoleUI.Info("Fetching installed app list...");

        List<string> packages = new();
        ConsoleUI.Spinner("Getting packages", () => packages = AdbHelper.GetPackages(true));

        string dir = BackupDir;
        Directory.CreateDirectory(dir);
        int saved = 0;

        ConsoleUI.Info($"Backing up {packages.Count} APKs to {dir}");
        Console.WriteLine();

        for (int i = 0; i < packages.Count; i++)
        {
            string pkg = packages[i];
            ConsoleUI.ProgressBar(i, packages.Count, pkg);

            string pathResult = AdbHelper.Shell($"pm path {pkg}");
            string? apkPath = pathResult
                .Split('\n')
                .FirstOrDefault(l => l.Contains("package:"))
                ?.Replace("package:", "").Trim();

            if (!string.IsNullOrEmpty(apkPath))
            {
                string localFile = Path.Combine(dir, $"{pkg}.apk");
                AdbHelper.PullFile(apkPath, localFile);
                if (File.Exists(localFile)) saved++;
            }
        }

        ConsoleUI.ProgressBar(packages.Count, packages.Count, "Done");
        Console.WriteLine("\n");
        ConsoleUI.Success($"Saved {saved} APKs to {dir}");
        ConsoleUI.Pause();
    }

    private static void BackupSelected()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("💾  Backup Selected Apps");
        ConsoleUI.Info("Loading packages...");

        var packages = AdbHelper.GetPackages(true);
        var selectedIndices = ConsoleUI.MultiSelect(packages, "Select apps to backup");
        if (selectedIndices == null || selectedIndices.Count == 0)
        {
            ConsoleUI.Warning("Nothing selected."); ConsoleUI.Pause(); return;
        }

        string dir = BackupDir;
        Directory.CreateDirectory(dir);
        string outFile = Path.Combine(dir, "selected_backup.ab");
        string pkgArgs = string.Join(" ", selectedIndices.Select(i => packages[i]));

        ConsoleUI.Warning("Check your phone — tap 'Back up my data' when prompted.");
        ConsoleUI.Spinner("Backing up selected apps", () =>
        {
            AdbHelper.Run($"backup -apk -f \"{outFile}\" {pkgArgs}", 300000);
        });

        ConsoleUI.Success($"Backup saved: {outFile}");
        ConsoleUI.Pause();
    }

    private static void PullSdcard()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("💾  Pull /sdcard to PC");

        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sdcard_backup",
            DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        Directory.CreateDirectory(dir);

        ConsoleUI.Info($"Destination: {dir}");
        ConsoleUI.Warning("This may take a very long time depending on storage size.");
        Console.WriteLine();

        if (!ConsoleUI.Confirm("Continue?")) return;

        ConsoleUI.Spinner("Pulling /sdcard (this may take minutes)...", () =>
        {
            AdbHelper.PullFile("/sdcard/.", dir);
        });

        ConsoleUI.Success($"Files pulled to: {dir}");
        ConsoleUI.Pause();
    }

    private static void RestoreBackup()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("♻  Restore ADB Backup");

        string backupFile = ConsoleUI.ReadInput("Path to .ab backup file");
        if (string.IsNullOrEmpty(backupFile) || !File.Exists(backupFile))
        {
            ConsoleUI.Error("File not found."); ConsoleUI.Pause(); return;
        }

        ConsoleUI.Warning("Unlock your phone and tap 'Restore my data' when prompted.");
        Console.WriteLine();
        if (!ConsoleUI.Confirm("Start restore?")) return;

        ConsoleUI.Spinner("Restoring backup (check your phone screen)...", () =>
        {
            AdbHelper.Run($"restore \"{backupFile}\"", 300000);
        });

        ConsoleUI.Success("Restore command sent. Follow prompts on device.");
        ConsoleUI.Pause();
    }

    private static void PushFiles()
    {
        ConsoleUI.PrintBanner();
        ConsoleUI.SectionHeader("📤  Push Files to Device");

        string localPath = ConsoleUI.ReadInput("Local file or folder path");
        if (!File.Exists(localPath) && !Directory.Exists(localPath))
        {
            ConsoleUI.Error("Path not found."); ConsoleUI.Pause(); return;
        }

        string remotePath = ConsoleUI.ReadInput("Destination on device", "/sdcard/");

        ConsoleUI.Spinner($"Pushing to {remotePath}", () =>
        {
            AdbHelper.PushFile(localPath, remotePath);
        });

        ConsoleUI.Success("Push complete.");
        ConsoleUI.Pause();
    }
}
