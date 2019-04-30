using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// A parser used to parse data from an LFP.
    /// </summary>
    public class LfpParser : IParser
    {
        /// <summary>
        /// Parses a LFP file.
        /// </summary>
        /// <param name="file">The file to parse.</param>
        /// <param name="encoding">The encoding of the file.</param>
        /// <returns>Returns a list of parsed fields.</returns>
        public virtual List<string[]> Parse(FileInfo file, Encoding encoding)
        {
            bool detectEncoding = true;
            List<string[]> records = null;

            using (TextReader reader = new StreamReader(file.FullName, encoding, detectEncoding))
            {                
                records = Parse(reader);
            }

            return records;
        }

        /// <summary>
        /// Parses data from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="reader">The reader used to read the text to be parsed.</param>
        /// <returns>Returns a list of parsed fields.</returns>
        public virtual List<string[]> Parse(TextReader reader)
        {            
            List<string[]> records = new List<string[]>();                
            string line = String.Empty;

            while ((line = reader.ReadLine()) != null)
            {
                string[] record = ParseLine(line);
                records.Add(record);
            }            

            return records;
        }

        /// <summary>
        /// Parses data from a LFP record.
        /// </summary>
        /// <param name="line">The line to be parsed.</param>
        /// <returns>Returns an array of parsed fields.</returns>
        public string[] ParseLine(string line)
        {
            return line.Split(new char[] { ';', ',' });
        }                
    }
}
