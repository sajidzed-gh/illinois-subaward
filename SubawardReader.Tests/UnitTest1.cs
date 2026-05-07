using SubawardReader.Core;

namespace SubawardReader.Tests;

public class SubawardExcelParserTests
{
    private static string TestDataPath => Path.Combine(
        AppContext.BaseDirectory, "TestData");

    private static string FilePath(string name) =>
        Path.Combine(TestDataPath, name);

    // ──────────────────────────────────────────────────────────────────────────
    // Required test: confirms exactly four recipients in Example1 and that
    // Indiana, Mayo, Purdue, and Florida are all present.
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public void Example1_HasFourSubrecipients_Indiana_Mayo_Purdue_Florida()
    {
        var parser = new SubawardExcelParser();
        var records = parser.Parse(FilePath("SubawardBudgetExample1.xlsx"));

        var names = records.Select(r => r.RecipientName).ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Equal(4, records.Count);
        Assert.Contains("Indiana", names);
        Assert.Contains("Mayo", names);
        Assert.Contains("Purdue", names);
        Assert.Contains("Florida", names);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Verify total amounts for Example1 recipients
    // (subaward row total + exempt row total, read from the "Total" column)
    // ──────────────────────────────────────────────────────────────────────────
    [Theory]
    [InlineData("Indiana", 126_904)]  // 25000 + 101904
    [InlineData("Mayo",     20_637)]  // 20637 + 0
    [InlineData("Purdue",   64_101)]  // 25000 + 39101
    [InlineData("Florida",  74_423)]  // 25000 + 49423
    public void Example1_SubrecipientTotals_AreCorrect(string recipient, decimal expectedTotal)
    {
        var parser = new SubawardExcelParser();
        var records = parser.Parse(FilePath("SubawardBudgetExample1.xlsx"));

        var actual = records
            .Where(r => r.RecipientName.Equals(recipient, StringComparison.OrdinalIgnoreCase))
            .Sum(r => r.TotalAmount);

        Assert.Equal(expectedTotal, actual);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Example2: three subaward rows (Ecotek, Purdue, Mayo)
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public void Example2_HasThreeSubrecipients()
    {
        var parser = new SubawardExcelParser();
        var records = parser.Parse(FilePath("SubawardBudgetExample2.xlsx"));

        Assert.Equal(3, records.Count);
    }

    [Theory]
    [InlineData("Ecotek",  80_000)]   // 25000 + 55000
    [InlineData("Purdue",  20_000)]   // 20000 + 0
    [InlineData("Mayo",    19_782)]   // 19782 + 0
    public void Example2_SubrecipientTotals_AreCorrect(string recipient, decimal expectedTotal)
    {
        var parser = new SubawardExcelParser();
        var records = parser.Parse(FilePath("SubawardBudgetExample2.xlsx"));

        var actual = records
            .Where(r => r.RecipientName.Equals(recipient, StringComparison.OrdinalIgnoreCase))
            .Sum(r => r.TotalAmount);

        Assert.Equal(expectedTotal, actual);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Example3: four subaward rows (U WA, U CO, unknown, Mayo)
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public void Example3_HasFourSubawardRows()
    {
        var parser = new SubawardExcelParser();
        var records = parser.Parse(FilePath("SubawardBudgetExample3.xlsx"));

        Assert.Equal(4, records.Count);
    }

    [Theory]
    [InlineData("U WA",  203_859)]  // 25000 + 178859
    [InlineData("U CO",  132_877)]  // 25000 + 107877
    [InlineData("Mayo",   92_154)]  // 25000 + 67154
    public void Example3_NamedSubrecipientTotals_AreCorrect(string recipient, decimal expectedTotal)
    {
        var parser = new SubawardExcelParser();
        var records = parser.Parse(FilePath("SubawardBudgetExample3.xlsx"));

        var actual = records
            .Where(r => r.RecipientName.Equals(recipient, StringComparison.OrdinalIgnoreCase))
            .Sum(r => r.TotalAmount);

        Assert.Equal(expectedTotal, actual);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Each record carries the correct source file name
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public void ParsedRecords_HaveCorrectFileName()
    {
        var parser = new SubawardExcelParser();
        var records = parser.Parse(FilePath("SubawardBudgetExample1.xlsx"));

        Assert.All(records, r =>
            Assert.Equal("SubawardBudgetExample1.xlsx", r.FileName));
    }
}
