namespace AAKIFTools.Core;

public static class ConsoleUI
{
    // ─── Colors ───────────────────────────────────────────────────────────────
    public static void Print(string text, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }

    public static void PrintLine(string text = "", ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void Success(string msg) => PrintLine($"  ✔  {msg}", ConsoleColor.Green);
    public static void Error(string msg) => PrintLine($"  ✘  {msg}", ConsoleColor.Red);
    public static void Warning(string msg) => PrintLine($"  ⚠  {msg}", ConsoleColor.Yellow);
    public static void Info(string msg) => PrintLine($"  ℹ  {msg}", ConsoleColor.Cyan);
    public static void Dim(string msg) => PrintLine($"     {msg}", ConsoleColor.DarkGray);

    // ─── Header ───────────────────────────────────────────────────────────────
    public static void PrintBanner()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine("  ╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║                                                           ║");
        Console.WriteLine("  ║        █████╗       ████████╗ ██████╗  ██████╗ ██╗       ║");
        Console.WriteLine("  ║       ██╔══██╗         ██╔══╝██╔═══██╗██╔═══██╗██║       ║");
        Console.WriteLine("  ║       ███████║ ─────   ██║   ██║   ██║██║   ██║██║       ║");
        Console.WriteLine("  ║       ██╔══██║         ██║   ██║   ██║██║   ██║██║       ║");
        Console.WriteLine("  ║       ██║  ██║      ████████╗╚██████╔╝╚██████╔╝███████╗  ║");
        Console.WriteLine("  ║       ╚═╝  ╚═╝      ╚═══════╝ ╚═════╝  ╚═════╝ ╚══════╝  ║");
        Console.WriteLine("  ║                                                           ║");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  ║          Android ADB Utility Tool  •  v2.0.0             ║");
        Console.WriteLine("  ║              github.com/princemdaakif/A-TOOL             ║");
        Console.WriteLine("  ╚═══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void SectionHeader(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        string line = new string('─', 55);
        Console.WriteLine($"  ┌{line}┐");
        Console.WriteLine($"  │  {title.PadRight(53)}│");
        Console.WriteLine($"  └{line}┘");
        Console.ResetColor();
        Console.WriteLine();
    }

    // ─── Menu ─────────────────────────────────────────────────────────────────
    public static void PrintMenu(string[] options, string? hint = null)
    {
        for (int i = 0; i < options.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"    [{i + 1}]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  {options[i]}");
        }
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("    [0]  ← Back");
        Console.ResetColor();
        Console.WriteLine();
        if (hint != null) Dim(hint);
    }

