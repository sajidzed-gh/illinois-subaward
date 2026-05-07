namespace SubawardBusiness.Core;

/// <summary>
/// Represents a single subaward entry parsed from a budget spreadsheet.
/// </summary>
public record SubawardRecord(
    string FileName,
    string RecipientName,
    decimal TotalAmount
);
