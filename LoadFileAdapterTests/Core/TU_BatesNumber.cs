using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;

namespace LoadFileAdapterTests.Core
{
    [TestClass]
    public class TU_BatesNumber
    {
        [TestMethod]
        public void Core_BatesNumber()
        {
            BatesNumber bates = new BatesNumber("PREFIX000001.002");
            Assert.AreEqual("PREFIX", bates.Prefix);
            Assert.AreEqual(1, bates.Number);
            Assert.AreEqual(2, bates.Suffix);
            Assert.AreEqual(".", bates.SuffixDelimiter);
            Assert.AreEqual("PREFIX000001.002", bates.ToString());
            Assert.AreEqual(true, bates.HasDelim);
            Assert.AreEqual(true, bates.HasSuffix);
            bates = new BatesNumber("_3_000001.002");
            Assert.AreEqual("_3_", bates.Prefix);
            Assert.AreEqual(1, bates.Number);
            Assert.AreEqual(2, bates.Suffix);
            Assert.AreEqual(".", bates.SuffixDelimiter);
            Assert.AreEqual("_3_000001.002", bates.ToString());
            Assert.AreEqual(true, bates.HasDelim);
            Assert.AreEqual(true, bates.HasSuffix);
            bates = new BatesNumber("TEST000001");
            Assert.AreEqual("TEST", bates.Prefix);
            Assert.AreEqual(1, bates.Number);
            Assert.AreEqual(0, bates.Suffix);
            Assert.AreEqual(String.Empty, bates.SuffixDelimiter);
            Assert.AreEqual("TEST000001", bates.ToString());
            Assert.AreEqual(false, bates.HasDelim);
            Assert.AreEqual(false, bates.HasSuffix);
            bates = new BatesNumber("000001");
            Assert.AreEqual(String.Empty, bates.Prefix);
            Assert.AreEqual(1, bates.Number);
            Assert.AreEqual(0, bates.Suffix);
            Assert.AreEqual(String.Empty, bates.SuffixDelimiter);
            Assert.AreEqual("000001", bates.ToString());
            Assert.AreEqual(false, bates.HasDelim);
            Assert.AreEqual(false, bates.HasSuffix);
        }
    }
}
