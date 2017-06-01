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
    public class TU_LfpExporter
    {
        List<string> datLines;
        List<string> lfpSingleLines;
        List<string> lfpMultiLines;
        List<string> optLines;
        
        public TU_LfpExporter()
        {
            datLines = new List<string>(new string[] {
                "þDOCIDþþBEGATTþþVOLUMEþþNATIVEþ",
                "þDOC000001þþDOC000001þþVOL001þþX:\\VOL001\\NATIVE\\0001\\DOC000001.XLSXþ",
                "þDOC000002þþDOC000001þþVOL001þþþ",
                null,null
            });

            lfpSingleLines = new List<string>(new string[]
            {
                "IM,000000001,D,0,@001;IMG_0001;000000001.jpg;4,0",
                "IM,000000002,,0,@001;IMG_0001;000000002.tif;2,0",
                "IM,000000003,,0,@001;IMG_0001;000000003.tif;2,0",
                null, null
            });

            lfpMultiLines = new List<string>(new string[]
            {
                "IM,000000001,D,1,@001;IMG_0001;000000001.pdf;7,0",
                "IM,000000002,,2,@001;IMG_0001;000000001.pdf;7,0",
                "IM,000000003,,3,@001;IMG_0001;000000001.pdf;7,0",
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
        public void Exporters_LfpExporter_FromCsvTest()
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
            string vol = "TEST001";
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
            IExporter<IExportImageSettings> exporter = new LfpExporter();
            ExportImageWriterSettings exportArgs = new ExportImageWriterSettings(mockWriter.Object, docs, vol);
            exporter.Export(exportArgs);

            // assert
            Assert.AreEqual("OF,DOC000001,@TEST001;X:\\VOL001\\NATIVE\\0001;DOC000001.XLSX,1", output[0]);
        }

        [TestMethod]
        public void Exporters_LfpExporter_FromLfpSingleTest()
        {
            // Arrange               
            var mockReader = new Mock<TextReader>();
            var mockWriter = new Mock<TextWriter>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => lfpSingleLines[calls])
                .Callback(() => calls++);
            List<string> output = new List<string>();
            mockWriter
                .Setup(r => r.WriteLine(It.IsAny<string>()))
                .Callback((string s) => output.Add(s));
            ParseReaderSettings readArgs = new ParseReaderSettings(mockReader.Object);            
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.lfp");                                    
            string vol = "TEST001";
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
            IExporter<IExportImageSettings> exporter = new LfpExporter();
            ExportImageWriterSettings exportArgs = new ExportImageWriterSettings(mockWriter.Object, docs, vol);
            exporter.Export(exportArgs);

            // assert
            Assert.AreEqual("IM,000000001,D,0,@TEST001;IMG_0001;000000001.jpg;4,0", output[0]);
            Assert.AreEqual("IM,000000002,,0,@TEST001;IMG_0001;000000002.tif;2,0", output[1]);
            Assert.AreEqual("IM,000000003,,0,@TEST001;IMG_0001;000000003.tif;2,0", output[2]);
        }

        [TestMethod]
        public void Exporters_LfpExporter_FromLfpMultiTest()
        {
            // Arrange               
            var mockReader = new Mock<TextReader>();
            var mockWriter = new Mock<TextWriter>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => lfpMultiLines[calls])
                .Callback(() => calls++);
            List<string> output = new List<string>();
            mockWriter
                .Setup(r => r.WriteLine(It.IsAny<string>()))
                .Callback((string s) => output.Add(s));
            ParseReaderSettings readArgs = new ParseReaderSettings(mockReader.Object);
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.lfp");
            string vol = "TEST001";
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
            IExporter<IExportImageSettings> exporter = new LfpExporter();
            ExportImageWriterSettings exportArgs = new ExportImageWriterSettings(mockWriter.Object, docs, vol);
            exporter.Export(exportArgs);

            // assert
            Assert.AreEqual("IM,000000001,D,1,@TEST001;IMG_0001;000000001.pdf;7,0", output[0]);
            Assert.AreEqual("IM,000000002,,2,@TEST001;IMG_0001;000000001.pdf;7,0", output[1]);
            Assert.AreEqual("IM,000000003,,3,@TEST001;IMG_0001;000000001.pdf;7,0", output[2]);
        }

        [TestMethod]
        public void Exporters_LfpExporter_FromOptTest()
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
            string vol = "TEST001";
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
            IExporter<IExportImageSettings> exporter = new LfpExporter();
            ExportImageWriterSettings exportArgs = new ExportImageWriterSettings(mockWriter.Object, docs, vol);
            exporter.Export(exportArgs);

            // assert
            Assert.AreEqual("IM,000000001,D,0,@TEST001;X:\\VOL001\\IMAGES\\0001;000000001.jpg;4,0", output[0]);
            Assert.AreEqual("IM,000000002,D,0,@TEST001;X:\\VOL001\\IMAGES\\0001;000000002.tif;2,0", output[1]);
            Assert.AreEqual("IM,000000003,,0,@TEST001;X:\\VOL001\\IMAGES\\0001;000000003.tif;2,0", output[2]);
        }
    }
}