    public static int ReadMenuChoice(int max)
    {
        while (true)
        {
            Print("  ➤ Choice: ", ConsoleColor.Yellow);
            string? input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= max)
                return choice;
            Error("Invalid choice. Try again.");
        }
    }

    public static string ReadInput(string prompt, string defaultVal = "")
    {
        Print($"  ➤ {prompt}", ConsoleColor.Yellow);
        if (!string.IsNullOrEmpty(defaultVal))
            Print($" [{defaultVal}]", ConsoleColor.DarkGray);
        Print(": ", ConsoleColor.Yellow);
        string? val = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(val) ? defaultVal : val;
    }

    public static bool Confirm(string prompt)
    {
        Print($"  ➤ {prompt} (y/N): ", ConsoleColor.Yellow);
        string? input = Console.ReadLine()?.Trim().ToLower();
        return input == "y" || input == "yes";
    }

    public static void Pause()
    {
        Console.WriteLine();
        PrintLine("  Press any key to continue...", ConsoleColor.DarkGray);
        Console.ReadKey(true);
    }

    // ─── Progress ─────────────────────────────────────────────────────────────
    public static void ProgressBar(int current, int total, string label = "")
    {
        int barWidth = 40;
        int filled = total == 0 ? 0 : (int)((double)current / total * barWidth);
        string bar = new string('█', filled) + new string('░', barWidth - filled);
        int pct = total == 0 ? 0 : (int)((double)current / total * 100);
        Console.Write($"\r  [{bar}] {pct,3}%  {label,-30}");
    }

    public static void Spinner(string message, Action action)
    {
        char[] spin = { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
        bool done = false;
        int idx = 0;

        var thread = new Thread(() =>
        {
            while (!done)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"\r  {spin[idx % spin.Length]}  {message}   ");
                Console.ResetColor();
                idx++;
                Thread.Sleep(80);
            }
        });
        thread.IsBackground = true;
        thread.Start();

        action();
        done = true;
        thread.Join();
        Console.Write($"\r  ✔  {message,-50}\n");
    }

    // ─── Interactive Checkbox Selection ───────────────────────────────────────
    /// <summary>
    /// Shows a navigable, filterable, checkbox-style list.
    /// Returns indices of selected items. Returns null if user cancelled.
    /// </summary>
    public static List<int>? MultiSelect(
        List<string> items,
        string title,
        string hint = "SPACE=toggle  ENTER=confirm  ESC=cancel  /=filter  A=all  N=none")
    {
        var selected = new HashSet<int>();
        int cursor = 0;
        int scroll = 0;
        string filter = "";
        bool filtering = false;
        int visibleRows = Math.Min(20, Console.WindowHeight - 12);

        while (true)
        {
            // Apply filter
            var filtered = items
                .Select((item, idx) => (item, idx))
                .Where(x => string.IsNullOrEmpty(filter) || x.item.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (cursor >= filtered.Count) cursor = Math.Max(0, filtered.Count - 1);
            if (cursor < scroll) scroll = cursor;
            if (cursor >= scroll + visibleRows) scroll = cursor - visibleRows + 1;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n  ╔══ {title} ══╗");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  {hint}");
            Console.ResetColor();

            if (!string.IsNullOrEmpty(filter) || filtering)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"  Filter: {filter}");
                if (filtering) Console.Write("█");
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  Showing {filtered.Count}/{items.Count}  •  {selected.Count} selected");
            Console.ResetColor();
            Console.WriteLine();

            for (int i = scroll; i < Math.Min(scroll + visibleRows, filtered.Count); i++)
            {
                var (item, origIdx) = filtered[i];
                bool isSel = selected.Contains(origIdx);
                bool isCur = i == cursor;

                if (isCur)
                    Console.BackgroundColor = ConsoleColor.DarkBlue;

                Console.ForegroundColor = isSel ? ConsoleColor.Green : ConsoleColor.White;
                string checkbox = isSel ? "[✔]" : "[ ]";
                Console.WriteLine($"  {checkbox} {item}");
                Console.ResetColor();
            }

            // Scrollbar hint
            if (filtered.Count > visibleRows)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n  ... {scroll + 1}–{Math.Min(scroll + visibleRows, filtered.Count)} of {filtered.Count}");
                Console.ResetColor();
            }

            var key = Console.ReadKey(true);

            if (filtering)
            {
                if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Enter)
                {
                    filtering = false;
                    cursor = 0; scroll = 0;
                }
                else if (key.Key == ConsoleKey.Backspace && filter.Length > 0)
                    filter = filter[..^1];
                else if (!char.IsControl(key.KeyChar))
                    filter += key.KeyChar;
                continue;
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (cursor > 0) cursor--;
                    break;
                case ConsoleKey.DownArrow:
                    if (cursor < filtered.Count - 1) cursor++;
                    break;
                case ConsoleKey.PageUp:
                    cursor = Math.Max(0, cursor - visibleRows);
                    break;
                case ConsoleKey.PageDown:
                    cursor = Math.Min(filtered.Count - 1, cursor + visibleRows);
                    break;
                case ConsoleKey.Spacebar:
                    if (filtered.Count > 0)
                    {
                        int origIdx = filtered[cursor].idx;
                        if (!selected.Remove(origIdx)) selected.Add(origIdx);
                    }
                    break;
                case ConsoleKey.Enter:
                    return selected.OrderBy(x => x).ToList();
                case ConsoleKey.Escape:
                    return null;
                default:
                    if (key.KeyChar == '/')
                    { filtering = true; filter = ""; }
                    else if (key.KeyChar == 'a' || key.KeyChar == 'A')
                    { foreach (var (_, idx) in filtered) selected.Add(idx); }
                    else if (key.KeyChar == 'n' || key.KeyChar == 'N')
                    { foreach (var (_, idx) in filtered) selected.Remove(idx); }
                    break;
            }
        }
    }
}
