using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileAdapter.Parsers
{
    public class LfpParser : IParser<ParseFileSettings, ParseReaderSettings, ParseLineSettings>
    {
        public virtual List<string[]> Parse(ParseFileSettings args)
        {            
            bool detectEncoding = true;
            List<string[]> records = null;

            using (TextReader reader = new StreamReader(args.File.FullName, args.Encoding, detectEncoding))
            {
                ParseReaderSettings readerArgs = new ParseReaderSettings(reader);
                records = Parse(readerArgs);
            }

            return records;
        }

        public virtual List<string[]> Parse(ParseReaderSettings args)
        {            
            List<string[]> records = new List<string[]>();                
            string line = String.Empty;

            while ((line = args.Reader.ReadLine()) != null)
            {
                ParseLineSettings lineParameters = new ParseLineSettings(line);
                string[] record = ParseLine(lineParameters);
                records.Add(record);
            }            

            return records;
        }

        public string[] ParseLine(ParseLineSettings args)
        {
            return args.Line.Split(new char[] { ';', ',' });
        }                
    }
}
