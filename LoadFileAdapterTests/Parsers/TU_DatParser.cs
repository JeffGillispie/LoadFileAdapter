using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Parsers;
using Moq;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_DatParser
    {
        private IParser parser;
        private List<string> concordanceInput;
        private List<string> csvInput;
        private List<string> qualifiedInput;

        public TU_DatParser()
        {
            setup();            
        }

        [TestMethod]
        public void Parsers_DatParser_Concordance()
        {
            // arange
            var mockReader = new Mock<TextReader>();
            int calls = 0;            
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => concordanceInput[calls])
                .Callback(() => calls++);            
            this.parser = new DatParser(Delimiters.CONCORDANCE);            

            // act
            List<string[]> docs = parser.Parse(mockReader.Object);

            // assert
            Assert.IsTrue(docs[0].SequenceEqual(new string[] { "DOCID", "BEGATT", "VOLUME" }));
            Assert.IsTrue(docs[1].SequenceEqual(new string[] { "DOC000001", "DOC000001", "VOL001" }));
            Assert.IsTrue(docs[2].SequenceEqual(new string[] { "DOC000002", "DOC000001", "VOL001" }));
        }

        [TestMethod]
        public void Parsers_DatParser_CSV()
        {
            // arange
            var mockReader = new Mock<TextReader>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => csvInput[calls])
                .Callback(() => calls++);
            this.parser = new DatParser(Delimiters.COMMA_DELIMITED);
            
            // act
            List<string[]> docs = parser.Parse(mockReader.Object);

            // assert
            Assert.IsTrue(docs[0].SequenceEqual(new string[] { "DOCID", "BEGATT", "VOLUME", "EMPTY1", "EMPTY2", "EMPTY3" }));
            Assert.IsTrue(docs[1].SequenceEqual(new string[] { "DOC000001", "DOC000001", "VOL001", "", "", "" }));
            Assert.IsTrue(docs[2].SequenceEqual(new string[] { "DOC000002", "DOC000001", "VOL001", "", "SURPRISE", "" }));
        }

        [TestMethod]
        public void Parsers_DatParser_Qualified()
        {
            // arange
            var mockReader = new Mock<TextReader>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => qualifiedInput[calls])
                .Callback(() => calls++);
            this.parser = new DatParser(Delimiters.COMMA_QUOTE);

            // act
            List<string[]> docs = parser.Parse(mockReader.Object);

            // assert
            Assert.IsTrue(docs[0].SequenceEqual(new string[] { "DOCID", "BEGATT", "VOLUME", "EMPTY1", "EMPTY2", "EMPTY3" }));
            Assert.IsTrue(docs[1].SequenceEqual(new string[] { "DOC000001", "DOC000001", "VOL001", "", "", "" }));
            Assert.IsTrue(docs[2].SequenceEqual(new string[] { "DOC000002", "DOC000001", "VOL001,EXTRA", "", "SURPRISE", "" }));
        }

        private void setup()
        {
            concordanceInput = new List<string>(new string[] {
                "þDOCIDþþBEGATTþþVOLUMEþ",
                "þDOC000001þþDOC000001þþVOL001þ",
                "þDOC000002þþDOC000001þþVOL001þ",
                null,null
            });

            csvInput = new List<string>(new string[]
            {
                "DOCID,BEGATT,VOLUME,EMPTY1,EMPTY2,EMPTY3",
                "DOC000001,DOC000001,VOL001,,,",
                "DOC000002,DOC000001,VOL001,,SURPRISE,",
                null, null
            });

            qualifiedInput = new List<string>(new string[] {
                "\"DOCID\",\"BEGATT\",\"VOLUME\",\"EMPTY1\",\"EMPTY2\",\"EMPTY3\"",
                "\"DOC000001\",\"DOC000001\",\"VOL001\",\"\",\"\",\"\"",
                "\"DOC000002\",\"DOC000001\",\"VOL001,EXTRA\",\"\",\"SURPRISE\",\"\"",
                null, null
            });
        }
    }
}
