using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// A parser used to parse text delimited data from a DAT file.
    /// </summary>
    public class DatParser : IParser
    {
        private Delimiters delimiters;

        /// <summary>
        /// Initializes a new instance of <see cref="DatParser"/>.
        /// </summary>
        /// <param name="delimiters">The delimiters used for parsing.</param>
        public DatParser(Delimiters delimiters)
        {
            this.delimiters = delimiters;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DatParser"/>.
        /// </summary>
        public DatParser()
        {
            this.delimiters = Delimiters.CONCORDANCE;
        }

        /// <summary>
        /// Parses a delimited text file.
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
        /// <param name="reader">The reader used to read the delimited text.</param>
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
        /// Parses a single delimited line of text.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>Returns the parsed fields.</returns>
        public string[] ParseLine(string line)
        {
            List<string> fieldValues = new List<string>();
            int startIndex = 0;

            while (startIndex < line.Length)
            {
                string fieldValue = parseField(line, ref startIndex, delimiters);
                fieldValues.Add(fieldValue);
            }

            if (line[line.Length - 1] == delimiters.FieldSeparator)
            {
                fieldValues.Add(String.Empty);
            }

            return fieldValues.ToArray();
        }

        /// <summary>
        /// Parses a single field from a string of delimited fields based on
        /// the position of startIndex.
        /// </summary>
        /// <param name="line">The line that is being parsed.</param>
        /// <param name="startIndex">The current position in the line.</param>
        /// <param name="delimiters">The delimiters used to parse the line.
        /// </param>
        /// <returns>Returns the next field in the line.</returns>
        protected string parseField(string line, ref int startIndex, Delimiters delimiters)
        {
            StringBuilder fieldValue = new StringBuilder();
            int currentIndex = startIndex;
            char currentChar = line[currentIndex];
            // if this is an empty field return an empty value
            if (currentChar == delimiters.FieldSeparator && delimiters.TextQualifier != Delimiters.Null)
            {
                currentIndex++;
            }
            // if this is not a qualified field then scan through to the next 
            // field separator
            else if (currentChar != delimiters.TextQualifier)
            {
                int endIndex = line.IndexOf(
                    delimiters.FieldSeparator, currentIndex);
                endIndex = (endIndex < 0) ? line.Length : endIndex;
                fieldValue.Append(
                    line.Substring(currentIndex, endIndex - currentIndex));
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

                    // current character is an escape character for a text qualifier, 
                    // append text qualifier as part of the field value
                    if (currentChar == delimiters.EscapeCharacter && nextChar == delimiters.TextQualifier)
                    {
                        fieldValue.Append(nextChar);
                        currentIndex++;
                        continue;
                    }
                    // if current character is text qualifer then next 
                    // character is the field separator of doesn't exist then 
                    // we're done with the field
                    else if (currentChar == delimiters.TextQualifier 
                        && (nextChar == null || nextChar == delimiters.FieldSeparator))
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
