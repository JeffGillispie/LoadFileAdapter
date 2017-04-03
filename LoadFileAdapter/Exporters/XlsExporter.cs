using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// An exporter that export a <see cref="DocumentCollection"/> to an Excel file.
    /// </summary>
    public class XlsExporter : IExporter<ExportXlsSettings>
    {
        /// <summary>
        /// Exports an excel file.
        /// </summary>
        /// <param name="args">Excel export settings.</param>
        public void Export(ExportXlsSettings args)
        {
            FileInfo file = args.GetFile();

            ExcelPackage package = new ExcelPackage(file);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
#warning needs implementation
        }
    }
}
