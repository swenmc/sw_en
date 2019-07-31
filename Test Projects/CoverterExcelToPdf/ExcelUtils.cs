using iTextSharp.text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoverterExcelToPdf
{
    public static class ExcelUtils
    {
        public static IList<string> GetColumns(string filePath, string excelWorksheet)
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"{filePath} file not found.");
            }

            var columns = new List<string>();
            using (var package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = null;
                if (string.IsNullOrEmpty(excelWorksheet)) worksheet = package.Workbook.Worksheets.FirstOrDefault();
                else worksheet = package.Workbook.Worksheets[excelWorksheet];

                var startCell = worksheet.Dimension.Start;
                var endCell = worksheet.Dimension.End;

                for (int col = startCell.Column; col <= endCell.Column; col++)
                {
                    var colHeader = string.Empty;
                    if (worksheet.Cells[1, col].Value != null) colHeader = worksheet.Cells[1, col].Value.ToString();
                    
                    columns.Add(colHeader);
                    
                }
            }
            return columns;
        }
    }

    
    public class ExcelToPdfReport
    {
        
        public void Verify_ExcelToPdfReport_Can_Be_Created()
        {
            ExcelData.CreateSampleFile();

            var report = CreateExcelToPdfReport(
                filePath: TestUtils.GetDataFilePath("sample.xlsx"),
                excelWorksheet: "Sheet1");
            TestUtils.VerifyPdfFileIsReadable(report.FileName);
        }

        public IPdfReportData CreateExcelToPdfReport(string filePath, string excelWorksheet)
        {
            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Vahid", Application = "PdfRpt", Keywords = "Test", Subject = "Test Rpt", Title = "Test" });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
                .DefaultFonts(fonts =>
                {
                    //fonts.Path(TestUtils.GetVerdanaFontPath(),
                    //    TestUtils.GetTahomaFontPath());
                    fonts.Size(9);
                    fonts.Color(System.Drawing.Color.Black);
                })
                .PagesFooter(footer =>
                {
                    footer.DefaultFooter(DateTime.Now.ToString("MM/dd/yyyy"));
                })
                .PagesHeader(header =>
                {
                    header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                    header.DefaultHeader(defaultHeader =>
                    {
                        defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                        //defaultHeader.ImagePath(TestUtils.GetImagePath("01.png"));
                        defaultHeader.Message("Excel To Pdf Report");
                    });
                })
                .MainTableTemplate(template =>
                {
                    template.BasicTemplate(BasicTemplate.ClassicTemplate);
                })
                .MainTablePreferences(table =>
                {
                    table.ColumnsWidthsType(TableColumnWidthType.Relative);
                    table.MultipleColumnsPerPage(new MultipleColumnsPerPage
                    {
                        ColumnsGap = 7,
                        ColumnsPerPage = 3,
                        ColumnsWidth = 170,
                        IsRightToLeft = false,
                        TopMargin = 7
                    });
                })
                .MainTableDataSource(dataSource =>
                {
                    dataSource.CustomDataSource(() => new ExcelDataReaderDataSource(filePath, excelWorksheet));
                })
                .MainTableColumns(columns =>
                {
                    columns.AddColumn(column =>
                    {
                        column.PropertyName("rowNo");
                        column.IsRowNumber(true);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(0);
                        column.Width(1);
                        column.HeaderCell("#");
                    });

                    var order = 1;
                    foreach (var columnInfo in ExcelUtils.GetColumns(filePath, excelWorksheet))
                    {
                        columns.AddColumn(column =>
                        {
                            //tato kniznica vie zase exportovat len tabulky, pokial je tam prvy riadok prazdny,tak ma problem

                            if(!string.IsNullOrEmpty(columnInfo)) column.PropertyName(columnInfo);
                            column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                            column.IsVisible(true);
                            column.Order(order++);
                            column.Width(1);
                            column.HeaderCell(columnInfo);
                        });
                    }
                })
                .MainTableEvents(events =>
                {
                    events.DataSourceIsEmpty(message: "There is no data available to display.");
                })
                .Generate(data => data.AsPdfFile(TestUtils.GetOutputFileName()));
        }



        public static class ExcelData
        {
            public static void CreateSampleFile()
            {
                var newFile = new FileInfo(TestUtils.GetDataFilePath("sample.xlsx"));
                if (newFile.Exists)
                {
                    return;
                }

                using (var package = new ExcelPackage(newFile))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    //Add the headers
                    worksheet.Cells[1, 1].Value = "User";
                    worksheet.Cells[1, 2].Value = "Path";

                    //Add some items...
                    worksheet.Cells["A2"].Value = "User 1";
                    worksheet.Cells["B2"].Value = "/path1";

                    worksheet.Cells["A3"].Value = "User 2";
                    worksheet.Cells["B3"].Value = "/path2";

                    worksheet.Cells["A4"].Value = "User 3";
                    worksheet.Cells["B4"].Value = "/path3";

                    worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                    // set some document properties
                    package.Workbook.Properties.Title = "Sample";
                    package.Workbook.Properties.Author = "Vahid";
                    package.Workbook.Properties.Comments = "This is a sample file.";

                    package.Save();
                }
            }
        }

        public class ExcelDataReaderDataSource : IDataSource
        {
            private readonly string _filePath;
            private readonly string _worksheet;

            public ExcelDataReaderDataSource(string filePath, string worksheet)
            {
                _filePath = filePath;
                _worksheet = worksheet;
            }

            public IEnumerable<IList<CellData>> Rows()
            {
                var fileInfo = new FileInfo(_filePath);
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException($"{_filePath} file not found.");
                }

                using (var package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = null;
                    if (string.IsNullOrEmpty(_worksheet)) worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    else worksheet = package.Workbook.Worksheets[_worksheet];
                    var startCell = worksheet.Dimension.Start;
                    var endCell = worksheet.Dimension.End;

                    for (var row = startCell.Row + 1; row < endCell.Row + 1; row++)
                    {
                        var i = 0;
                        var result = new List<CellData>();
                        for (var col = startCell.Column; col <= endCell.Column; col++)
                        {
                            var pdfCellData = new CellData
                            {
                                PropertyName = worksheet.Cells[1, col].Value?.ToString(),
                                PropertyValue = worksheet.Cells[row, col].Value,
                                PropertyIndex = i++
                            };
                            result.Add(pdfCellData);
                        }
                        yield return result;
                    }
                }
            }
        }


        public static class TestUtils
        {
            public static string GetBaseDir()
            {
                return "";
            }

            public static string GetImagePath(string fileName)
            {

                return Path.Combine(GetBaseDir(), "Images", fileName);
            }

            public static string GetDataFilePath(string fileName)
            {

                return Path.Combine(GetBaseDir(), "Data", fileName);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static string GetOutputFileName([CallerMemberName] string methodName = null)
            {
                return Path.Combine(GetOutputFolder(), $"{methodName}.pdf");
            }

            public static string GetOutputFolder()
            {
                var dir = Path.Combine(GetBaseDir(), "App_Data", "out");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }

            public static string GetWingdingFontPath()
            {
                return Path.Combine(GetBaseDir(), "fonts", "wingding.ttf");
            }

            public static string GetTahomaFontPath()
            {
                return Path.Combine(GetBaseDir(), "fonts", "tahoma.ttf");
            }

            public static string GetVerdanaFontPath()
            {
                return Path.Combine(GetBaseDir(), "fonts", "verdana.ttf");
            }

            public static Font GetUnicodeFont(
                        string fontName, string fontFilePath, float size, int style, BaseColor color)
            {
                if (!FontFactory.IsRegistered(fontName))
                {
                    FontFactory.Register(fontFilePath);
                }
                return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, size, style, color);
            }

            public static void VerifyPdfFileIsReadable(byte[] file)
            {
                PdfReader reader = null;
                try
                {
                    reader = new PdfReader(file);
                    var author = reader.Info["Author"] as string;
                    if (string.IsNullOrWhiteSpace(author) || !author.Equals("Vahid"))
                    {
                        throw new InvalidPdfException("This is not a valid PDF file.");
                    }
                }
                finally
                {
                    reader?.Close();
                }
            }

            public static void VerifyPdfFileIsReadable(string filePath)
            {
                VerifyPdfFileIsReadable(File.ReadAllBytes(filePath));
            }
        }

    }
}
