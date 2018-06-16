using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
//using Microsoft.Office.Interop.Excel; // Problem s nacitanim - TODO - prepisat kod tak, aby to nebolo zavisle na tom ci je alebo nie je nainstalovany MS Office
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace SharedLibraries.EXPIMP
{
	public static class CExportToExcel
	{
		public static void ExportToExcel(DataSet myDataSet, string fileName)
		{
			if (myDataSet == null)
			{
				throw new ArgumentNullException("myDataSet");
			}

			//var excel = new Application();

			try
			{
				/*Workbook book;

				if (File.Exists(fileName))
				{
					book = excel.Workbooks.Open(fileName);
				}
				else
				{
					book = excel.Workbooks.Add();
				}

				for (int t = myDataSet.Tables.Count - 1; t >= 0; t--)
				{
					dynamic sheet = book.Sheets.Add();

					System.Data.DataTable table = myDataSet.Tables[t];
					sheet.Name = table.TableName;

					for (int i = 0; i < table.Columns.Count; i++)
					{
						sheet.Cells[1, i + 1] = table.Columns[i].ColumnName;
						sheet.Cells[1, i + 1].Font.Bold = true;
						sheet.Cells[1, i + 1].Interior.Color = ColorTranslator.ToOle(Color.Gray);
					}

					for (int row = 0; row < table.Rows.Count; row++)
					{
						for (int column = 0; column < table.Columns.Count; column++)
						{
							sheet.Cells[row + 2, column + 1] =
								table.Rows[row][column].ToString();
						}
					}

					for (int i = 0; i < table.Columns.Count; i++)
					{
						sheet.Columns[i + 1].AutoFit();
					}
				}
				book.SaveAs(fileName, AccessMode: XlSaveAsAccessMode.xlShared);
				book.Close();*/
			}
			catch (Exception ex) 
			{
				if (!EventLog.SourceExists("sw_en"))
				{
					EventLog.CreateEventSource("sw_en", "Application");
				}
				EventLog.WriteEntry("sw_en", ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
			}
			finally
			{
				//excel.Workbooks.Close();
				//excel.Quit();
			}
		}
		
	}
}
