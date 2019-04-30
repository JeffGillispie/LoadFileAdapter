using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Importers;
using LoadFileAdapter.Parsers;
using Moq;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_LfpImporter
    {        
        private LfpImporter importer;
        private List<string[]> records;

        public TU_LfpImporter()
        {
            records = new List<string[]> {
                new string[] { "IM", "DOC000001", "D", "0", "@VOL001", "IMAGES\\0001", "DOC000001.tif", "2", "0" },
                new string[] { "OF", "DOC000001", "@VOL001", "NATIVE\\0001", "DOC000001.XLSX", "1" },
                new string[] { "IM", "DOC000002", "D", "0", "@VOL001", "IMAGES\\0001", "DOC000002.tif", "2", "0" },
                new string[] { "IM", "DOC000003", " ", "0", "@VOL001", "IMAGES\\0001", "DOC000003.tif", "2", "0" },
                new string[] { "IM", "DOC000004", "C", "0", "@VOL001", "IMAGES\\0001", "DOC000004.tif", "2", "0" },
                new string[] { "IM", "DOC000005", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000005.tif", "2", "0" },
                new string[] { "IM", "DOC000006", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000006.tif", "2", "0" },
                new string[] { "IM", "DOC000007", "C", "0", "@VOL001", "IMAGES\\0001", "DOC000007.tif", "2", "0" },
                new string[] { "IM", "DOC000008", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000008.tif", "2", "0" },
                new string[] { "IM", "DOC000009", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000009.tif", "2", "0" },
                new string[] { "IM", "DOC000010", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000010.tif", "2", "0" },
                new string[] { "IM", "DOC000011", "D", "0", "@VOL001", "IMAGES\\0001", "DOC000011.tif", "2", "0" },
                new string[] { "IM", "DOC000012", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000012.tif", "2", "0" },
                new string[] { "IM", "DOC000013", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000013.tif", "2", "0" },
                new string[] { "IM", "DOC000014", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000014.tif", "2", "0" },
                new string[] { "IM", "DOC000015", "C", "0", "@VOL001", "IMAGES\\0001", "DOC000015.jpg", "5", "0" },
                new string[] { "IM", "DOC000016", "D", "0", "@VOL001", "IMAGES\\0001", "DOC000016.tif", "2", "0" },
                new string[] { "IM", "DOC000017", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000017.tif", "2", "0" },
                new string[] { "IM", "DOC000018", "D", "0", "@VOL001", "IMAGES\\0001", "DOC000018.tif", "2", "0" },
                new string[] { "IM", "DOC000019", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000019.tif", "2", "0" },
                new string[] { "IM", "DOC000020", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000020.tif", "2", "0" },
                new string[] { "IM", "DOC000021", "C", "0", "@VOL001", "IMAGES\\0001", "DOC000021.tif", "2", "0" },
                new string[] { "IM", "DOC000022", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000022.tif", "2", "0" },
                new string[] { "IM", "DOC000023", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000023.tif", "2", "0" },
                new string[] { "IM", "DOC000024", "",  "0", "@VOL001", "IMAGES\\0001", "DOC000024.tif", "2", "0" },
                new string[] { "IM", "DOC000025", "C", "0", "@VOL001", "IMAGES\\0001", "DOC000025.tif", "2", "0" }
            };
        }

        [TestMethod]
        public void Importers_LfpImporter_Test()
        {
            // arrange
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.lfp");
            Encoding encoding = Encoding.GetEncoding(1252);
            TextBuilder.TextLevel textLevel = TextBuilder.TextLevel.None;
            TextBuilder.TextLocation textLocation = TextBuilder.TextLocation.None;
            string pattern = String.Empty;
            Regex find = new Regex(pattern);
            string replace = String.Empty;
            TextBuilder repSetting = new TextBuilder(textLevel, textLocation, find, replace);
            var builder = new LfpBuilder();
            builder.PathPrefix = infile.Directory.FullName;
            builder.TextBuilder = repSetting;
            var mockParser = new Mock<LfpParser>(MockBehavior.Strict);
            mockParser.Setup(p => p.Parse(It.IsAny<TextReader>())).Returns(records);
            mockParser.Setup(p => p.Parse(It.IsAny<FileInfo>(), It.IsAny<Encoding>())).Returns(records);
            importer = new LfpImporter();
            importer.Parser = mockParser.Object;
            importer.Builder = builder;

            // act
            DocumentCollection docs = importer.Import(infile, encoding);

            // assert
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(25, docs.ImageCount);
            Assert.AreEqual(0, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);
        }        
    }
}
