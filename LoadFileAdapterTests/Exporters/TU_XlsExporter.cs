using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Exporters;
using LoadFileAdapter.Parsers;
using Moq;
using OfficeOpenXml;

namespace LoadFileAdapterTests.Exporters
{
    /// <summary>
    /// Tests the <see cref="XlsExporter"/> class.
    /// </summary>
    [TestClass]
    public class TU_XlsExporter
    {
        private List<string> datLines;
        private DocumentCollection docs;
                
        class TestExporter : XlsExporter
        {
            public TestExporter()
            {

            }

            public TestExporter(FileInfo file, string[] fields, HyperLinkInfo[] links)
            {
                base.file = file;
                base.exportFields = fields;
                base.links = links;
            }

            public new DataTable getMetaDataTable(DocumentCollection docs)
            {
                return base.getMetaDataTable(docs);
            }

            public new string[] getRowValues(Document doc, string[] fields)
            {
                return base.getRowValues(doc, fields);
            }

            public new void insertLinkColumns(DataTable dt, HyperLinkInfo[] links)
            {
                base.insertLinkColumns(dt, links);
            }

            public new void insertLinks(ExcelWorksheet ws, DocumentCollection docs, DataTable dt)
            {
                base.insertLinks(ws, docs, dt);
            }

            public new void insertRowLinks(ExcelWorksheet ws, DataTable dt, Document doc, int row)
            {
                base.insertRowLinks(ws, dt, doc, row);
            }
        }

        [TestInitialize()]
        public void TestSetup()
        {
            datLines = new List<string>(new string[] {
                "þDOCIDþþBEGATTþþVOLUMEþþNATIVEþ",
                "þDOC000001þþDOC000001þþVOL001þþX:\\VOL001\\NATIVE\\0001\\DOC000001.XLSXþ",
                "þDOC000002þþDOC000001þþVOL001þþþ",
                null,null
            });
            var mockReader = new Mock<TextReader>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => datLines[calls])
                .Callback(() => calls++);            
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.dat");
            bool hasHeader = true;
            string keyColName = "DOCID";
            string parentColName = "BEGATT";
            string childColName = String.Empty;
            string childColDelim = ";";
            RepresentativeBuilder repSetting = new RepresentativeBuilder("NATIVE", Representative.FileType.Native);
            List<RepresentativeBuilder> reps = new List<RepresentativeBuilder>();
            reps.Add(repSetting);
            var builder = new DatBuilder();
            IParser parser = new DatParser(Delimiters.CONCORDANCE);
            List<string[]> records = parser.Parse(mockReader.Object);
            builder.HasHeader = hasHeader;
            builder.KeyColumnName = keyColName;
            builder.ParentColumnName = parentColName;
            builder.ChildColumnName = childColName;
            builder.ChildSeparator = childColDelim;
            builder.RepresentativeBuilders = reps;
            builder.ParentColumnName = infile.Directory.FullName;            
            List<Document> documents = builder.Build(records);
            docs = new DocumentCollection(documents);
        }
        
        [TestMethod]
        public void Exporters_XlsExporter_getMetaDataTable()
        {
            FileInfo outfile = new FileInfo(@"X:\noWhere.xlsx");
            string[] exportFields = new string[] { "DOCID", "BEGATT", "VOLUME", "NATIVE" };
            HyperLinkInfo[] links = new HyperLinkInfo[] { };            
            TestExporter tester = new TestExporter(outfile, exportFields, links);
            DataTable dt = tester.getMetaDataTable(this.docs);
            
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string expected = exportFields[i];
                string actual = dt.Columns[i].ColumnName;
                Assert.AreEqual(expected, actual);
            }

            for (int row = 0; row < dt.Rows.Count; row++)
            {
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    string expected = docs[row].Metadata[dt.Columns[col].ColumnName];
                    string actual = dt.Rows[row][col].ToString();
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        [TestMethod]
        public void Exporters_XlsExporter_getRowValues()
        {
            Document doc = docs.First();
            string[] fields = new string[] { "DOCID", "VOLUME" };
            string[] expected = new string[] { "DOC000001", "VOL001" };

            TestExporter tester = new TestExporter();
            string[] values = tester.getRowValues(doc, fields);
            
            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(expected[i], values[i]);
            }
        }

