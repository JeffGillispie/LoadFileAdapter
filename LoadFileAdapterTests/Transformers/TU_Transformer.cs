using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_Transformer
    {
        private static List<string[]> records;

        public TU_Transformer()
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
        public void Transformers_Transformer_Test()
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
            List<Transformation> edits = new List<Transformation>();
            IBuilder<BuildDocCollectionDatSettings, BuildDocDatSettings> builder = new DatBuilder();

            // edit 1
            string fieldName = "VOLUME";
            Regex find = new Regex("VOL");
            string replace = "TEST";          
            string altDest = null;
            string prepend = null;
            string append = null;
            string join = null;
            Regex filterText = null;
            string filterField = null;
            DirectoryInfo dir = null;
            edits.Add(new MetaDataTransformation(fieldName, find, replace, altDest, prepend, append, join, filterField, filterText, dir));

            // edit 2
            fieldName = "TEST3";
            find = null;
            replace = String.Empty;
            prepend = "VOLUME";
            edits.Add(new MetaDataTransformation(fieldName, find, replace, altDest, prepend, append, join, filterField, filterText, dir));

            // edit 3            
            fieldName = "TEST1";            
            prepend = String.Empty;
            append = "VOLUME";
            filterText = new Regex("a");
            filterField = "TEST1";            
            join = "-";
            edits.Add(new MetaDataTransformation(fieldName, find, replace, altDest, prepend, append, join, filterField, filterText, dir));

            // edit 4
            fieldName = "TEST2";
            append = String.Empty;
            altDest = "VOLUME";
            filterText = new Regex("j");
            edits.Add(new MetaDataTransformation(fieldName, find, replace, altDest, prepend, append, join, filterField, filterText, dir));

            // edit 5
            altDest = String.Empty;
            filterText = null;
            find = new Regex("E+$");
            replace = "x";
            edits.Add(new MetaDataTransformation(fieldName, find, replace, altDest, prepend, append, join, filterField, filterText, dir));

            // edit 6
            edits.Add(new RepresentativeTransformation(
                Representative.FileType.Native, 
                null, 
                new Regex("X:\\\\VOL001\\\\"), String.Empty, String.Empty, null));

            // act
            List<Document> documents = builder.BuildDocuments(args);
            DocumentCollection docs = new DocumentCollection(documents);
            Transformer transformer = new Transformer();
            transformer.Transform(docs, edits.ToArray());

            // assert
            Assert.AreEqual("TEST001", docs[0].Metadata["VOLUME"]);
            Assert.AreEqual("TEST001k", docs[0].Metadata["TEST3"]);
            Assert.AreEqual("a-TEST001", docs[0].Metadata["TEST1"]);
            Assert.AreEqual("b", docs[1].Metadata["TEST1"]);
            Assert.AreEqual("DOOR", docs[9].Metadata["VOLUME"]);
            Assert.AreEqual("DOOR", docs[9].Metadata["TEST2"]);
            Assert.AreEqual("BUCKLx", docs[2].Metadata["TEST2"]);
            Assert.AreEqual("SHOx", docs[4].Metadata["TEST2"]);
            Assert.AreEqual("THRx", docs[5].Metadata["TEST2"]);
            Assert.AreEqual("THx", docs[8].Metadata["TEST2"]);
            Assert.AreEqual(Representative.FileType.Native, docs[0].Representatives.First().Type);
            Assert.AreEqual("NATIVE\\0001\\DOC000001.XLSX", docs[0].Representatives.First().Files.First().Value);
        }        
    }
}
