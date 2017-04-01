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
        /// <param name="x"></param>
        public void Export(ExportXlsSettings x)
        {
#warning needs implementation
        }
    }
}
