using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_Core_DocumentCollection
    {
        [TestMethod]
        public void Core_DocumentCollection_Add()
        {
            Document doc1 = new Document(
                "1", 
                null, 
                null, 
                new Dictionary<string, string>() {
                    { "DOCID","1" },
                    { "VOLUME", "test" }
                }, 
                null);
            Document doc2 = new Document(
                "2",
                doc1,
                null,
                new Dictionary<string, string>() {
                    { "DOCID","2" },
                    { "VOLUME", "test" }
                },
                null);
            Document doc3 = new Document(
                "1",
                null,
                new HashSet<Document>() { doc2 },
                new Dictionary<string, string>() {
                    { "DOCID","1" },
                    { "VOLUME", "testOverlay" }
                },
                null);
            DocumentCollection docs = new DocumentCollection();
            docs.Add(doc1);
            Assert.AreEqual(0, docs.ParentCount);
            Assert.AreEqual(0, docs.ChildCount);
            docs.Add(doc2);
            Assert.AreEqual(0, docs.ParentCount);
            Assert.AreEqual(1, docs.ChildCount);
            int parents = docs.ParentCount;
            docs.Add(doc3);
            parents = docs.ParentCount;
            int p = docs.Count(d => d.Parent != null);
            Assert.AreEqual(1, docs.ParentCount);
            Assert.AreEqual(1, docs.ChildCount);
            Assert.AreEqual(2, docs.Count);
            Assert.AreEqual("testOverlay", docs[0].Metadata["VOLUME"]);
        }
    }
}
