using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    public class TextDelimitedParser : Parser<TextDelimitedParseFileParameters, TextDelimitedParseReaderParameters, TextDelimitedParseLineParameters>
    {
        public List<string[]> Parse(TextDelimitedParseFileParameters parameters)
        {            
            bool detectEncoding = true;
            List<string[]> records = null;

            using (StreamReader reader = new StreamReader(parameters.File.FullName, parameters.Encoding, detectEncoding))
            {
                TextDelimitedParseReaderParameters readerParameters = new TextDelimitedParseReaderParameters(reader, parameters.Delimiters);
                records = Parse(readerParameters);
            }

            return records;
        }

        public List<string[]> Parse(TextDelimitedParseReaderParameters parameters)
        {
            List<string[]> records = new List<string[]>();

            string line = String.Empty;

            while ((line = parameters.Reader.ReadLine()) != null)
            {
                TextDelimitedParseLineParameters lineParameters = new TextDelimitedParseLineParameters(line, parameters.Delimiters);
                string[] record = ParseLine(lineParameters);
                records.Add(record);
            }

            return records;
        }

        public string[] ParseLine(TextDelimitedParseLineParameters parameters)
        {
            List<string> fieldValues = new List<string>();
            Int32 startIndex = 0;

            while (startIndex < parameters.Line.Length)
            {
                string fieldValue = parseField(parameters.Line, ref startIndex, parameters.Delimiters);
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
                int endIndex = line.IndexOf(delimiters.FieldSeparator, currentIndex);
                endIndex = (endIndex < 0) ? line.Length : endIndex;
                fieldValue.Append(line.Substring(currentIndex, endIndex - currentIndex));
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
