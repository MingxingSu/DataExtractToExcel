using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MaxSu.Framework.Common;

namespace Lazyman
{
    class Program
    {
        static void Main(string[] args)
        {

            Dictionary<string, string> connStrDict = GetConnectionStrDictionary();

            ReportSection reportSection =(ReportSection)System.Configuration.ConfigurationManager.GetSection("reportGroup/report");
            ShowSupportedReports(reportSection);

            Console.WriteLine("Type the report name and press enter:");
            string reportName = Console.ReadLine();

            foreach (ReportElement report in reportSection.ReportElementCollection)
            {
                if (report.ReportName.ToLower() == reportName.ToLower())
                {
                    if (!string.IsNullOrWhiteSpace(report.DbList))
                    {
                        string[] dbList = report.DbList.Split(',');
                        foreach (string dbShortName in dbList)
                        {
                            GenerateReports(report,connStrDict[dbShortName],dbShortName);
                        }
                    }
                    break;
                }
            }

            Console.WriteLine("Done.");

            Console.ReadKey();
        }

        private static void GenerateReports(ReportElement report,string connStr,string dbShortName)
        {
            if (string.IsNullOrWhiteSpace(report.SqlParameters))
            {
                SaveDataToExcel(report, connStr, dbShortName, string.Empty,string.Empty);
            }
            else
            {
                string[] parallelParameters = report.SqlParameters.Split('|');
                string[] parameterNames = report.ParameterNames.Split('|');
                if (parallelParameters.Length != parameterNames.Length)
                {
                    Console.WriteLine("SqlParameters not matched with ParameterNames");
                    return;
                }
                for (int i=0;i < parallelParameters.Length ; i ++)
                {
                    SaveDataToExcel(report, connStr, dbShortName, parallelParameters[i], parameterNames[i]);
                }
            }
        }

        private static void SaveDataToExcel(ReportElement report, string connStr, string dbShortName, string parameter,string parameterName)
        {
            string formattedSql = GetFormattedSqlScrit(report.SqlScript, parameter);

            string resultReportName =  Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                (report.ReportName + "_" + dbShortName + (string.IsNullOrEmpty(parameter) ? "" : "_" + parameterName)) + ".xlsx");

            //Save data
            ExcelUtils.WriteExcelV2(connStr, resultReportName,formattedSql, report.TemplateName);
        }

        private static void ShowSupportedReports(ReportSection reportSection)
        {
            //print reports names to console
            Console.WriteLine("Supported Reports:");

            foreach (var report in reportSection.ReportElementCollection)
            {
                Console.WriteLine(report.ReportName);
            }
        }

        //Save all DB connection string to Dictionionary so that can be easily lookup
        private static Dictionary<string, string> GetConnectionStrDictionary()
        {
            Dictionary<string, string> connStrDict = new Dictionary<string, string>();
            ConnectionStringsSection conenctStrs =
                (ConnectionStringsSection) System.Configuration.ConfigurationManager.GetSection("connectionStrings");
            foreach (ConnectionStringSettings connSetting in conenctStrs.ConnectionStrings)
            {
                if (!connStrDict.ContainsKey(connSetting.Name))
                    connStrDict.Add(connSetting.Name, connSetting.ConnectionString);
            }
            return connStrDict;
        }


        /// <summary>
        /// Get SQL script with parameters specified
        /// </summary>
        /// <param name="sqlScriptFile">the file contains SQL query to extract data </param>
        /// <param name="sqlParameters">Coma seperated string, be used as SQL Paramter in script file, make sure in order</param>
        /// <returns></returns>
        private static string GetFormattedSqlScrit(string sqlScriptFile,string sqlParameters)
        {
            string finalSQL = "";
            if (!String.IsNullOrEmpty(sqlScriptFile) && File.Exists(sqlScriptFile))
            {
                finalSQL = File.ReadAllText(sqlScriptFile);
            }

            if (!String.IsNullOrEmpty(sqlParameters))
            {
                string[] parameters = sqlParameters.Split(',');
                finalSQL = String.Format(finalSQL, parameters);
            }
            return finalSQL;
        }
    }
}
