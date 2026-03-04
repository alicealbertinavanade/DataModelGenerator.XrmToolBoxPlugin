using System.Collections.Generic;
using System.Data;
using System.IO;
using ClosedXML.Excel;

namespace DataModelDevOpsExtractor.Service
{
    public static class DataModelExcelExporter
    {
        public static void ExportToExcel(List<string[]> rows, string[] headers, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DataModel");
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }
                for (int r = 0; r < rows.Count; r++)
                {
                    var row = rows[r];
                    for (int c = 0; c < row.Length; c++)
                    {
                        worksheet.Cell(r + 2, c + 1).Value = row[c];
                    }
                }
                workbook.SaveAs(filePath);
            }
        }
    }
}
