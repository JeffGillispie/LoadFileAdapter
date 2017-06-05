using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapterTests.Parsers
{
    [TestClass]
    public class TU_Delimiters
    {        
        [TestMethod]
        [ExpectedException(typeof(Exception), "The field separator and the text qualifier can not have the same value.")]
        public void Parsers_Delimiters_ExceptionFSisTQ()
        {
            var delims = Delimiters.Builder.Start()                
                .SetTextQualifier(',')
                .Build();                
        }
                
        [TestMethod]
        [ExpectedException(typeof(Exception), "The field separator and the new record delimiter can not have the same value.")]
        public void Parsers_Delimiters_ExceptionFSisNR()
        {
            var delims = Delimiters.Builder.Start()
                .SetFieldSeparator('\n')                
                .Build();            
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "The text qualifier and the new record delimiter can not have the same value.")]
        public void Parsers_Delimiters_ExceptionTQisNR()
        {
            var delims = Delimiters.Builder.Start()                
                .SetTextQualifier('\n')
                .Build();            
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "The field separator and the escape character can not have the same value.")]
        public void Parsers_Delimiters_ExceptionFSisEC()
        {
            var delims = Delimiters.Builder.Start()                
                .SetEscapeCharacter(',')
                .Build();            
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "The escape character and the new record delimiter can not have the same value.")]
        public void Parsers_Delimiters_ExceptionECisNR()
        {
            var delims = Delimiters.Builder.Start()
                .SetEscapeCharacter('\n')                
                .Build();
        }
    }
}
