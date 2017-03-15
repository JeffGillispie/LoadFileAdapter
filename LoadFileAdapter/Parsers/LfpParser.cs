using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    public class LfpParser : Parser
    {
        public List<string[]> Parse(FileInfo lfpFile, Delimiters delimiters, Encoding encoding)
        {
            List<string[]> records = new List<string[]>();
            bool detectEncoding = true;

            using (TextReader reader = new StreamReader(lfpFile.FullName, encoding, detectEncoding))
            {
                string line = string.Empty;
                                
                while ((line = reader.ReadLine()) != null)
                {                
                    string[] record = ParseLine(line, null);
                    records.Add(record);
                }
            }

            return records;
        }

        public string[] ParseLine(string line, Delimiters delimiters)
        {
            return line.Split(new char[] { ';', ',' });
        }
    }
}
