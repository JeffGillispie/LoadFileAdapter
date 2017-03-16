using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileAdapter.Parsers
{
    public class LfpParser : Parser<ParseFileSetting, ParseReaderSetting, ParseLineSetting>
    {
        public virtual List<string[]> Parse(ParseFileSetting args)
        {            
            bool detectEncoding = true;
            List<string[]> records = null;

            using (TextReader reader = new StreamReader(args.File.FullName, args.Encoding, detectEncoding))
            {
                ParseReaderSetting readerArgs = new ParseReaderSetting(reader);
                records = Parse(readerArgs);
            }

            return records;
        }

        public virtual List<string[]> Parse(ParseReaderSetting args)
        {            
            List<string[]> records = new List<string[]>();                
            string line = String.Empty;

            while ((line = args.Reader.ReadLine()) != null)
            {
                ParseLineSetting lineParameters = new ParseLineSetting(line);
                string[] record = ParseLine(lineParameters);
                records.Add(record);
            }            

            return records;
        }

        public string[] ParseLine(ParseLineSetting args)
        {
            return args.Line.Split(new char[] { ';', ',' });
        }                
    }
}
