﻿using System;
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
    public class TU_OptExporter
    {
        List<string> datLines;
        List<string> lfpLines;        
        List<string> optLines;

        public TU_OptExporter()
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
        public void Exporters_OptExporter_FromCsvTest()
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
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.dat");
            bool hasHeader = true;
            string keyColName = "DOCID";
            string parentColName = "BEGATT";
            string childColName = String.Empty;
            string childColDelim = ";";
            string vol = "TEST001";
            RepresentativeBuilder repSetting = new RepresentativeBuilder("NATIVE", Representative.FileType.Native);
            List<RepresentativeBuilder> reps = new List<RepresentativeBuilder>();
            reps.Add(repSetting);
            var builder = new DatBuilder();
            IParser parser = new DatParser(Delimiters.CONCORDANCE);
            builder.HasHeader = hasHeader;
            builder.KeyColumnName = keyColName;
            builder.ParentColumnName = parentColName;
            builder.ChildColumnName = childColName;
            builder.ChildSeparator = childColDelim;
            builder.RepresentativeBuilders = reps;
            builder.PathPrefix = infile.Directory.FullName;

            // act
            List<string[]> records = parser.Parse(mockReader.Object);            
            List<Document> documents = builder.Build(records);
            DocumentCollection docs = new DocumentCollection(documents);
            var exporter = OptExporter.Builder.Start(mockWriter.Object).SetVolumeName(vol).Build();                
            exporter.Export(docs);

            // assert            
            Assert.IsTrue(output.Count == 0);
        }

        [TestMethod]
        public void Exporters_OptExporter_FromLfpTest()
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
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.lfp");
            string vol = "TEST001";
            TextBuilder rep = new TextBuilder(
                TextBuilder.TextLevel.None,
                TextBuilder.TextLocation.None,
                null, null);
            var builder = new LfpBuilder();
            IParser parser = new LfpParser();
            builder.PathPrefix = String.Empty;
            builder.TextBuilder = rep;

            // act
            List<string[]> records = parser.Parse(mockReader.Object);            
            List<Document> documents = builder.Build(records);
            DocumentCollection docs = new DocumentCollection(documents);
            var exporter = OptExporter.Builder.Start(mockWriter.Object).SetVolumeName(vol).Build();                
            exporter.Export(docs);

            // assert
            Assert.AreEqual("000000001,TEST001,IMG_0001\\000000001.jpg,Y,,,3", output[0]);
            Assert.AreEqual("000000002,TEST001,IMG_0001\\000000002.tif,,,,", output[1]);
            Assert.AreEqual("000000003,TEST001,IMG_0001\\000000003.tif,,,,", output[2]);
        }
                
        [TestMethod]
        public void Exporters_OptExporter_FromOptTest()
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
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.opt");
            string vol = "TEST001";
            TextBuilder rep = new TextBuilder(
                TextBuilder.TextLevel.None,
                TextBuilder.TextLocation.None,
                null, null);
            var builder = new OptBuilder();
            IParser parser = new DatParser(Delimiters.COMMA_DELIMITED);
            builder.PathPrefix = String.Empty;
            builder.TextBuilder = rep;

            // act
            List<string[]> records = parser.Parse(mockReader.Object);            
            List<Document> documents = builder.Build(records);
            DocumentCollection docs = new DocumentCollection(documents);
            var exporter = OptExporter.Builder.Start(mockWriter.Object).SetVolumeName(vol).Build();                
            exporter.Export(docs);

            // assert
            Assert.AreEqual("000000001,TEST001,X:\\VOL001\\IMAGES\\0001\\000000001.jpg,Y,,,1", output[0]);
            Assert.AreEqual("000000002,TEST001,X:\\VOL001\\IMAGES\\0001\\000000002.tif,Y,,,2", output[1]);
            Assert.AreEqual("000000003,TEST001,X:\\VOL001\\IMAGES\\0001\\000000003.tif,,,,", output[2]);
        }
    }
}
