using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_LfpBuilder
    {
        private static List<string[]> SinglePageLfpRecords;
        private static List<string[]> MultiPageLfpRecords;
        private LfpBuilder builder = new LfpBuilder();

        public TU_LfpBuilder()
        {
            SinglePageLfpRecords = new List<string[]> {
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
            MultiPageLfpRecords = new List<string[]> {
                new string[] { "IM", "DOC000001", "D", "1", "@VOL001", "IMAGES\\0001", "DOC000001.pdf", "7", "0" },
                new string[] { "OF", "DOC000001", "@VOL001", "NATIVE\\0001", "DOC000001.XLSX", "1" },
                new string[] { "IM", "DOC000002", "D", "1", "@VOL001", "IMAGES\\0001", "DOC000002.tif", "7", "0" },
                new string[] { "IM", "DOC000003", " ", "2", "@VOL001", "IMAGES\\0001", "DOC000002.pdf", "7", "0" },
                new string[] { "IM", "DOC000004", "C", "1", "@VOL001", "IMAGES\\0001", "DOC000004.pdf", "7", "0" },
                new string[] { "IM", "DOC000005", "",  "2", "@VOL001", "IMAGES\\0001", "DOC000004.pdf", "7", "0" },
                new string[] { "IM", "DOC000006", "",  "3", "@VOL001", "IMAGES\\0001", "DOC000004.pdf", "7", "0" },
                new string[] { "IM", "DOC000007", "C", "1", "@VOL001", "IMAGES\\0001", "DOC000007.pdf", "7", "0" },
                new string[] { "IM", "DOC000008", "",  "2", "@VOL001", "IMAGES\\0001", "DOC000007.pdf", "7", "0" },
                new string[] { "IM", "DOC000009", "",  "3", "@VOL001", "IMAGES\\0001", "DOC000007.pdf", "7", "0" },
                new string[] { "IM", "DOC000010", "",  "4", "@VOL001", "IMAGES\\0001", "DOC000007.pdf", "7", "0" },
                new string[] { "IM", "DOC000011", "D", "1", "@VOL001", "IMAGES\\0001", "DOC000011.pdf", "7", "0" },
                new string[] { "IM", "DOC000012", "",  "2", "@VOL001", "IMAGES\\0001", "DOC000011.pdf", "7", "0" },
                new string[] { "IM", "DOC000013", "",  "3", "@VOL001", "IMAGES\\0001", "DOC000011.pdf", "7", "0" },
                new string[] { "IM", "DOC000014", "",  "4", "@VOL001", "IMAGES\\0001", "DOC000011.pdf", "7", "0" },
                new string[] { "IM", "DOC000015", "C", "1", "@VOL001", "IMAGES\\0001", "DOC000015.pdf", "7", "0" },
                new string[] { "IM", "DOC000016", "D", "1", "@VOL001", "IMAGES\\0001", "DOC000016.pdf", "7", "0" },
                new string[] { "IM", "DOC000017", "",  "2", "@VOL001", "IMAGES\\0001", "DOC000016.pdf", "7", "0" },
                new string[] { "IM", "DOC000018", "D", "1", "@VOL001", "IMAGES\\0001", "DOC000018.pdf", "7", "0" },
                new string[] { "IM", "DOC000019", "",  "2", "@VOL001", "IMAGES\\0001", "DOC000018.pdf", "7", "0" },
                new string[] { "IM", "DOC000020", "",  "3", "@VOL001", "IMAGES\\0001", "DOC000018.pdf", "7", "0" },
                new string[] { "IM", "DOC000021", "C", "1", "@VOL001", "IMAGES\\0001", "DOC000021.pdf", "7", "0" },
                new string[] { "IM", "DOC000022", "",  "2", "@VOL001", "IMAGES\\0001", "DOC000021.pdf", "7", "0" },
                new string[] { "IM", "DOC000023", "",  "3", "@VOL001", "IMAGES\\0001", "DOC000021.pdf", "7", "0" },
                new string[] { "IM", "DOC000024", "",  "4", "@VOL001", "IMAGES\\0001", "DOC000021.pdf", "7", "0" },
                new string[] { "IM", "DOC000025", "C", "1", "@VOL001", "IMAGES\\0001", "DOC000025.pdf", "7", "0" }
            };
        }

        [TestMethod]
        public void Builders_LfpBuilder_SinglePageNoText()
        {               
            builder.PathPrefix = "X:\\VOL001";
            builder.TextBuilder = null;
            List<Document> documents = builder.Build(SinglePageLfpRecords);
            DocumentCollection docs = new DocumentCollection(documents);            
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(25, docs.ImageCount);
            Assert.AreEqual(0, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);          
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);
            Assert.AreEqual(
                "X:\\VOL001\\IMAGES\\0001\\DOC000003.tif", 
                docs[1].Representatives.Last().Files.Last().Value);
            Assert.AreEqual(0, docs[1].Representatives.Count(r => r.Type.Equals(Representative.FileType.Text)));
        }

        [TestMethod]
        public void Builders_LfpBuilder_SinglePageWithPageText()
        {            
            TextBuilder textBuilder = new TextBuilder(
                TextBuilder.TextLevel.Page,
                TextBuilder.TextLocation.SameAsImages, 
                null,   // find
                null);  // replace
            builder.PathPrefix = String.Empty;
            builder.TextBuilder = textBuilder;
            List<Document> documents = builder.Build(SinglePageLfpRecords);
            DocumentCollection docs = new DocumentCollection(documents);            
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(25, docs.ImageCount);
            Assert.AreEqual(25, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);
            var rep = docs[1].Representatives.Where(r => r.Type.Equals(Representative.FileType.Text)).First();
            Assert.AreEqual(
                "IMAGES\\0001\\DOC000003.txt", 
                rep.Files.Last().Value);
            Assert.AreEqual(2, docs[1].Representatives.Count);
        }

        [TestMethod]
        public void Builders_LfpBuilder_SinglePageWithDocText()
        {
            TextBuilder textBuilder = new TextBuilder(
                TextBuilder.TextLevel.Doc,
                TextBuilder.TextLocation.SameAsImages,
                null,   // find
                null);  // replace
            builder.PathPrefix = String.Empty;
            builder.TextBuilder = textBuilder;            
            List<Document> documents = builder.Build(SinglePageLfpRecords);
            DocumentCollection docs = new DocumentCollection(documents);            
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(25, docs.ImageCount);
            Assert.AreEqual(10, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);
            var rep = docs[1].Representatives.Where(r => r.Type.Equals(Representative.FileType.Text)).First();
            Assert.AreEqual(
                "IMAGES\\0001\\DOC000002.txt",
                rep.Files.Last().Value);
            Assert.AreEqual(2, docs[1].Representatives.Count);
        }

        [TestMethod]
        public void Builders_LfpBuilder_MultiPageNoText()
        {
            // Arrange               
            builder.PathPrefix = "X:\\VOL001";
            builder.TextBuilder = null;            
            List<Document> documents = builder.Build(MultiPageLfpRecords);
            DocumentCollection docs = new DocumentCollection(documents);            
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(10, docs.ImageCount);
            Assert.AreEqual(0, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);            
            Assert.AreEqual(
                "X:\\VOL001\\IMAGES\\0001\\DOC000002.tif",
                docs[1].Representatives.Last().Files.Last().Value);
        }

        [TestMethod]
        public void Builders_LfpBuilder_MultiPageWithDocText()
        {
            builder.PathPrefix = null;                     
            builder.TextBuilder = new TextBuilder(
                TextBuilder.TextLevel.Doc,
                TextBuilder.TextLocation.SameAsImages,
                null, null);            
            List<Document> documents = builder.Build(MultiPageLfpRecords);
            DocumentCollection docs = new DocumentCollection(documents);
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(10, docs.ImageCount);
            Assert.AreEqual(10, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);
            var rep = docs[2].Representatives.Where(r => r.Type.Equals(Representative.FileType.Text)).First();
            Assert.AreEqual(
                "IMAGES\\0001\\DOC000004.txt",
                rep.Files.Last().Value);
        }

        [TestMethod]
        public void Builders_LfpBuilder_MultiPageWithPageText()
        {           
            builder.PathPrefix = "";
            builder.TextBuilder = new TextBuilder(
                TextBuilder.TextLevel.Doc,
                TextBuilder.TextLocation.AlternateLocation,
                new Regex("IMAGES\\\\"), "TEXT\\");            
            List<Document> documents = builder.Build(MultiPageLfpRecords);
            DocumentCollection docs = new DocumentCollection(documents);
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(10, docs.ImageCount);
            Assert.AreEqual(10, docs.TextCount);
            Assert.AreEqual(1, docs.NativeCount);
            Assert.AreEqual(3, docs.ParentCount);
            Assert.AreEqual(5, docs.ChildCount);
            Assert.AreEqual(2, docs.StandAloneCount);
            var rep = docs[2].Representatives.Where(r => r.Type.Equals(Representative.FileType.Text)).First();
            Assert.AreEqual(
                "TEXT\\0001\\DOC000004.txt",
                rep.Files.Last().Value);
        }
    }
}
