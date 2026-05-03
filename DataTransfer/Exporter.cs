using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;


namespace DataTransfer
{
    public class Exporter
    {
        public byte[] ExportToWord(IDictionary<string, string> values)
        {
            Application wordApp = null;
            Documents docs = null;
            Document doc = null;
            Table table = null;
            Word.Range tableRange = null;

            string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.docx");

            try
            {
                wordApp = new Application();
                docs = wordApp.Documents;
                doc = docs.Add();

                tableRange = doc.Range();

                table = doc.Tables.Add(tableRange, values.Count, 2);

                table.Borders.Enable = 1;

                int rowIndex = 1;
                foreach (var item in values)
                {
                    Cell keyCell = table.Cell(rowIndex, 1);
                    keyCell.Range.Text = item.Key;
                    keyCell.Range.Font.Bold = 1;

                    Cell valueCell = table.Cell(rowIndex, 2);
                    valueCell.Range.Text = item.Value;

                    Marshal.ReleaseComObject(keyCell);
                    Marshal.ReleaseComObject(valueCell);

                    rowIndex++;
                }

                table.Columns.AutoFit();

                doc.SaveAs2(tempFile);
                doc.Close(false);
                wordApp.Quit();

                return File.ReadAllBytes(tempFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Word Table Export failed: {ex.Message}");
            }
            finally
            {
                if (File.Exists(tempFile)) try { File.Delete(tempFile); } catch { }

                if (table != null) Marshal.ReleaseComObject(table);
                if (tableRange != null) Marshal.ReleaseComObject(tableRange);
                if (doc != null) Marshal.ReleaseComObject(doc);
                if (docs != null) Marshal.ReleaseComObject(docs);
                if (wordApp != null) Marshal.ReleaseComObject(wordApp);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public byte[] ExportToExcel(IDictionary<string, string> values)
        {
            Excel.Application excelApp = null;
            Excel.Workbooks workbooks = null;
            Excel.Workbook workbook = null;
            Excel.Sheets sheets = null;
            Excel.Worksheet sheet = null;
            Excel.Range cells = null;

            string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");

            try
            {
                excelApp = new Excel.Application();
                excelApp.DisplayAlerts = false;

                workbooks = excelApp.Workbooks;
                workbook = workbooks.Add();
                sheets = workbook.Sheets;
                sheet = (Excel.Worksheet)sheets.Item[1];
                cells = sheet.Cells;

                int row = 1;
                foreach (var item in values)
                {
                    cells[row, 1] = item.Key;
                    cells[row, 2] = item.Value;

                    row++;
                }

                workbook.SaveAs(tempFile);

                workbook.Close(false);
                excelApp.Quit();

                return File.ReadAllBytes(tempFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Excel Export failed: {ex.Message}");
            }
            finally
            {
                if (File.Exists(tempFile)) try { File.Delete(tempFile); } catch { }

                if (cells != null) Marshal.ReleaseComObject(cells);
                if (sheet != null) Marshal.ReleaseComObject(sheet);
                if (sheets != null) Marshal.ReleaseComObject(sheets);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (workbooks != null) Marshal.ReleaseComObject(workbooks);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
