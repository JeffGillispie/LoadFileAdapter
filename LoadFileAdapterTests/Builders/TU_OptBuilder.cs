using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Builders;

namespace LoadFileAdapterTests
{    
    [TestClass]
    public class TU_OptBuilder
    {
        private static List<string[]> OptRecords;
        private OptBuilder builder = new OptBuilder();

        public TU_OptBuilder()
        {
            OptRecords = new List<string[]> {
                new string[] { "DOC000001", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000001.tif", "Y", "", "", "1" },
                new string[] { "DOC000002", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000002.tif", "Y", "", "", "2" },
                new string[] { "DOC000003", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000003.tif", "",  "", "", ""  },
                new string[] { "DOC000004", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000004.tif", "Y", "", "", "3" },
                new string[] { "DOC000005", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000005.tif", "",  "", "", ""  },
                new string[] { "DOC000006", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000006.tif", "",  "", "", ""  },
                new string[] { "DOC000007", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000007.tif", "Y", "", "", "4" },
                new string[] { "DOC000008", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000008.tif", "",  "", "", ""  },
                new string[] { "DOC000009", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000009.tif", "",  "", "", ""  },
                new string[] { "DOC000010", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000010.tif", "",  "", "", ""  },
                new string[] { "DOC000011", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000011.tif", "Y", "", "", "4" },
                new string[] { "DOC000012", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000012.tif", "",  "", "", ""  },
                new string[] { "DOC000013", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000013.tif", "",  "", "", ""  },
                new string[] { "DOC000014", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000014.tif", "",  "", "", ""  },
                new string[] { "DOC000015", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000015.tif", "Y", "", "", "1" },
                new string[] { "DOC000016", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000016.tif", "Y", "", "", "2" },
                new string[] { "DOC000017", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000017.tif", "",  "", "", ""  },
                new string[] { "DOC000018", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000018.tif", "Y", "", "", "3" },
                new string[] { "DOC000019", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000019.tif", "",  "", "", ""  },
                new string[] { "DOC000020", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000020.tif", "",  "", "", ""  },
                new string[] { "DOC000021", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000021.tif", "Y", "", "", "4" },
                new string[] { "DOC000022", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000022.tif", "",  "", "", ""  },
                new string[] { "DOC000023", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000023.tif", "",  "", "", ""  },
                new string[] { "DOC000024", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000024.tif", "",  "", "", ""  },
                new string[] { "DOC000025", "VOL001", "X:\\VOL001\\IMAGES\\0001\\DOC000025.tif", "Y", "", "", "1" }
            };
        }

        [TestMethod]
        public void Builders_OptBuilder_NoText()
        {
            builder.PathPrefix = String.Empty;
            builder.TextBuilder = null;                         
            List<Document> documents = builder.Build(OptRecords);
            DocumentCollection docs = new DocumentCollection(documents);
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(25, docs.ImageCount);
            Assert.AreEqual(0, docs.TextCount);
            Assert.AreEqual(0, docs.NativeCount);
            Assert.AreEqual(0, docs.ParentCount);
            Assert.AreEqual(0, docs.ChildCount);
            Assert.AreEqual(10, docs.StandAloneCount);
        }

        [TestMethod]
        public void Builders_OptBuilder_WithPageText()
        {
            builder.PathPrefix = String.Empty;
            builder.TextBuilder = new TextBuilder(
                TextBuilder.TextLevel.Page,
                TextBuilder.TextLocation.SameAsImages,
                null, null);
            List<Document> documents = builder.Build(OptRecords);
            DocumentCollection docs = new DocumentCollection(documents);
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(25, docs.ImageCount);
            Assert.AreEqual(25, docs.TextCount);
            Assert.AreEqual(0, docs.NativeCount);
            Assert.AreEqual(0, docs.ParentCount);
            Assert.AreEqual(0, docs.ChildCount);
            Assert.AreEqual(10, docs.StandAloneCount);
            Assert.AreEqual(
                "X:\\VOL001\\IMAGES\\0001\\DOC000006.txt", 
                docs[2].Representatives.Last().Files.Last().Value);
        }

        [TestMethod]
        public void Builders_OptBuilder_WithDocText()
        {
            builder.PathPrefix = String.Empty;
            builder.TextBuilder = new TextBuilder(
                TextBuilder.TextLevel.Doc,
                TextBuilder.TextLocation.SameAsImages,
                null, null);            
            List<Document> documents = builder.Build(OptRecords);
            DocumentCollection docs = new DocumentCollection(documents);
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(25, docs.ImageCount);
            Assert.AreEqual(10, docs.TextCount);
            Assert.AreEqual(0, docs.NativeCount);
            Assert.AreEqual(0, docs.ParentCount);
            Assert.AreEqual(0, docs.ChildCount);
            Assert.AreEqual(10, docs.StandAloneCount);
            Assert.AreEqual(
                "X:\\VOL001\\IMAGES\\0001\\DOC000004.txt",
                docs[2].Representatives.Last().Files.Last().Value);
        }

        [TestMethod]
        public void Builders_OptBuilder_WithRegexSameLoc()
        {
            builder.PathPrefix = "test preprend";
            builder.TextBuilder = new TextBuilder(
                TextBuilder.TextLevel.Page,
                TextBuilder.TextLocation.SameAsImages,
                new Regex("\\\\IMAGES"),
                "\\TEXT");            
            List<Document> documents = builder.Build(OptRecords);
            DocumentCollection docs = new DocumentCollection(documents);
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(25, docs.ImageCount);
            Assert.AreEqual(25, docs.TextCount);
            Assert.AreEqual(0, docs.NativeCount);
            Assert.AreEqual(0, docs.ParentCount);
            Assert.AreEqual(0, docs.ChildCount);
            Assert.AreEqual(10, docs.StandAloneCount);
            Assert.AreEqual(
                "X:\\VOL001\\IMAGES\\0001\\DOC000006.txt",
                docs[2].Representatives.Last().Files.Last().Value);
        }

        [TestMethod]
        public void Builders_OptBuilder_WithRegexAltLoc()
        {
            builder.PathPrefix = "test preprend";
            builder.TextBuilder = new TextBuilder(
                TextBuilder.TextLevel.Page,
                TextBuilder.TextLocation.AlternateLocation,
                new Regex("\\\\IMAGES"),
                "\\TEXT");            
            List<Document> documents = builder.Build(OptRecords);
            DocumentCollection docs = new DocumentCollection(documents);
            Assert.AreEqual(10, docs.Count);
            Assert.AreEqual(25, docs.ImageCount);
            Assert.AreEqual(25, docs.TextCount);
            Assert.AreEqual(0, docs.NativeCount);
            Assert.AreEqual(0, docs.ParentCount);
            Assert.AreEqual(0, docs.ChildCount);
            Assert.AreEqual(10, docs.StandAloneCount);
            Assert.AreEqual(
                "X:\\VOL001\\TEXT\\0001\\DOC000006.txt",
                docs[2].Representatives.Last().Files.Last().Value);
        }
    }
}
