using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Excel;
using System.Diagnostics;

namespace SharedLibraries.EXPIMP
{
	public static class CImportFromExcel
	{
		public static DataSet ImportFromExcel(string fileName)
		{
			DataSet result = null;
			try
			{
				FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
				IExcelDataReader excelReader = null;
				if (fileName.EndsWith(".xls")) 
				{
					// Reading from a binary Excel file ('97-2003 format; *.xls)
					excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
				}
				else if(fileName.EndsWith(".xlsx"))
				{
					//Reading from a OpenXml Excel file (2007 format; *.xlsx)
					excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
				}
				
				//3. DataSet - The result of each spreadsheet will be created in the result.Tables
				//DataSet result = excelReader.AsDataSet();
				//...
				//4. DataSet - Create column names from first row
				excelReader.IsFirstRowAsColumnNames = true;
				result = excelReader.AsDataSet();

				//5. Data Reader methods
				//while (excelReader.Read())
				//{
				//    //excelReader.GetInt32(0);
				//}

				//6. Free resources (IExcelDataReader is IDisposable)
				excelReader.Close();
			}
			catch (Exception ex)
			{
                if (!EventLog.SourceExists("sw_en"))
				{
					EventLog.CreateEventSource("sw_en", "Application");
				}
				EventLog.WriteEntry("sw_en", ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
			}
			return result;
		}
	}
}
