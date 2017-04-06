using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// An exporter that export a <see cref="DocumentCollection"/> to an Excel file.
    /// </summary>
    public class XlsExporter : IExporter<ExportXlsSettings>
    {
        private const string ROOT_REGEX = "^[A-Za-z]:\\\\";

        /// <summary>
        /// Exports an excel file.
        /// </summary>
        /// <param name="args">Excel export settings.</param>
        public void Export(ExportXlsSettings args)
        {            
            FileInfo file = args.GetFile();
            DocumentCollection docs = args.GetDocuments();
            ExportXlsLinkSettings[] links = args.GetLinks();

            if (file.Exists)
                file.Delete();

            ExcelPackage package = new ExcelPackage(file);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
            DataTable dt = getMetaDataTable(args);
            worksheet.Cells[1, 1].LoadFromDataTable(dt, true);
            insertLinks(worksheet, docs, dt, links);
            package.Save();
        }

        /// <summary>
        /// Gets a <see cref="DataTable"/> build from excel export settings.
        /// </summary>
        /// <param name="args">Excel export settings.</param>
        /// <returns>Returns the data table to export.</returns>
        protected DataTable getMetaDataTable(ExportXlsSettings args)
        {
            DocumentCollection docs = args.GetDocuments();
            string[] fields = args.GetExportFields();
            ExportXlsLinkSettings[] links = args.GetLinks();
            DataTable dt = new DataTable();
            fields.ToList().ForEach(f => dt.Columns.Add(f));
            docs.ToList().ForEach(d => dt.Rows.Add(getRowValues(d, fields)));
            insertLinkColumns(dt, links);
            return dt;
        }

        /// <summary>
        /// Gets the row values for construction of a data table.
        /// </summary>
        /// <param name="doc">The target document.</param>
        /// <param name="fields">The fields to include in the row.</param>
        /// <returns>Returns the row values for a data table.</returns>
        protected string[] getRowValues(Document doc, string[] fields)
        {
            List<string> values = new List<string>();

            foreach (string field in fields)
            {
                if (doc.Metadata.ContainsKey(field))
                {
                    values.Add(doc.Metadata[field]);
                }
                else
                {
                    values.Add(String.Empty);
                }
            }
                        
            return values.ToArray();
        }

        /// <summary>
        /// Inserts columns into the output <see cref="DataTable"/> if needed.
        /// </summary>
        /// <param name="dt"><see cref="DataTable"/> containing output data.</param>
        /// <param name="links">Hyperlink settings.</param>
        protected void insertLinkColumns(DataTable dt, ExportXlsLinkSettings[] links)
        {
            foreach (ExportXlsLinkSettings link in links)
            {
                if (!String.IsNullOrWhiteSpace(link.GetDisplayText()))
                {
                    dt.Columns.Add(link.GetDisplayText());
                    dt.Columns[link.GetDisplayText()].SetOrdinal(link.GetColumnIndex());
                }
            }
        }

        /// <summary>
        /// Inserts hyperlinks into an <see cref="ExcelWorksheet"/>.
        /// </summary>
        /// <param name="ws">The target excel worksheet.</param>
        /// <param name="docs">Source documents.</param>
        /// <param name="dt"><see cref="DataTable"/> containing output data.</param>
        /// <param name="links">Hyperlink settings.</param>
        protected void insertLinks(ExcelWorksheet ws, DocumentCollection docs, 
            DataTable dt, ExportXlsLinkSettings[] links)
        {
            for (int row = 0; row < docs.Count; row++)
            {
                Document doc = docs[row];
                insertRowLinks(ws, dt, doc, links, row);
            }
        }

        /// <summary>
        /// Inserts hyperlinks into the specified row.
        /// </summary>
        /// <param name="ws">The target excel worksheet.</param>
        /// <param name="dt"><see cref="DataTable"/> containing output data.</param>
        /// <param name="doc">The target document.</param>
        /// <param name="links">Hyperlink settings.</param>
        /// <param name="row">The target row.</param>
        protected void insertRowLinks(ExcelWorksheet ws, DataTable dt, Document doc, 
            ExportXlsLinkSettings[] links, int row)
        {
            foreach (ExportXlsLinkSettings link in links)
            {
                Representative rep = doc.Representatives
                    .FirstOrDefault(r => r.Type.Equals(link.GetFileType()));

                if (rep != null)
                {
                    string fieldName = (String.IsNullOrWhiteSpace(link.GetDisplayText()))
                        ? dt.Columns[link.GetColumnIndex()].ColumnName
                        : link.GetDisplayText();
                    int col = dt.Columns.IndexOf(dt.Columns[fieldName]);
                    string linkValue = rep.Files.First().Value;

                    Regex regex = new Regex(ROOT_REGEX);
                    Match match = regex.Match(linkValue);
                    
                    if (!match.Success)
                    {
                        linkValue = ".\\" + linkValue.TrimStart('\\');
                    }                    

                    string displayText = (!String.IsNullOrWhiteSpace(link.GetDisplayText()))
                        ? link.GetDisplayText()
                        : (String.IsNullOrWhiteSpace(dt.Rows[row][col].ToString()))
                            ? "Link"
                            : dt.Rows[row][col].ToString();
                    string cellValue = String.Format("HYPERLINK(\"{0}\", \"{1}\")", linkValue, displayText);
                    ws.Cells[row + 2, col + 1].Formula = cellValue;
                    ws.Cells[row + 2, col + 1].Style.Font.Color.SetColor(Color.Blue);
                }
            }
        }
    }
}
