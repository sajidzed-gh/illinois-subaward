
using SubawardBusiness.Core;
using Xunit;

namespace SubawardReader.Tests;

public class SubawardExcelParserTests
{
    private static string TestDataPath => Path.Combine(
        AppContext.BaseDirectory, "TestData");

    private static string FilePath(string name) =>
        Path.Combine(TestDataPath, name);

    // ------------------------------------------------------------------------------
    // Required test: confirms exactly four recipients 
    // Indiana, Mayo, Purdue, and Florida are all present.
    // ------------------------------------------------------------------------------
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

    [Fact]
    public void Example2_No_Subaward()
    {

        var parser = new SubawardExcelParser();
        var records = parser.Parse(FilePath("SubawardBudgetExampleZero.xlsx"));

        Assert.Empty(records);
            
        
    }

    
    
}
