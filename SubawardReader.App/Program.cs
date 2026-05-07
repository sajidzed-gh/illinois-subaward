//using SubawardReader.Core;
using SubawardBusiness.Core;

// ─────────────────────────────────────────────────────────────────────────────
// Usage:  SubawardReader <folder-path>
//         SubawardReader          (defaults to the current directory)
// ─────────────────────────────────────────────────────────────────────────────

var folderPath = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

if (!Directory.Exists(folderPath))
{
    Console.Error.WriteLine($"Error: folder not found – \"{folderPath}\"");
    return 1;
}

var xlsxFiles = Directory.GetFiles(folderPath, "*.xlsx", SearchOption.TopDirectoryOnly)
                          .OrderBy(f => f)
                          .ToArray();

if (xlsxFiles.Length == 0)
{
    Console.WriteLine($"No .xlsx files found in \"{folderPath}\".");
    return 0;
}

var parser = new SubawardExcelParser();
var allRecords = new List<SubawardRecord>();

// ── Per-file output ─────────────────────────────────────────────────────────
foreach (var file in xlsxFiles)
{
    var records = parser.Parse(file);
    allRecords.AddRange(records);

    Console.WriteLine();
    Console.WriteLine(new string('-', 60));
    Console.WriteLine($"  File: {Path.GetFileName(file)}");
    Console.WriteLine(new string('-', 60));

    if (records.Count == 0)
    {
        Console.WriteLine("  (no subaward rows found)");
    }
    else
    {
        foreach (var rec in records)
            Console.WriteLine($"  * {rec.RecipientName}");
    }
}

// ── Summary across all files ─────────────────────────────────────────────────
Console.WriteLine();
Console.WriteLine(new string('=', 60));
Console.WriteLine("  SUMMARY - Total Subaward Amounts by Recipient");
Console.WriteLine(new string('=', 60));

if (allRecords.Count == 0)
{
    Console.WriteLine("  No subaward records found across all files.");
}
else
{
    var summary = allRecords
        .GroupBy(r => r.RecipientName, StringComparer.OrdinalIgnoreCase)
        .Select(g => (Name: g.Key, Total: g.Sum(r => r.TotalAmount)))
        .OrderByDescending(x => x.Total)
        .ToList();

    int nameWidth = summary.Max(x => x.Name.Length) + 2;

    foreach (var (name, total) in summary)
        Console.WriteLine($"  {name.PadRight(nameWidth)}  ${total:N0}");

    Console.WriteLine(new string('-', 60));
    Console.WriteLine($"  {"Grand Total".PadRight(nameWidth)}  ${summary.Sum(x => x.Total):N0}");
}

Console.WriteLine();

return 0;