        [TestMethod]
        public void Exporters_XlsExporter_insertLinkColumns()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("one");
            dt.Columns.Add("two");
            List<HyperLinkInfo> links = new List<HyperLinkInfo>();
            Representative.FileType type = Representative.FileType.Native;
            string display = "display text";
            int index = 1;
            HyperLinkInfo link = new HyperLinkInfo(type, display, index);
            links.Add(link);

            TestExporter tester = new TestExporter();
            tester.insertLinkColumns(dt, links.ToArray());
            Assert.AreEqual(dt.Columns[index].ColumnName, display);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Exporters_XlsExporter_insertLinkColumns_ExLinkIndex()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("one");
            dt.Columns.Add("two");
            List<HyperLinkInfo> links = new List<HyperLinkInfo>();
            Representative.FileType type = Representative.FileType.Native;
            string display = "display text";
            int index = 5;
            HyperLinkInfo link = new HyperLinkInfo(type, display, index);
            links.Add(link);

            TestExporter tester = new TestExporter();
            tester.insertLinkColumns(dt, links.ToArray());
        }

        [TestMethod]
        public void Exporters_XlsExporter_insertLinkColumns_NullName()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("one");
            dt.Columns.Add("two");
            List<HyperLinkInfo> links = new List<HyperLinkInfo>();
            Representative.FileType type = Representative.FileType.Native;
            string display = null;
            int index = 1;
            HyperLinkInfo link = new HyperLinkInfo(type, display, index);
            links.Add(link);

            TestExporter tester = new TestExporter();
            tester.insertLinkColumns(dt, links.ToArray());
            Assert.AreEqual(dt.Columns[index].ColumnName, "two");
        }

        [TestMethod]
        public void Exporters_XlsExporter_insertLinkColumns_NoLinks()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("one");
            dt.Columns.Add("two");
            
            TestExporter tester = new TestExporter();
            tester.insertLinkColumns(dt, null);
            Assert.AreEqual("one", dt.Columns[0].ColumnName);
            Assert.AreEqual("two", dt.Columns[1].ColumnName);
            Assert.AreEqual(2, dt.Columns.Count);
        }

        [TestMethod]
        public void Exporters_XlsExporter_insertRowLinks_DisplayText()
        {
            FileInfo outfile = new FileInfo(@"X:\noWhere.xlsx");
            string[] exportFields = new string[] { "DOCID", "BEGATT", "VOLUME", "NATIVE" };
            Representative.FileType type = Representative.FileType.Native;
            string display = "display text";
            int index = 1;
            HyperLinkInfo link = new HyperLinkInfo(type, display, index);
            HyperLinkInfo[] links = new HyperLinkInfo[] { link };            
            TestExporter tester = new TestExporter(outfile, exportFields, links);
            DataTable dt = tester.getMetaDataTable(this.docs);
            FileInfo file = new FileInfo("test.xlsx");
            ExcelPackage package = new ExcelPackage(file);
            ExcelWorksheet ws = package.Workbook.Worksheets.Add("Sheet1");
            ws.Cells[1, 1].LoadFromDataTable(dt, true);            
            Document doc = docs.First();            
            int row = 0;
            int col = 1;
            string linkValue = @"X:\VOL001\NATIVE\0001\DOC000001.XLSX";
            tester.insertRowLinks(ws, dt, doc, row);
            string expected = String.Format("HYPERLINK(\"{0}\", \"{1}\")", linkValue, display);
            string actual = ws.Cells[row + 2, col + 1].Formula;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Exporters_XlsExporter_insertRowLinks_LinkIndex()
        {
            FileInfo outfile = new FileInfo(@"X:\noWhere.xlsx");
            string[] exportFields = new string[] { "DOCID", "BEGATT", "VOLUME", "NATIVE" };
            Representative.FileType type = Representative.FileType.Native;
            string display = null;
            int index = 3;            
            HyperLinkInfo link = new HyperLinkInfo(type, display, index);
            HyperLinkInfo[] links = new HyperLinkInfo[] { link };            
            TestExporter tester = new TestExporter(outfile, exportFields, links);
            DataTable dt = tester.getMetaDataTable(this.docs);
            FileInfo file = new FileInfo("test.xlsx");
            ExcelPackage package = new ExcelPackage(file);
            ExcelWorksheet ws = package.Workbook.Worksheets.Add("Sheet1");
            ws.Cells[1, 1].LoadFromDataTable(dt, true);
            Document doc = docs.First();
            int row = 0;
            int col = 3;
            string linkValue = @"X:\VOL001\NATIVE\0001\DOC000001.XLSX";
            tester.insertRowLinks(ws, dt, doc, row);
            string expected = String.Format("HYPERLINK(\"{0}\", \"{1}\")", linkValue, linkValue);
            string actual = ws.Cells[row + 2, col + 1].Formula;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Exporters_XlsExporter_insertRowLinks_RelativeLink()
        {
            FileInfo outfile = new FileInfo(@"X:\noWhere.xlsx");
            string[] exportFields = new string[] { "DOCID", "BEGATT", "VOLUME", "NATIVE" };
            Representative.FileType type = Representative.FileType.Native;
            string display = null;
            int index = 3;            
            docs.First().Metadata["NATIVE"] = @"\NATIVE\0001\DOC000001.XLSX";
            docs.First().Representatives.First().Files["DOC000001"] = @"\NATIVE\0001\DOC000001.XLSX";            
            HyperLinkInfo link = new HyperLinkInfo(type, display, index);
            HyperLinkInfo[] links = new HyperLinkInfo[] { link };            
            TestExporter tester = new TestExporter(outfile, exportFields, links);
            DataTable dt = tester.getMetaDataTable(this.docs);            
            FileInfo file = new FileInfo("test.xlsx");
            ExcelPackage package = new ExcelPackage(file);
            ExcelWorksheet ws = package.Workbook.Worksheets.Add("Sheet1");
            ws.Cells[1, 1].LoadFromDataTable(dt, true);
            Document doc = docs.First();
            int row = 0;
            int col = 3;
            string linkValue = @".\NATIVE\0001\DOC000001.XLSX";
            tester.insertRowLinks(ws, dt, doc, row);
            string expected = String.Format(
                "HYPERLINK(\"{0}\", \"{1}\")", linkValue, "\\NATIVE\\0001\\DOC000001.XLSX");
            string actual = ws.Cells[row + 2, col + 1].Formula;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Exporters_XlsExporter_insertRowLinks_NoLink()
        {
            FileInfo outfile = new FileInfo(@"X:\noWhere.xlsx");
            string[] exportFields = new string[] { "DOCID", "BEGATT", "VOLUME", "NATIVE" };
            Representative.FileType type = Representative.FileType.Native;
            string display = null;
            int index = 3;
            HyperLinkInfo link = new HyperLinkInfo(type, display, index);
            HyperLinkInfo[] links = new HyperLinkInfo[] { link };            
            TestExporter tester = new TestExporter(outfile, exportFields, links);
            DataTable dt = tester.getMetaDataTable(this.docs);
            FileInfo file = new FileInfo("test.xlsx");
            ExcelPackage package = new ExcelPackage(file);
            ExcelWorksheet ws = package.Workbook.Worksheets.Add("Sheet1");
            ws.Cells[1, 1].LoadFromDataTable(dt, true);
            Document doc = docs[1];
            int row = 0;
            int col = 3;            
            tester.insertRowLinks(ws, dt, doc, row);
            string expected = String.Empty;
            string actual = ws.Cells[row + 2, col + 1].Formula;
            Assert.AreEqual(expected, actual);
        }
    }
}
