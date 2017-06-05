using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Importers;
using LoadFileAdapter.Parsers;
using LoadFileAdapter.Builders;
using Moq;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_DatImporter
    {
        private DatImporter importer;
        private List<string[]> records;
                
        public TU_DatImporter()
        {
            records = new List<string[]>() {
                new string[] { "DOCID", "BEGATT", "VOLUME", "PAGE COUNT", "NATIVE", "TEST1", "TEST2", "TEST3" },
                new string[] { "DOC000001", "DOC000001", "VOL001", "1", "X:\\VOL001\\NATIVE\\0001\\DOC000001.XLSX", "a", "ONE", "k" },
                new string[] { "DOC000002", "DOC000002", "VOL001", "2", "", "b", "TWO", "l" },
                new string[] { "DOC000004", "DOC000002", "VOL001", "3", "", "c", "BUCKLE", "m" },
                new string[] { "DOC000007", "DOC000002", "VOL001", "4", "", "d", "MY", "n" },
                new string[] { "DOC000011", "DOC000011", "VOL001", "5", "", "e", "SHOE", "o" },
                new string[] { "DOC000015", "DOC000011", "VOL001", "1", "", "f", "THREE", "p" },
                new string[] { "DOC000016", "DOC000016", "VOL001", "2", "", "g", "FOUR", "q" },
                new string[] { "DOC000018", "DOC000018", "VOL001", "3", "", "h", "SHUT", "r" },
                new string[] { "DOC000021", "DOC000018", "VOL001", "4", "", "i", "THE", "s" },
                new string[] { "DOC000025", "DOC000018", "VOL001", "1", "", "j", "DOOR", "t" },
            };
        }

        [TestMethod]
        public void Importers_DatImporter_Test()
        {
            // arrange
            FileInfo infile = new FileInfo(@"X:\VOL001\infile.lfp");
            Encoding encoding = Encoding.GetEncoding(1252);
            Delimiters delimiters = Delimiters.CONCORDANCE;
            bool hasHeader = true;
            string keyColName = "DOCID";
            string parentColName = "BEGATT";
            string childColName = String.Empty;
            string childColDelim = ";";
            RepresentativeBuilder repSetting = new RepresentativeBuilder("NATIVE", Representative.FileType.Native);            
            List<RepresentativeBuilder> reps = new List<RepresentativeBuilder>();
            reps.Add(repSetting);
            var builder = new DatBuilder();
            builder.HasHeader = hasHeader;
            builder.KeyColumnName = keyColName;
            builder.ParentColumnName = parentColName;
            builder.ChildColumnName = childColName;
            builder.ChildSeparator = childColDelim;
            builder.RepresentativeBuilders = reps;
            builder.PathPrefix = infile.Directory.FullName;
            var mockParser = new Mock<DatParser>(MockBehavior.Strict);
            mockParser.Setup(p => p.Parse(It.IsAny<TextReader>())).Returns(records);
            mockParser.Setup(p => p.Parse(It.IsAny<FileInfo>(), It.IsAny<Encoding>())).Returns(records);
            importer = new DatImporter(mockParser.Object);
            importer.Builder = builder;

            // act
            DocumentCollection docs = importer.Import(infile, encoding);
                

            // assert
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(0, docs.ImageCount);
            Assert.AreEqual(0, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);
            Assert.AreEqual("DOC000001", docs[0].Metadata["DOCID"]);
        }
    }
}
