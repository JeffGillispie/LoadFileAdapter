using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Exporters;
using LoadFileAdapter.Parsers;
using Moq;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_DatExporter
    {
        List<string> datLines;
        List<string> lfpLines;
        List<string> optLines;

        public TU_DatExporter()
        {
            datLines = new List<string>(new string[] {
                "þDOCIDþþBEGATTþþVOLUMEþþNATIVEþ",
                "þDOC000001þþDOC000001þþVOL001þþX:\\VOL001\\NATIVE\\0001\\DOC000001.XLSXþ",
                "þDOC000002þþDOC000001þþVOL001þþþ",
                null,null
            });

            lfpLines = new List<string>(new string[]
            {
                "IM,000000001,D,0,@001;IMG_0001;000000001.jpg;4,0",
                "IM,000000002,,0,@001;IMG_0001;000000002.tif;2,0",
                "IM,000000003,,0,@001;IMG_0001;000000003.tif;2,0",
                null, null
            });

            optLines = new List<string>(new string[]
            {
                "000000001,VOL001,X:\\VOL001\\IMAGES\\0001\\000000001.jpg,Y,,,1",
                "000000002,VOL001,X:\\VOL001\\IMAGES\\0001\\000000002.tif,Y,,,2",
                "000000003,VOL001,X:\\VOL001\\IMAGES\\0001\\000000003.tif,,,,",
                null, null
            });
        }

        [TestMethod]
        public void Exporters_DatExporter_FromCsvTest()
        {
            // Arrange               
            var mockReader = new Mock<TextReader>();
            var mockWriter = new Mock<TextWriter>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => datLines[calls])
                .Callback(() => calls++);
            List<string> output = new List<string>();
            mockWriter
                .Setup(r => r.WriteLine(It.IsAny<string>()))
                .Callback((string s) => output.Add(s));
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

            // act
            List<string[]> records = parser.Parse(readArgs);
            BuildDocCollectionDatSettings buildArgs = new BuildDocCollectionDatSettings(
                records, infile.Directory.FullName, hasHeader, keyColName, parentColName, childColName, childColDelim, reps);
            List<Document> documents = builder.BuildDocuments(buildArgs);
            DocumentCollection docs = new DocumentCollection(documents);
            IExporter<IExportDatSettings> exporter = new DatExporter();
            string[] fields = new string[] { "DOCID", "BEGATT", "VOLUME", "NATIVE" };
            ExportDatWriterSettings exportArgs = new ExportDatWriterSettings(mockWriter.Object, docs, Delimiters.PIPE_CARET, fields);            
            exporter.Export(exportArgs);

            // assert            
            Assert.AreEqual("^DOCID^|^BEGATT^|^VOLUME^|^NATIVE^", output[0]);
            Assert.AreEqual("^DOC000001^|^DOC000001^|^VOL001^|^X:\\VOL001\\NATIVE\\0001\\DOC000001.XLSX^", output[1]);
            Assert.AreEqual("^DOC000002^|^DOC000001^|^VOL001^|^^", output[2]);
        }

        [TestMethod]
        public void Exporters_DatExporter_FromOptTest()
        {
            // Arrange               
            var mockReader = new Mock<TextReader>();
            var mockWriter = new Mock<TextWriter>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => lfpLines[calls])
                .Callback(() => calls++);
            List<string> output = new List<string>();
            mockWriter
                .Setup(r => r.WriteLine(It.IsAny<string>()))
                .Callback((string s) => output.Add(s));
            ParseReaderSettings readArgs = new ParseReaderSettings(mockReader.Object);
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.lfp");
            TextRepresentativeSettings rep = new TextRepresentativeSettings(
                TextRepresentativeSettings.TextLevel.None,
                TextRepresentativeSettings.TextLocation.None,
                null, null);
            IBuilder<BuildDocCollectionImageSettings, BuildDocLfpSettings> builder = new LfpBuilder();
            IParser<ParseFileSettings, ParseReaderSettings, ParseLineSettings> parser = new LfpParser();

            // act
            List<string[]> records = parser.Parse(readArgs);
            BuildDocCollectionImageSettings buildArgs = new BuildDocCollectionImageSettings(records, String.Empty, rep);
            List<Document> documents = builder.BuildDocuments(buildArgs);
            DocumentCollection docs = new DocumentCollection(documents);
            IExporter<IExportDatSettings> exporter = new DatExporter();
            string[] fields = new string[] { "DocID", "Page Count", "Volume Name" };
            ExportDatWriterSettings exportArgs = new ExportDatWriterSettings(mockWriter.Object, docs, Delimiters.COMMA_DELIMITED, fields);            
            exporter.Export(exportArgs);

            // assert
            Assert.AreEqual("DocID,Page Count,Volume Name", output[0]);
            Assert.AreEqual("000000001,3,001", output[1]);            
        }

        [TestMethod]
        public void Exporters_DatExporter_FromLfpTest()
        {
            // Arrange               
            var mockReader = new Mock<TextReader>();
            var mockWriter = new Mock<TextWriter>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => optLines[calls])
                .Callback(() => calls++);
            List<string> output = new List<string>();
            mockWriter
                .Setup(r => r.WriteLine(It.IsAny<string>()))
                .Callback((string s) => output.Add(s));
            ParseReaderDatSettings readArgs = new ParseReaderDatSettings(mockReader.Object, Delimiters.COMMA_DELIMITED);
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.opt");
            TextRepresentativeSettings rep = new TextRepresentativeSettings(
                TextRepresentativeSettings.TextLevel.None,
                TextRepresentativeSettings.TextLocation.None,
                null, null);
            IBuilder<BuildDocCollectionImageSettings, BuildDocImageSettings> builder = new OptBuilder();
            IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings> parser = new DatParser();

            // act
            List<string[]> records = parser.Parse(readArgs);
            BuildDocCollectionImageSettings buildArgs = new BuildDocCollectionImageSettings(records, String.Empty, rep);
            List<Document> documents = builder.BuildDocuments(buildArgs);
            DocumentCollection docs = new DocumentCollection(documents);
            IExporter<IExportDatSettings> exporter = new DatExporter();
            string[] fields = new string[] { "DocID", "Page Count" };
            ExportDatWriterSettings exportArgs = new ExportDatWriterSettings(mockWriter.Object, docs, Delimiters.COMMA_QUOTE, fields);            
            exporter.Export(exportArgs);

            // assert
            Assert.AreEqual("\"DocID\",\"Page Count\"", output[0]);
            Assert.AreEqual("\"000000001\",\"1\"", output[1]);
            Assert.AreEqual("\"000000002\",\"2\"", output[2]);
        }
    }
}
