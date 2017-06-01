using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;

namespace LoadFileAdapterTests.Core
{
    [TestClass]
    public class TU_Document
    {
        [TestMethod]
        public void Core_Document_SetParent()
        {
            Document docA = new Document("1", null, null, null, null);
            Document docB = new Document("2", null, null, null, null);

            Assert.AreEqual(null, docA.Children);
            Assert.AreEqual(null, docB.Parent);
            docB.SetParent(docA);
            Assert.AreEqual(docA, docB.Parent);
            Assert.IsTrue(docA.Children.Contains(docB));
        }
    }
}
