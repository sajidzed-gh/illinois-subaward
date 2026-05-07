using ClosedXML.Excel;
//using SubawardReader.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;




namespace SubawardBusiness.Core
{
    public class SubawardExcelParser
    {
        public List<SubawardRecord> Parse(string filePath)
        {
            var results = new List<SubawardRecord>();
            var fileName = Path.GetFileName(filePath);

            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();

            var rows = worksheet.RowsUsed();
            var totalColIndex = FindTotalColumnIndex(worksheet);

            foreach (var row in rows)
            {
                var cellB = row.Cell(2).Value.ToString().Trim();

                if (cellB.StartsWith("Subaward:", StringComparison.OrdinalIgnoreCase))
                {
                    var name = row.Cell(3).Value.ToString().Trim();
                

                    if (string.IsNullOrWhiteSpace(name))
                        name = "(Unknown)";

                    decimal amount = 0;
                    if (totalColIndex > 0)
                    {
                        var cellValue = row.Cell(totalColIndex).Value;
                        //if (cellValue != null && decimal.TryParse(cellValue.ToString(), out var parsed))
                        if (row.Cell(totalColIndex) != null && decimal.TryParse(cellValue.ToString(), out var parsed))
                            amount = parsed;
                    }

                    results.Add(new SubawardRecord(fileName, name, amount));
                }
            }

            return results;
        }

        private static int FindTotalColumnIndex(IXLWorksheet worksheet)
        {
            var headerRow = worksheet.RowsUsed()
                .FirstOrDefault(r => r.CellsUsed()
                    .Any(c => c.Value.ToString().Equals("Total", StringComparison.OrdinalIgnoreCase)));

            if (headerRow == null)
                return 6; // fallback to column F

            return headerRow.CellsUsed()
                .First(c => c.Value.ToString().Equals("Total", StringComparison.OrdinalIgnoreCase))
                .Address.ColumnNumber;
        }
    }
}