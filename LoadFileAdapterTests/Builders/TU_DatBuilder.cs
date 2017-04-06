using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_DatBuilder
    {
        private static List<string[]> records;
        private IBuilder<BuildDocCollectionDatSettings, BuildDocDatSettings> builder = new DatBuilder();

        public TU_DatBuilder()
        {
            records = new List<string[]>() {
                new string[] { "DOCID", "BEGATT", "ATTCHIDS", "VOLUME", "PAGE COUNT", "NATIVE", "TEST1", "TEST2", "TEST3" },
                new string[] { "DOC000001", "DOC000001", "", "VOL001", "1", "X:\\VOL001\\NATIVE\\0001\\DOC000001.XLSX", "a", "ONE", "k" },
                new string[] { "DOC000002", "DOC000002", "DOC000004;DOC000007", "VOL001", "2", "", "b", "TWO", "l" },
                new string[] { "DOC000004", "DOC000002", "", "VOL001", "3", "", "c", "BUCKLE", "m" },
                new string[] { "DOC000007", "DOC000002", "", "VOL001", "4", "", "d", "MY", "n" },
                new string[] { "DOC000011", "DOC000011", "DOC000015", "VOL001", "5", "", "e", "SHOE", "o" },
                new string[] { "DOC000015", "DOC000011", "", "VOL001", "1", "", "f", "THREE", "p" },
                new string[] { "DOC000016", "DOC000016", "", "VOL001", "2", "", "g", "FOUR", "q" },
                new string[] { "DOC000018", "DOC000018", "DOC000021;DOC000025", "VOL001", "3", "", "h", "SHUT", "r" },
                new string[] { "DOC000021", "DOC000018", "", "VOL001", "4", "", "i", "THE", "s" },
                new string[] { "DOC000025", "DOC000018", "", "VOL001", "1", "", "j", "DOOR", "t" },
            };
        }

        [TestMethod]
        public void Builders_DatBuilder_ParentName()
        {
            // Arrange
            bool hasHeader = true;
            string keyColName = "DOCID";
            string parentColName = "BEGATT";
            string childColName = String.Empty;
            string childColDelim = ";";
            DatRepresentativeSettings rep = new DatRepresentativeSettings("NATIVE", Representative.FileType.Native);            
            List<DatRepresentativeSettings> repColInfo = new List<DatRepresentativeSettings>() { rep };
            string pathPrefix = String.Empty;
            BuildDocCollectionDatSettings args = new BuildDocCollectionDatSettings(
                records, pathPrefix, hasHeader, keyColName, parentColName, childColName, childColDelim, repColInfo);

            // Act
            List<Document> documents = builder.BuildDocuments(args);
            DocumentCollection docs = new DocumentCollection(documents);

            // Assert
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(0, docs.ImageCount);
            Assert.AreEqual(0, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);
        }

        [TestMethod]
        public void Builders_DatBuilder_ChildName()
        {
            // Arrange
            bool hasHeader = true;
            string keyColName = "DOCID";
            string parentColName = null;
            string childColName = "ATTCHIDS";
            string childColDelim = ";";
            DatRepresentativeSettings rep = new DatRepresentativeSettings("NATIVE", Representative.FileType.Native);
            List<DatRepresentativeSettings> repColInfo = new List<DatRepresentativeSettings>() { rep };
            string pathPrefix = String.Empty;
            BuildDocCollectionDatSettings args = new BuildDocCollectionDatSettings(
                records, pathPrefix, hasHeader, keyColName, parentColName, childColName, childColDelim, repColInfo);

            // Act
            List<Document> documents = builder.BuildDocuments(args);
            DocumentCollection docs = new DocumentCollection(documents);

            // Assert
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(0, docs.ImageCount);
            Assert.AreEqual(0, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);
        }

        [TestMethod]
        public void Builders_DatBuilder_GetHeader()
        {
            string[] record = new string[] { "one", "two", "three" };
            string[] headerA = ((DatBuilder)builder).GetHeader(record, true);
            string[] headerB = ((DatBuilder)builder).GetHeader(record, false);
            Assert.AreEqual(String.Join(",", record), String.Join(",", headerA));
            Assert.AreEqual("Column 1,Column 2,Column 3", String.Join(",", headerB));
        }
    }
}
