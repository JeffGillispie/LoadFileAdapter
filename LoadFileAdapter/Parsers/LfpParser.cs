using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// A parser used to parse data from an LFP.
    /// </summary>
    public class LfpParser : IParser<ParseFileSettings, ParseReaderSettings, ParseLineSettings>
    {
        /// <summary>
        /// Parses a LFP read from a file into a list of parsed records.
        /// </summary>
        /// <param name="args">Contains the file to be parsed and the encoding
        /// used to read it.</param>
        /// <returns>Returns a list of parsed records.</returns>
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

        /// <summary>
        /// Parses a LFP read from a <see cref="TextReader"/> into a list of
        /// parsed records.
        /// </summary>
        /// <param name="args">Contains the <see cref="TextReader"/>.</param>
        /// <returns>Returns a list of parsed records.</returns>
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

        /// <summary>
        /// Parses data from a LFP record.
        /// </summary>
        /// <param name="args">Contains the line to be parsed.</param>
        /// <returns>Returns an array of parsed fields.</returns>
        public string[] ParseLine(ParseLineSettings args)
        {
            return args.Line.Split(new char[] { ';', ',' });
        }                
    }
}
