using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Parsers
{
    public class LfpParser
    {
        public List<string[]> Parse(FileInfo lfpFile, Encoding encoding)
        {
            List<string[]> records = new List<string[]>();
            bool detectEncoding = true;

            using (StreamReader reader = new StreamReader(lfpFile.FullName, encoding, detectEncoding))
            {
                while (reader.EndOfStream == false)
                {
                    string line = reader.ReadLine();
                    string[] record = line.Split(new char[] { ';', ',' });
                    records.Add(record);
                }
            }

            return records;
        }
    }
}
