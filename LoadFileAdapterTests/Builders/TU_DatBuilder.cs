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

        class TestBuilder : DatBuilder
        {
            public new void settleFamilyDrama(
                string parentColName, string childColName, string childSeparator, Document doc, Dictionary<string, Document> docs, Dictionary<string, Document> paternity)
            {
                base.settleFamilyDrama(parentColName, childColName, childSeparator, doc, docs, paternity);
            }
            public new void setFamilyFromParent(
                Document doc, Dictionary<string, Document> docs, Dictionary<string, Document> paternity, string parentColName, string childColName, string childSeparator)
            {
                base.setFamilyFromParent(doc, docs, paternity, parentColName, childColName, childSeparator);
            }
            public new void setFamilyFromChildren(
                Document doc, string childColName, string childSeparator, Dictionary<string, Document> paternity)
            {
                base.setFamilyFromChildren(doc, childColName, childSeparator, paternity);
            }
        }

        [TestMethod]
        public void Builders_DatBuilder_BuildDocument_Reps()
        {
            // test with an empty path prefix
            DatRepresentativeSettings rep = new DatRepresentativeSettings(
                "NATIVE", Representative.FileType.Native);
            BuildDocDatSettings settings = new BuildDocDatSettings(
                records[1], records[0], "DOCID", new List<DatRepresentativeSettings>() { rep }, String.Empty);
            Document doc = builder.BuildDocument(settings);            
            Assert.AreEqual(Representative.FileType.Native, doc.Representatives.First().Type);
            Assert.AreEqual(records[1][0], doc.Representatives.First().Files.First().Key);
            string expected = "X:\\VOL001\\NATIVE\\0001\\DOC000001.XLSX";
            Assert.AreEqual(expected, doc.Representatives.First().Files.First().Value);

            // test with a populated path prefix
            string[] newRecord = new string[records[1].Length];
            records[1].CopyTo(newRecord, 0);
            newRecord[5] = "\\VOL001\\NATIVE\\0001\\DOC000001.XLSX";
            settings = new BuildDocDatSettings(
                newRecord, records[0], "DOCID", new List<DatRepresentativeSettings>() { rep }, "Z:\\");
            doc = builder.BuildDocument(settings);
            Assert.AreEqual(Representative.FileType.Native, doc.Representatives.First().Type);
            Assert.AreEqual(records[1][0], doc.Representatives.First().Files.First().Key);
            expected = "Z:\\VOL001\\NATIVE\\0001\\DOC000001.XLSX";
            Assert.AreEqual(expected, doc.Representatives.First().Files.First().Value);
        }

        [TestMethod]
        public void Builders_DatBuilder_BuildDocument_Metadata()
        {
            // test that the document metadata is populated correctly
            BuildDocDatSettings settings = new BuildDocDatSettings(
                records[2], records[0], "DOCID", null, String.Empty);
            Document doc = builder.BuildDocument(settings);

            for (int i = 0; i < records[0].Length; i++)
            {
                Assert.AreEqual(records[2][i], doc.Metadata[records[0][i]]);
            }

            // default values for family properties
            Assert.IsTrue(doc.Parent == null);
            Assert.IsTrue(doc.Children == null);
        }

        [TestMethod]
        public void Builders_DatBuilder_BuildDocument_KeyValue()
        {
            // test that the document key is set as expected
            // if the key column name has a value or is empty
            BuildDocDatSettings settingsA = new BuildDocDatSettings(records[1], records[0], "DOCID", null, String.Empty);
            BuildDocDatSettings settingsB = new BuildDocDatSettings(records[1], records[0], String.Empty, null, String.Empty);
            Document docA = builder.BuildDocument(settingsA);
            Document docB = builder.BuildDocument(settingsB);
            Assert.AreEqual(records[1][0], docA.Key);
            Assert.AreEqual(records[1][0], docB.Key);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "The document record does not contain the correct number of fields.")]
        public void Builders_DatBuilder_BuildDocument_Ex()
        {
            // this test is going to make the record one field larger than the header
            string[] record = new string[records[1].Length + 1];
            records[1].CopyTo(record, 0);
            string[] header = records[0];
            BuildDocDatSettings settings = new BuildDocDatSettings(
                record, header, "DOCID", null, String.Empty);
            Document doc = builder.BuildDocument(settings);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Broken families, 1 children have disowned their parents.")]

        public void Builders_DatBuilder_BuildDocuments_ExDisownedParents()
        {
            List<string[]> newRecords = new List<string[]>();
            records.ForEach(r => {
                string[] record = new string[r.Length];
                r.CopyTo(record, 0);
                newRecords.Add(record);
            });

            newRecords[3] = new string[] { "DOC000004", "", "", "VOL001", "3", "", "c", "BUCKLE", "m" };

            bool hasHeader = true;
            string keyColName = "DOCID";
            string parentColName = "BEGATT";
            string childColName = "ATTCHIDS";
            string childColDelim = ";";
            BuildDocCollectionDatSettings args = new BuildDocCollectionDatSettings(
                newRecords, String.Empty, hasHeader, keyColName, parentColName, childColName, childColDelim, null);
            List<Document> documents = builder.BuildDocuments(args);
            DocumentCollection docs = new DocumentCollection(documents);
        }

        [TestMethod]
        public void Builders_DatBuilder_BuildDocuments_ParentName()
        {
            // Here we are testing that the document collection is formed correctly
            // when family relationships are formed around the parent id value

            // Arrange
            bool hasHeader = true;
            string keyColName = "DOCID";
            string parentColName = "BEGATT"; // <--- critical variable
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
        public void Builders_DatBuilder_BuildDocuments_ChildName()
        {
            // Here we are testing is the document collection is formed correctly
            // when family relationships are formed around the child id values

            // Arrange
            bool hasHeader = true;
            string keyColName = "DOCID";
            string parentColName = null;
            string childColName = "ATTCHIDS"; // <--- critical variable
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
        
        [TestMethod]
        public void Builders_DatBuilder_settleFamilyDrama()
        {
            // test the pass through
            // non-empty values are tested elsewhere
            // an exception is thrown if these values are not empty
            string parentColName = String.Empty;
            string childColName =  String.Empty;
            TestBuilder testBuilder = new TestBuilder();
            testBuilder.settleFamilyDrama(parentColName, childColName, ";", null, null, null);
        }

        [TestMethod]
        public void Builders_DatBuilder_setFamilyFromParent()
        {
            // test that the parent key is not in the metadata
            // so nothing happens
            BuildDocDatSettings settings = new BuildDocDatSettings(records[1], records[0], "DOCID", null, "");
            Document doc = builder.BuildDocument(settings);
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            string childColName = "";
            TestBuilder testBuilder = new TestBuilder();
            testBuilder.setFamilyFromParent(doc, docs, paternity, "test", childColName, ";");
            Assert.IsTrue(paternity.Count.Equals(0));
            Assert.IsTrue(doc.Parent == null);

            // test that the parent key  refers to itself
            settings = new BuildDocDatSettings(records[2], records[0], "DOCID", null, "");
            doc = builder.BuildDocument(settings);            
            testBuilder.setFamilyFromParent(doc, docs, paternity, "BEGATT", childColName, ";");
            Assert.IsTrue(paternity.Count.Equals(0));
            Assert.IsTrue(doc.Parent == null);
            
            // test that the parent was found in docs, but child col name is empty
            settings = new BuildDocDatSettings(records[3], records[0], "DOCID", null, "");
            Document parent = doc;
            doc = builder.BuildDocument(settings);
            docs.Add(parent.Key, parent);
            testBuilder.setFamilyFromParent(doc, docs, paternity, "BEGATT", childColName, ";");
            Assert.IsTrue(paternity.Count.Equals(0));
            Assert.IsTrue(doc.Parent == parent);

            // test the parent is set, removed from paternity, and doc is added for the disowned check
            childColName = "ATTCHIDS";
            doc = builder.BuildDocument(settings);            
            paternity.Add(doc.Key, parent);
            doc.Metadata[childColName] = "TEST001";
            Assert.IsTrue(docs.ContainsKey(parent.Key));
            Assert.AreEqual(parent, paternity[doc.Key]);
            testBuilder.setFamilyFromParent(doc, docs, paternity, "BEGATT", childColName, ";");
            Assert.IsFalse(paternity.ContainsKey(doc.Key));
            Assert.AreEqual(doc, paternity["TEST001"]);
            Assert.IsTrue(paternity.Count == 1);
            Assert.AreEqual(parent, doc.Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Broken families, the parent (0) disowns a child document (0).")]
        public void Builders_DatBuilder_setFamilyFromParent_ExBrokenFamily()
        {
            BuildDocDatSettings settings = new BuildDocDatSettings(records[2], records[0], "DOCID", null, "");
            Document parent = builder.BuildDocument(settings);
            settings = new BuildDocDatSettings(records[3], records[0], "DOCID", null, "");
            Document doc = builder.BuildDocument(settings);
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            string childColName = "ATTCHIDS";
            docs.Add(parent.Key, parent);
            paternity.Add(doc.Key, parent);
            parent.Metadata[childColName] = String.Empty;
            TestBuilder testBuilder = new TestBuilder();
            testBuilder.setFamilyFromParent(doc, docs, paternity, "BEGATT", childColName, ";");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Broken Families, the parent DOC000002 is missing for the document DOC000004.")]
        public void Builders_DatBuilder_setFamilyFromParent_ExParentMissing()
        {
            // test exception when parent doesn't exist in docs
            BuildDocDatSettings settings = new BuildDocDatSettings(records[3], records[0], "DOCID", null, "");
            Document doc = builder.BuildDocument(settings);
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            string childColName = "";
            TestBuilder testBuilder = new TestBuilder();
            testBuilder.setFamilyFromParent(doc, docs, paternity, "BEGATT", childColName, ";");
        }
        
        [TestMethod]        
        public void Builders_DatBuilder_setFamilyFromChildren()
        {
            // test if childColName is empty
            BuildDocDatSettings settings = new BuildDocDatSettings(records[2], records[0], "DOCID", null, "");
            Document doc = builder.BuildDocument(settings);
            string childColName = String.Empty;
            string childSeparator = ";";
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            TestBuilder testBuilder = new TestBuilder();
            testBuilder.setFamilyFromChildren(doc, childColName, childSeparator, paternity);
            Assert.IsTrue(doc.Parent == null);
            Assert.IsTrue(paternity.Count.Equals(0));

            // test that paternity is not populated when attchids value is empty
            settings = new BuildDocDatSettings(records[1], records[0], "DOCID", null, "");
            doc = builder.BuildDocument(settings);
            childColName = "ATTCHIDS";            
            testBuilder.setFamilyFromChildren(doc, childColName, childSeparator, paternity);
            Assert.IsTrue(doc.Parent == null);
            Assert.IsTrue(paternity.Count.Equals(0));

            // test that paternity is populated when the doc has metadata attchids values
            settings = new BuildDocDatSettings(records[2], records[0], "DOCID", null, "");
            doc = builder.BuildDocument(settings);
            testBuilder.setFamilyFromChildren(doc, childColName, childSeparator, paternity);
            Assert.AreEqual(doc, paternity["DOC000004"]);
            Assert.AreEqual(doc, paternity["DOC000007"]);
            Assert.IsTrue(doc.Parent == null);
            Assert.IsTrue(paternity.Count.Equals(2));

            // test that the parent is set and paternity is updated
            // when the child is processed
            settings = new BuildDocDatSettings(records[3], records[0], "DOCID", null, "");
            Document child = builder.BuildDocument(settings);
            Assert.IsTrue(child.Parent == null);
            Assert.IsTrue(paternity.ContainsKey(child.Key));
            testBuilder.setFamilyFromChildren(child, childColName, childSeparator, paternity);
            Assert.AreEqual(doc, child.Parent);
            Assert.IsFalse(paternity.ContainsKey(child.Key));
            Assert.IsTrue(paternity.Count.Equals(1));
        }        
    }
}
