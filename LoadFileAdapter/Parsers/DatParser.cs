using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// A parser used to parse text delimited data from a DAT file.
    /// </summary>
    public class DatParser : IParser<ParseFileDatSettings, ParseReaderDatSettings, ParseLineDatSettings>
    {
        /// <summary>
        /// Parses data from a delimited text file into a list of string
        /// arrays based on the supplied <see cref="Delimiters"/>.
        /// </summary>
        /// <param name="args">Contains the path to the file to parse
        /// and the encoding and delimiters used to read it.</param>
        /// <returns>Returns a list of fields parsed</returns>
        public virtual List<string[]> Parse(ParseFileDatSettings args)
        {
            bool detectEncoding = true;
            List<string[]> records = null;

            using (StreamReader reader = new StreamReader(args.File.FullName, args.Encoding, detectEncoding))
            {
                ParseReaderDatSettings readerArgs = new ParseReaderDatSettings(reader, args.Delimiters);
                records = Parse(readerArgs);
            }

            return records;
        }

        /// <summary>
        /// Parses data read from a <see cref="TextReader"/> into a list of 
        /// string arrays based on the supplied <see cref="Delimiters"/>.
        /// </summary>
        /// <param name="args">Contains the <see cref="TextReader"/> used to 
        /// read the input data and the <see cref="Delimiters"/> used to parse
        /// it.</param>
        /// <returns>Returns a list of fields parsed.</returns>
        public virtual List<string[]> Parse(ParseReaderDatSettings args)
        {
            List<string[]> records = new List<string[]>();

            string line = String.Empty;

            while ((line = args.Reader.ReadLine()) != null)
            {
                ParseLineDatSettings lineParameters = new ParseLineDatSettings(
                    line, args.Delimiters);
                string[] record = ParseLine(lineParameters);
                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Parse a string into an array of fields.
        /// </summary>
        /// <param name="args">Contains both the line to be parsed and the 
        /// <see cref="Delimiters"/> used to parse the line.</param>
        /// <returns>Returns an array of parsed fields.</returns>
        public string[] ParseLine(ParseLineDatSettings args)
        {
            List<string> fieldValues = new List<string>();
            int startIndex = 0;

            while (startIndex < args.Line.Length)
            {
                string fieldValue = parseField(
                    args.Line, ref startIndex, args.Delimiters);
                fieldValues.Add(fieldValue);
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
        protected string parseField(
            string line, ref int startIndex, Delimiters delimiters)
        {
            StringBuilder fieldValue = new StringBuilder();
            int currentIndex = startIndex;
            char currentChar = line[currentIndex];
            // if this is an empty field return an empty value
            if (currentChar == delimiters.FieldSeparator)
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
