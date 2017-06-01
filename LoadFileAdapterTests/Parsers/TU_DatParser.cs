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
        private IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings> parser = new DatParser();
        private List<string> concordanceInput;
        private List<string> csvInput;

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
            Delimiters delimiters = Delimiters.CONCORDANCE;
            ParseReaderDatSettings args = new ParseReaderDatSettings(mockReader.Object, delimiters);

            // act
            List<string[]> docs = parser.Parse(args);

            // assert
            Assert.IsTrue(docs[0].SequenceEqual(new string[] { "DOCID", "BEGATT", "VOLUME" }));
            Assert.IsTrue(docs[1].SequenceEqual(new string[] { "DOC000001", "DOC000001", "VOL001" }));
            Assert.IsTrue(docs[2].SequenceEqual(new string[] { "DOC000002", "DOC000001", "VOL001" }));
        }

        [TestMethod]
        public void Parsers_TabularParser_CSV()
        {
            // arange
            var mockReader = new Mock<TextReader>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => csvInput[calls])
                .Callback(() => calls++);
            Delimiters delimiters = Delimiters.COMMA_DELIMITED;
            ParseReaderDatSettings args = new ParseReaderDatSettings(mockReader.Object, delimiters);

            // act
            List<string[]> docs = parser.Parse(args);

            // assert
            Assert.IsTrue(docs[0].SequenceEqual(new string[] { "DOCID", "BEGATT", "VOLUME" }));
            Assert.IsTrue(docs[1].SequenceEqual(new string[] { "DOC000001", "DOC000001", "VOL001" }));
            Assert.IsTrue(docs[2].SequenceEqual(new string[] { "DOC000002", "DOC000001", "VOL001" }));
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
                "DOCID,BEGATT,VOLUME",
                "DOC000001,DOC000001,VOL001",
                "DOC000002,DOC000001,VOL001",
                null, null
            });
        }
    }
}
