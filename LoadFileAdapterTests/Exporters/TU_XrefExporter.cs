using System;
using System.Collections.Generic;
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
            string result = exporter.getCustomValue(key1, doc, field);
            Assert.AreEqual(value, result);
            result = exporter.getCustomValue(key2, doc, field);
            Assert.AreEqual(String.Empty, result);            
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
    }
}
