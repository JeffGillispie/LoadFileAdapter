using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_Overlayer
    {
        [TestMethod]
        public void Transformers_Overlayer_Familes()
        {
            Overlayer overlayer = new Overlayer(true, false, false);
            Document docA = new Document("1", null, null, new Dictionary<string, string>() { { "VOLUME", "test" } }, null);
            Document docB = new Document("1", null, null, new Dictionary<string, string>() { { "VOLUME", "overlay" } }, null);
            Document child = new Document("2", null, null, null, null);
            child.SetParent(docB);
            docB.Children.Add(child);

            Assert.IsFalse(child.Parent == docA);
            Assert.IsTrue(docA.Children == null);
            Assert.IsTrue(docB.Children.Contains(child));
            docA = overlayer.Overlay(docA, docB);

            Assert.IsTrue(docA.Children.Contains(child));
            Assert.IsTrue(child.Parent == docA);
            Assert.AreEqual("overlay", docB.Metadata["VOLUME"]);
            Assert.AreEqual("test", docA.Metadata["VOLUME"]);
        }

        [TestMethod]
        public void Transformers_Overlayer_MetaData()
        {
            Overlayer overlayer = new Overlayer(false, true, false);
            Document docA = new Document("1", null, null, new Dictionary<string, string>() { { "VOLUME", "test" } }, null);
            Document docB = new Document("1", null, null, new Dictionary<string, string>() { { "VOLUME", "overlay" }, { "DOC", "meh" } }, null);

            Assert.AreEqual("test", docA.Metadata["VOLUME"]);
            docA = overlayer.Overlay(docA, docB);

            Assert.AreEqual("overlay", docA.Metadata["VOLUME"]);
            Assert.AreEqual("meh", docA.Metadata["DOC"]);
        }

        [TestMethod]
        public void Transformers_Overlayer_Representatives()
        {
            Overlayer overlayer = new Overlayer(false, false, true);
            Document docA = new Document("1", null, null, null, 
                new HashSet<Representative>() {
                    new Representative(
                        Representative.FileType.Text, 
                        new SortedDictionary<string, string>() {
                            { "1", "x:\\path\\1.txt" }
                        }
                    )
                }
            );
            Document docB = new Document("1", null, null, null, 
                new HashSet<Representative>() {
                    new Representative(
                        Representative.FileType.Native, 
                        new SortedDictionary<string, string>() {
                            { "1", "x:\\path\\file.ext" }
                        } 
                    )
                }
            );

            Assert.AreEqual(1, docA.Representatives.Count);
            docA = overlayer.Overlay(docA, docB);
            Assert.AreEqual(2, docA.Representatives.Count);

            foreach (var file in docA.Representatives)
            {
                if (file.Type.Equals(Representative.FileType.Native))
                {
                    Assert.AreEqual("x:\\path\\file.ext", file.Files.First().Value);
                }
                else
                {
                    Assert.AreEqual("x:\\path\\1.txt", file.Files.First().Value);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Overlay failure, the document key (1) does not match the overlay key (2).")]
        public void Transformers_Overlayer_Exception()
        {
            Overlayer overlayer = new Overlayer(true, true, true);
            Document docA = new Document("1", null, null, null, null);
            Document docB = new Document("2", null, null, null, null);
            overlayer.Overlay(docA, docB);
        }
    }
}
