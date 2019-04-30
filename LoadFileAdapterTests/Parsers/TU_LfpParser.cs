using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Parsers;
using Moq;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_LfpParser
    {
        private IParser parser = new LfpParser();
        List<string> input;

        public TU_LfpParser()
        {
            setup();
        }

        [TestMethod]
        public void Parsers_LfpParser_Test()
        {
            // arange
            var mockReader = new Mock<TextReader>();
            int calls = 0;
            mockReader
                .Setup(r => r.ReadLine())
                .Returns(() => input[calls])
                .Callback(() => calls++);            
                        
            // act
            List<string[]> docs = parser.Parse(mockReader.Object);

            // assert
            Assert.IsTrue(docs[0].SequenceEqual(new string[] { "IM", "000000001", "D", "1", "@001", "IMG_0001", "000000001.pdf", "7", "0" }));            
            Assert.IsTrue(docs[1].SequenceEqual(new string[] { "IM", "000000002", "",  "2", "@001", "IMG_0001", "000000001.pdf", "7", "0" }));
            Assert.IsTrue(docs[2].SequenceEqual(new string[] { "IM", "000000003", "",  "3", "@001", "IMG_0001", "000000001.pdf", "7", "0" }));
        }

        private void setup()
        {
            input = new List<string> {
                "IM,000000001,D,1,@001;IMG_0001;000000001.pdf;7,0",
                "IM,000000002,,2,@001;IMG_0001;000000001.pdf;7,0",
                "IM,000000003,,3,@001;IMG_0001;000000001.pdf;7,0",
                null, null
            };
        }
    }
}
