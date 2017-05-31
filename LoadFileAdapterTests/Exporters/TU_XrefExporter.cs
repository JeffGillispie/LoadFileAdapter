using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using LoadFileAdapter;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Exporters;
using LoadFileAdapter.Instructions;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapterTests.Exporters
{
    [TestClass]
    public class TU_XrefExporter
    {
        public class TestExporter : XrefExporter
        {
            public new string getCustomValue(string imageKey, Document doc, string customValueField)
            {
                return base.getCustomValue(imageKey, doc, customValueField);
            }

            public new bool getCodeEndFlag(int docIndex, int imageIndex, IExportXrefSettings settings)
            {
                return base.getCodeEndFlag(docIndex, imageIndex, settings);
            }

            public new bool getGroupEndFlag(int docIndex, int imageIndex, IExportXrefSettings settings)
            {
                return base.getGroupEndFlag(docIndex, imageIndex, settings);
            }

            public new bool isFlagNeeded(Document doc, XrefTrigger trigger, Document previousDoc)
            {
                return base.isFlagNeeded(doc, trigger, previousDoc);
            }

            public static bool hasFieldValueChanged(Document doc, Document previousDoc, XrefTrigger trigger)
            {
                return TestExporter.hasFieldValueChange(doc, previousDoc, trigger);
            }

            public new Document getNextDoc(int index)
            {
                return base.getNextDoc(index);
            }

            public new Document getPreviousDoc(int index)
            {
                return base.getPreviousDoc(index);
            }

            public new static string boolToString(bool b)
            {
                return XrefExporter.boolToString(b);
            }

            public new string getNextImageKey(int imgIndex, int docIndex)
            {
                return base.getNextImageKey(imgIndex, docIndex);
            }

            public new string[] getRecordComponents(IExportXrefSettings args, int docIndex, int imageIndex)
            {
                return base.getRecordComponents(args, docIndex, imageIndex);
            }

            public new string getGhostBoxLine(string imageKey, string pageRecord, XrefTrigger boxTrigger, int docIndex)
            {
                return base.getGhostBoxLine(imageKey, pageRecord, boxTrigger, docIndex);
            }

            public new List<string> getPageRecords(IExportXrefSettings args, int docIndex, SlipSheets slipsheets)
            {
                return base.getPageRecords(args, docIndex, slipsheets);
            }

            public void SetBoxNo(int n)
            {
                base.boxNumber = n;
            }

            public void SetWaitingForFlag(bool flag)
            {
                base.waitingForCodeEnd = flag;
                base.waitingForGroupEnd = flag;
            }

            public void SetDocs(DocumentCollection docs)
            {
                base.docs = docs;
            }
        }

        public DocumentCollection GetDocs()
        {
            var datLines = new List<string>(new string[] {
                "þDOCIDþþBEGATTþþVOLUMEþþDOCTYPEþþNATIVEþ",
                "þDOC000001þþDOC000001þþVOL001þþEMAILþþX:\\VOL001\\NATIVE\\0001\\DOC000001.XLSXþ",
                "þDOC000002þþDOC000001þþVOL001þþPDFþþþ",
                null,null
            });

            var optLines = new List<string[]> { 
                new string[] { "DOC000001", "VOL001","X:\\VOL001\\IMAGES\\0001\\DOC000001.jpg","Y","","","1" },
                new string[] { "DOC000002", "VOL001","X:\\VOL001\\IMAGES\\0001\\DOC000002.tif","Y","","","2" },
                new string[] { "DOC000003", "VOL001","X:\\VOL001\\IMAGES\\0001\\DOC000003.tif","","","","" }
            };
            
            var mockReader = new Mock<TextReader>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => datLines[calls])
                .Callback(() => calls++);
            Delimiters delimiters = Delimiters.CONCORDANCE;
            ParseReaderDatSettings readArgs = new ParseReaderDatSettings(mockReader.Object, delimiters);
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.dat");
            bool hasHeader = true;
            string keyColName = "DOCID";
            string parentColName = "BEGATT";
            string childColName = String.Empty;
            string childColDelim = ";";
            DatRepresentativeSettings repSetting = new DatRepresentativeSettings("NATIVE", Representative.FileType.Native);
            List<DatRepresentativeSettings> reps = new List<DatRepresentativeSettings>();
            reps.Add(repSetting);
            IBuilder<BuildDocCollectionDatSettings, BuildDocDatSettings> builder = new DatBuilder();
            IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings> parser = new DatParser();
            List<string[]> records = parser.Parse(readArgs);
            BuildDocCollectionDatSettings buildArgs = new BuildDocCollectionDatSettings(
                records, infile.Directory.FullName, hasHeader, keyColName, parentColName, childColName, childColDelim, reps);
            List<Document> documents = builder.BuildDocuments(buildArgs);
            var docs = new DocumentCollection(documents);
            IBuilder<BuildDocCollectionImageSettings, BuildDocImageSettings> optBuilder = new OptBuilder();
            BuildDocCollectionImageSettings args = new BuildDocCollectionImageSettings(optLines, String.Empty, null);            
            List<Document> optDocs = optBuilder.BuildDocuments(args);
            docs.AddRange(optDocs);
            docs[1].SetParent(docs[0]);
            return docs;
        }

        [TestMethod]
        public void Exporters_XrefExporter_getCustomValue()
        {
            string key1 = "DOC000123";
            string key2 = "DOC000124";
            string field = "DocType";
            string value = "Email";
            Document doc = new Document(key1, null, null, new Dictionary<string, string>() { { field, value } }, null);
            TestExporter exporter = new TestExporter();            
            Assert.AreEqual(value, exporter.getCustomValue(key1, doc, field));            
            Assert.AreEqual(String.Empty, exporter.getCustomValue(key2, doc, field));
            Assert.AreEqual(String.Empty, exporter.getCustomValue(key1, doc, null));
            Assert.AreEqual(String.Empty, exporter.getCustomValue(key1, doc, String.Empty));
            Assert.AreEqual(String.Empty, exporter.getCustomValue(key1, doc, "bananas"));
        }

        [TestMethod]
        public void Exporters_XrefExporter_getEndFlag()
        {
            var docs = GetDocs();
            int docIndex = 1;
            int imgIndex = 0;
            var settings = new XrefExport();
            Trigger trigger = new Trigger();
            trigger.Type = XrefTrigger.TriggerType.FieldValueChange;
            trigger.FieldName = "DOCTYPE";
            trigger.FieldChangeOption = XrefTrigger.FieldValueChangeOption.None;            
            settings.CodeStartTrigger = trigger;
            var args = settings.GetFileSettings(docs);
            TestExporter exporter = new TestExporter();
            exporter.SetDocs(docs);
            exporter.SetWaitingForFlag(true);            
            Assert.IsTrue(exporter.getCodeEndFlag(docIndex, imgIndex, args));
            Assert.IsTrue(exporter.getGroupEndFlag(docIndex, imgIndex, args));
            exporter.SetWaitingForFlag(false);
            Assert.IsFalse(exporter.getCodeEndFlag(docIndex, imgIndex, args));
            Assert.IsFalse(exporter.getGroupEndFlag(docIndex, imgIndex, args));
        }

        [TestMethod]
        public void Exporters_XrefExporter_isFlagNeeded()
        {
            Document parent = new Document("DOC122", null, null, new Dictionary<string, string>() { { "TEST", "abc123" } }, null);
            Document doc = new Document("DOC123", parent, null, new Dictionary<string, string>() { { "TEST", "XYZ321" } }, null);            
            Trigger trigger = new Trigger();
            trigger.Type = XrefTrigger.TriggerType.None;
            TestExporter exporter = new TestExporter();
            Assert.IsFalse(exporter.isFlagNeeded(doc, trigger.GetXrefTrigger(), parent));
            trigger.Type = XrefTrigger.TriggerType.Family;
            Assert.IsTrue(exporter.isFlagNeeded(parent, trigger.GetXrefTrigger(), null));
            Assert.IsFalse(exporter.isFlagNeeded(doc, trigger.GetXrefTrigger(), parent));
            trigger.Type = XrefTrigger.TriggerType.Regex;
            trigger.FieldName = "TEST";
            trigger.RegexPattern = "[a-zA-Z]+\\d+";
            Assert.IsTrue(exporter.isFlagNeeded(parent, trigger.GetXrefTrigger(), null));
            Assert.IsTrue(exporter.isFlagNeeded(doc, trigger.GetXrefTrigger(), parent));
            doc.Metadata["TEST"] = "123nope";
            Assert.IsFalse(exporter.isFlagNeeded(doc, trigger.GetXrefTrigger(), parent));
        }

        [TestMethod]
        public void Exporters_XrefExporter_hasFieldValueChanged()
        {
            Document parent = new Document("DOC122", null, null, 
                new Dictionary<string, string>() {
                    { "FILE", @"X:\ROOT\VOL\DIR1\FILE1.TXT" },
                    { "EXT", "TXT" } }, null);
            Document doc = new Document("DOC123", parent, null, 
                new Dictionary<string, string>() {
                    { "FILE", @"X:\ROOT\VOL\DIR2\FILE2.TXT" },
                    { "EXT", "TXT" } }, null);
            Document child = new Document("DOC124", doc, null, 
                new Dictionary<string, string>() {
                    { "FILE", @"X:\ROOT\VOL\DIR2\FILE3.PDF" },
                    { "EXT", "PDF" } }, null);
            Trigger trigger = new Trigger();
            trigger.Type = XrefTrigger.TriggerType.FieldValueChange;
            trigger.FieldName = "EXT";
            trigger.FieldChangeOption = XrefTrigger.FieldValueChangeOption.None;
            Assert.IsFalse(TestExporter.hasFieldValueChanged(doc, parent, trigger.GetXrefTrigger()));
            Assert.IsTrue(TestExporter.hasFieldValueChanged(child, doc, trigger.GetXrefTrigger()));
            trigger.FieldName = "FILE";
            trigger.FieldChangeOption = XrefTrigger.FieldValueChangeOption.StripFileName;
            Assert.IsTrue(TestExporter.hasFieldValueChanged(doc, parent, trigger.GetXrefTrigger()));
            Assert.IsFalse(TestExporter.hasFieldValueChanged(child, doc, trigger.GetXrefTrigger()));
            trigger.FieldChangeOption = XrefTrigger.FieldValueChangeOption.UseStartingSegments;
            trigger.SegmentDelimiter = "\\";
            trigger.SegmentCount = 4;
            Assert.IsTrue(TestExporter.hasFieldValueChanged(doc, parent, trigger.GetXrefTrigger()));
            Assert.IsFalse(TestExporter.hasFieldValueChanged(child, doc, trigger.GetXrefTrigger()));
            trigger.FieldChangeOption = XrefTrigger.FieldValueChangeOption.UseEndingSegments;
            trigger.SegmentDelimiter = ".";
            trigger.SegmentCount = 1;
            Assert.IsFalse(TestExporter.hasFieldValueChanged(doc, parent, trigger.GetXrefTrigger()));
            Assert.IsTrue(TestExporter.hasFieldValueChanged(child, doc, trigger.GetXrefTrigger()));
        }

        [TestMethod]
        public void Exporters_XrefExporter_moveDocPos()
        {
            var docs = GetDocs();
            Document first = docs[0];
            Document last = docs[1];
            TestExporter exporter = new TestExporter();
            exporter.SetDocs(docs);
            Assert.AreEqual(last, exporter.getNextDoc(0));
            Assert.AreEqual(first, exporter.getPreviousDoc(1));
            Assert.AreEqual(null, exporter.getNextDoc(1));
            Assert.AreEqual(null, exporter.getPreviousDoc(0));
        }

        [TestMethod]
        public void Exporters_XrefExporter_boolToString()
        {
            Assert.AreEqual("1", TestExporter.boolToString(true));
            Assert.AreEqual("0", TestExporter.boolToString(false));
        }

        [TestMethod]
        public void Exporters_XrefExporter_getNextImageKey()
        {
            var docs = GetDocs();            
            TestExporter exporter = new TestExporter();
            exporter.SetDocs(docs);
            Assert.AreEqual("DOC000002", exporter.getNextImageKey(0, 0));
            Assert.AreEqual("DOC000003", exporter.getNextImageKey(0, 1));
            Assert.AreEqual(null, exporter.getNextImageKey(1, 1));
            Assert.AreEqual(null, exporter.getNextImageKey(2, 1));
        }

        [TestMethod]
        public void Exporters_XrefExporter_getRecordComponents()
        {
            var docs = GetDocs();
            TestExporter exporter = new TestExporter();
            exporter.SetDocs(docs);
            var settings = new XrefExport();
            var args = settings.GetFileSettings(docs);
            string record = String.Join(", ", exporter.getRecordComponents(args, 0, 0));
            string expected = "X:\\VOL001\\IMAGES\\0001\\DOC000001.jpg, DOC, 000001, , 0, 0, 1, 0, 0, 0, 0, , , ";
            Assert.AreEqual(expected, record);
        }

        [TestMethod]
        public void Exporters_XrefExporter_getGhostBoxLine()
        {
            var docs = GetDocs();
            TestExporter exporter = new TestExporter();
            exporter.SetDocs(docs);
            exporter.SetBoxNo(0);
            Trigger trigger = new Trigger();
            trigger.Type = XrefTrigger.TriggerType.Family;
            string result = exporter.getGhostBoxLine("DOC000001", String.Empty, trigger.GetXrefTrigger(), 0);
            Assert.AreEqual(@"\Box001\..", result);
            result = exporter.getGhostBoxLine("DOC000002", String.Empty, trigger.GetXrefTrigger(), 1);
            Assert.AreEqual(@"\Box001\..", result);
            result = exporter.getGhostBoxLine("DOC000003", String.Empty, trigger.GetXrefTrigger(), 1);
            Assert.AreEqual(@"\Box001\..", result);
            trigger.Type = XrefTrigger.TriggerType.FieldValueChange;
            trigger.FieldName = "DOCID";
            exporter.SetBoxNo(0);
            result = exporter.getGhostBoxLine("DOC000001", String.Empty, trigger.GetXrefTrigger(), 0);
            Assert.AreEqual(@"\Box001\..", result);
            result = exporter.getGhostBoxLine("DOC000002", String.Empty, trigger.GetXrefTrigger(), 1);
            Assert.AreEqual(@"\Box002\..", result);
            result = exporter.getGhostBoxLine("DOC000003", String.Empty, trigger.GetXrefTrigger(), 1);
            Assert.AreEqual(@"\Box002\..", result);
        }

        [TestMethod]
        public void Exporters_XrefExporter_getPageRecords()
        {
            var docs = GetDocs();
            TestExporter exporter = new TestExporter();
            var settings = new XrefExport();
            var args = settings.GetFileSettings(docs);
            exporter.SetDocs(docs);
            var ssinfo = new SlipsheetsInfo();            
            ssinfo.FolderName = "SlipSheets";                        
            var field = new SlipsheetField();
            field.FieldName = "DOCID";
            field.Alias = "begno";
            ssinfo.Fields = new SlipsheetField[] { field };
            Trigger trigger = new Trigger();
            trigger.Type = XrefTrigger.TriggerType.FieldValueChange;
            trigger.FieldName = "DOCID";
            trigger.FieldChangeOption = XrefTrigger.FieldValueChangeOption.None;
            ssinfo.Trigger = trigger;
            var ss = new SlipSheets(docs, ssinfo.GetSlipsheetSettings(), null);
            var actual = exporter.getPageRecords(args, 1, ss);
            List<string> expected = new List<string>();
            expected.Add("\\SlipSheets\\DOC000001.001.TIF, DOC, 000001, .001, 0, 0, 1, 0, 0, 0, 1, , , ");
            expected.Add("X:\\VOL001\\IMAGES\\0001\\DOC000002.tif, DOC, 000002, , 0, 0, 1, 0, 0, 0, 0, , , ");
            expected.Add("X:\\VOL001\\IMAGES\\0001\\DOC000003.tif, DOC, 000003, , 0, 0, 0, 0, 0, 0, 0, , , ");

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
