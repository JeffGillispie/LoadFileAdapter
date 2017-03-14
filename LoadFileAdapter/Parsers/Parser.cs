using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Parsers
{
    public class Parser
    {
        public List<string[]> Parse(FileInfo file, Delimiters delimiters, Encoding encoding)
        {
            List<string[]> records = new List<string[]>();
            bool detectEncoding = true;

            using (StreamReader reader = new StreamReader(file.FullName, encoding, detectEncoding))
            {
                while (reader.EndOfStream == false)
                {
                    string line = reader.ReadLine();
                    string[] fieldValues = ParseLine(line, delimiters);
                    records.Add(fieldValues);
                }
            }

            return records;
        }

        public string[] ParseLine(string line, Delimiters delimiters)
        {
            List<string> fieldValues = new List<string>();
            Int32 startIndex = 0;

            while (startIndex < line.Length)
            {
                string fieldValue = parseField(line, ref startIndex, delimiters);
                fieldValues.Add(fieldValue);
            }

            return fieldValues.ToArray();
        }

        protected string parseField(string line, ref Int32 startIndex, Delimiters delimiters)
        {
            StringBuilder fieldValue = new StringBuilder();
            int currentIndex = startIndex;
            char currentChar = line[currentIndex];
            // if this is an empty field return an empty value
            if (currentChar == delimiters.FieldSeparator)
            {
                currentIndex++;
            }
            // if this is not a qualified field then scan through to the next field separator
            else if (currentChar != delimiters.TextQualifier)
            {
                int endIndex = line.IndexOf(delimiters.TextQualifier, currentIndex);
                endIndex = (endIndex < 0) ? line.Length : endIndex;
                fieldValue.Append(line.Substring(currentIndex, endIndex));
                currentIndex = endIndex;
            }
            // otherwise this field must be text qualified 
            else
            {
                for (currentIndex = currentIndex + 1; currentIndex < line.Length; currentIndex++)
                {
                    currentChar = line[currentIndex];
                    char? nextChar = null; 

                    if (currentIndex + 1 < line.Length)
                        nextChar = line[currentIndex + 1];

                    // current character is an escape character for a text qualifier, append text qualifier as part of the field value
                    if (currentChar == delimiters.EscapeCharacter && nextChar == delimiters.TextQualifier)
                    {
                        fieldValue.Append(nextChar);
                        currentIndex++;
                        continue;
                    }
                    // if current character is text qualifer then next character is the field separator of doesn't exist then we're done with the field
                    else if (currentChar == delimiters.TextQualifier && (nextChar == null || nextChar == delimiters.FieldSeparator))
                    {
                        currentIndex++;
                        break;
                    }
                    // append to field value
                    else
                    {
                        fieldValue.Append(currentChar);
                    }
                }
            }
            // update start position to the character after the last one examined
            startIndex = currentIndex + 1;
            return fieldValue.ToString();
        }
    }
}
