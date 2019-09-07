using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MaxSu.Framework.Common;
using OfficeOpenXml;



namespace Lazyman
{
    public static class ExcelUtils
    {

        public static void WriteExcelV2(string conn, string filename, string sql, string templateName)
        {
            string filePathTemplate = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory) + templateName;
            ExcelPackage pck = null;
            DataSet result = null;
            try
            {
                File.Copy(filePathTemplate, filename);
                File.SetCreationTime(filename, DateTime.Now);

                var newFile = new FileInfo(filename);

                pck = new ExcelPackage(newFile);
                ExcelWorkbook oDoc = pck.Workbook;

                var bankingInfo = oDoc.Worksheets["Sheet1"];
                result = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    bankingInfo.Cells[2, 1].LoadFromDataTable(result.Tables[0], false);
                }

                pck.Save();
            }
            catch (Exception oEx)
            {
                throw new Exception(oEx.Message);
            }
            finally
            {
                if (result!=null)
                    result.Dispose();
                if(pck !=null)
                    pck.Dispose();
            }
        }
    }
}