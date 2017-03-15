using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileAdapter.Parsers
{
    public class LfpParser : Parser<ParseFileParameters, ParseReaderParameters, ParseLineParameters>
    {
        public List<string[]> Parse(ParseFileParameters parameters)
        {            
            bool detectEncoding = true;
            List<string[]> records = null;

            using (TextReader reader = new StreamReader(parameters.File.FullName, parameters.Encoding, detectEncoding))
            {
                ParseReaderParameters readerParameters = new ParseReaderParameters(reader);
                records = Parse(readerParameters);
            }

            return records;
        }

        public List<string[]> Parse(ParseReaderParameters parameters)
        {            
            List<string[]> records = new List<string[]>();                
            string line = String.Empty;

            while ((line = parameters.Reader.ReadLine()) != null)
            {
                ParseLineParameters lineParameters = new ParseLineParameters(line);
                string[] record = ParseLine(lineParameters);
                records.Add(record);
            }            

            return records;
        }

        public string[] ParseLine(ParseLineParameters parameters)
        {
            return parameters.Line.Split(new char[] { ';', ',' });
        }                
    }
}
