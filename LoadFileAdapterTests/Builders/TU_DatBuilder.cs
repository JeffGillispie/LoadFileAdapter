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
        private TestBuilder builder = new TestBuilder();

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
            public new void SetHeader(string[] header)
            {
                base.SetHeader(header);
            }

            public new List<Document> Build(IEnumerable<string[]> records)
            {
                return base.Build(records);
            }

            public new void settleFamilyDrama(
                Document doc, Dictionary<string, Document> docs, Dictionary<string, Document> paternity)
            {
                base.settleFamilyDrama(doc, docs, paternity);
            }
            public new void setFamilyFromParent(
                Document doc, Dictionary<string, Document> docs, Dictionary<string, Document> paternity)
            {
                base.setFamilyFromParent(doc, docs, paternity);
            }
            public new void setFamilyFromChildren(Document doc, Dictionary<string, Document> paternity)
            {
                base.setFamilyFromChildren(doc, paternity);
            }
        }

        [TestMethod]
        public void Builders_DatBuilder_BuildDocument_Reps()
        {
            // test with an empty path prefix
            RepresentativeBuilder rep = new RepresentativeBuilder(
                "NATIVE", Representative.FileType.Native);            
            builder.RepresentativeBuilders = new List<RepresentativeBuilder>() { rep };
            builder.SetHeader(records[0]);
            Document doc = builder.BuildDocument(records[1]);            
            Assert.AreEqual(Representative.FileType.Native, doc.Representatives.First().Type);
            Assert.AreEqual(records[1][0], doc.Representatives.First().Files.First().Key);
            string expected = "X:\\VOL001\\NATIVE\\0001\\DOC000001.XLSX";
            Assert.AreEqual(expected, doc.Representatives.First().Files.First().Value);

            // test with a populated path prefix
            string[] newRecord = new string[records[1].Length];
            records[1].CopyTo(newRecord, 0);
            newRecord[5] = "\\VOL001\\NATIVE\\0001\\DOC000001.XLSX";            
            builder.RepresentativeBuilders = new RepresentativeBuilder[] { rep };
            builder.KeyColumnName = "DOCID";
            builder.PathPrefix = "Z:\\";
            doc = builder.BuildDocument(newRecord);
            Assert.AreEqual(Representative.FileType.Native, doc.Representatives.First().Type);
            Assert.AreEqual(records[1][0], doc.Representatives.First().Files.First().Key);
            expected = "Z:\\VOL001\\NATIVE\\0001\\DOC000001.XLSX";
            Assert.AreEqual(expected, doc.Representatives.First().Files.First().Value);
        }

        [TestMethod]
        public void Builders_DatBuilder_BuildDocument_Metadata()
        {
            // test that the document metadata is populated correctly            
            builder.SetHeader(records[0]);
            builder.KeyColumnName = "DOCID";
            Document doc = builder.BuildDocument(records[2]);

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
            builder.SetHeader(records[0]);
            builder.KeyColumnName = "DOCID";
            Document docA = builder.BuildDocument(records[1]);
            builder.KeyColumnName = String.Empty;
            Document docB = builder.BuildDocument(records[1]);
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
            builder.KeyColumnName = "DOCID";
            builder.SetHeader(header);
            Document doc = builder.BuildDocument(record);
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
            builder.HasHeader = true;
            builder.KeyColumnName = "DOCID";
            builder.ParentColumnName = "BEGATT";
            builder.ChildColumnName = "ATTCHIDS";
            builder.ChildSeparator = ";";
            List<Document> documents = builder.Build(newRecords);
            DocumentCollection docs = new DocumentCollection(documents);
        }

        [TestMethod]
        public void Builders_DatBuilder_BuildDocuments_ParentName()
        {
            // Here we are testing that the document collection is formed correctly
            // when family relationships are formed around the parent id value            
            builder.HasHeader = true;
            builder.KeyColumnName = "DOCID";
            builder.ParentColumnName = "BEGATT";
            builder.ChildColumnName = String.Empty;
            builder.ChildSeparator = ";";
            builder.RepresentativeBuilders = new RepresentativeBuilder[] {
                new RepresentativeBuilder("NATIVE", Representative.FileType.Native)
            };
            builder.PathPrefix = String.Empty;
            List<Document> documents = builder.Build(records);
            DocumentCollection docs = new DocumentCollection(documents);
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
            string pathPrefix = String.Empty;
            builder.HasHeader = true;
            builder.KeyColumnName = "DOCID";
            builder.ParentColumnName = null;
            builder.ChildColumnName = "ATTCHIDS"; // <-- critical
            builder.ChildSeparator = ";";
            builder.PathPrefix = String.Empty;
            builder.RepresentativeBuilders = new RepresentativeBuilder[] {
                new RepresentativeBuilder("NATIVE", Representative.FileType.Native)
                };
            
            // Act
            List<Document> documents = builder.Build(records);
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
            testBuilder.settleFamilyDrama(null, null, null);
        }

        [TestMethod]
        public void Builders_DatBuilder_setFamilyFromParent()
        {
            // test that the parent key is not in the metadata
            // so nothing happens            
            builder.SetHeader(records[0]);
            builder.KeyColumnName = "DOCID";
            builder.RepresentativeBuilders = null;
            builder.PathPrefix = "";
            builder.SetHeader(records[0]);
            Document doc = builder.BuildDocument(records[1]);
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            string childColName = "";
            TestBuilder testBuilder = new TestBuilder();
            testBuilder.ParentColumnName = "test";
            testBuilder.ChildColumnName = childColName;
            testBuilder.ChildSeparator = ";";
            testBuilder.setFamilyFromParent(doc, docs, paternity);
            Assert.IsTrue(paternity.Count.Equals(0));
            Assert.IsTrue(doc.Parent == null);

            // test that the parent key  refers to itself            
            builder.KeyColumnName = "DOCID";
            builder.RepresentativeBuilders = null;
            builder.PathPrefix = "";
            doc = builder.BuildDocument(records[2]);
            testBuilder.ParentColumnName = "BEGATT";
            testBuilder.ChildColumnName = childColName;
            testBuilder.ChildSeparator = ";";                   
            testBuilder.setFamilyFromParent(doc, docs, paternity);
            Assert.IsTrue(paternity.Count.Equals(0));
            Assert.IsTrue(doc.Parent == null);
            
            // test that the parent was found in docs, but child col name is empty            
            builder.KeyColumnName = "DOCID";
            builder.RepresentativeBuilders = null;
            builder.PathPrefix = "";
            Document parent = doc;
            doc = builder.BuildDocument(records[3]);
            docs.Add(parent.Key, parent);
            testBuilder.ParentColumnName = "BEGATT";
            testBuilder.ChildColumnName = childColName;
            testBuilder.ChildSeparator = ";";
            testBuilder.setFamilyFromParent(doc, docs, paternity);
            Assert.IsTrue(paternity.Count.Equals(0));
            Assert.IsTrue(doc.Parent == parent);

            // test the parent is set, removed from paternity, and doc is added for the disowned check
            childColName = "ATTCHIDS";
            testBuilder.ChildColumnName = childColName;
            doc = builder.BuildDocument(records[3]);            
            paternity.Add(doc.Key, parent);
            doc.Metadata[childColName] = "TEST001";
            Assert.IsTrue(docs.ContainsKey(parent.Key));
            Assert.AreEqual(parent, paternity[doc.Key]);
            testBuilder.setFamilyFromParent(doc, docs, paternity);
            Assert.IsFalse(paternity.ContainsKey(doc.Key));
            Assert.AreEqual(doc, paternity["TEST001"]);
            Assert.IsTrue(paternity.Count == 1);
            Assert.AreEqual(parent, doc.Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Broken families, the parent (0) disowns a child document (0).")]
        public void Builders_DatBuilder_setFamilyFromParent_ExBrokenFamily()
        {            
            builder.KeyColumnName = "DOCID";
            builder.RepresentativeBuilders = null;
            builder.PathPrefix = "";
            builder.SetHeader(records[0]);
            Document parent = builder.BuildDocument(records[2]);                        
            Document doc = builder.BuildDocument(records[3]);
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            string childColName = "ATTCHIDS";
            docs.Add(parent.Key, parent);
            paternity.Add(doc.Key, parent);
            parent.Metadata[childColName] = String.Empty;
            TestBuilder testBuilder = new TestBuilder();
            testBuilder.ParentColumnName = "BEGATT";
            testBuilder.ChildColumnName = childColName;
            testBuilder.ChildSeparator = ";";
            testBuilder.setFamilyFromParent(doc, docs, paternity);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Broken Families, the parent DOC000002 is missing for the document DOC000004.")]
        public void Builders_DatBuilder_setFamilyFromParent_ExParentMissing()
        {
            // test exception when parent doesn't exist in docs
            builder.KeyColumnName = "DOCID";
            builder.RepresentativeBuilders = null;
            builder.PathPrefix = "";
            builder.SetHeader(records[0]);
            Document doc = builder.BuildDocument(records[3]);
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            string childColName = "";
            TestBuilder testBuilder = new TestBuilder();
            testBuilder.ParentColumnName = "BEGATT";
            testBuilder.ChildColumnName = childColName;
            testBuilder.ChildSeparator = ";";
            testBuilder.setFamilyFromParent(doc, docs, paternity);
        }
        
        [TestMethod]        
        public void Builders_DatBuilder_setFamilyFromChildren()
        {
            // test if childColName is empty            
            builder.RepresentativeBuilders = null;
            builder.PathPrefix = "";
            builder.KeyColumnName = "DOCID";
            builder.SetHeader(records[0]);
            Document doc = builder.BuildDocument(records[2]);
            string childColName = String.Empty;
            string childSeparator = ";";
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            TestBuilder testBuilder = new TestBuilder();
            testBuilder.ChildColumnName = childColName;
            testBuilder.ChildSeparator = childSeparator;            
            testBuilder.setFamilyFromChildren(doc, paternity);
            Assert.IsTrue(doc.Parent == null);
            Assert.IsTrue(paternity.Count.Equals(0));

            // test that paternity is not populated when attchids value is empty            
            builder.SetHeader(records[0]);
            builder.KeyColumnName = "DOCID";
            builder.RepresentativeBuilders = null;
            builder.PathPrefix = "";
            doc = builder.BuildDocument(records[1]);
            childColName = "ATTCHIDS";
            testBuilder.ChildColumnName = childColName;
            testBuilder.ChildSeparator = childSeparator;         
            testBuilder.setFamilyFromChildren(doc, paternity);
            Assert.IsTrue(doc.Parent == null);
            Assert.IsTrue(paternity.Count.Equals(0));

            // test that paternity is populated when the doc has metadata attchids values            
            doc = builder.BuildDocument(records[2]);
            testBuilder.ChildColumnName = childColName;
            testBuilder.ChildSeparator = childSeparator;            
            testBuilder.setFamilyFromChildren(doc, paternity);
            Assert.AreEqual(doc, paternity["DOC000004"]);
            Assert.AreEqual(doc, paternity["DOC000007"]);
            Assert.IsTrue(doc.Parent == null);
            Assert.IsTrue(paternity.Count.Equals(2));

            // test that the parent is set and paternity is updated
            // when the child is processed            
            Document child = builder.BuildDocument(records[3]);
            Assert.IsTrue(child.Parent == null);
            Assert.IsTrue(paternity.ContainsKey(child.Key));
            testBuilder.ChildColumnName = childColName;
            testBuilder.ChildSeparator = childSeparator;
            testBuilder.setFamilyFromChildren(child, paternity);
            Assert.AreEqual(doc, child.Parent);
            Assert.IsFalse(paternity.ContainsKey(child.Key));
            Assert.IsTrue(paternity.Count.Equals(1));
        }        
    }
}
