using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Parsers
{
    public class Delimiters
    {
        private char fieldSeparator;
        private char textQualifier;
        private char newRecord;
        private char escapeCharacter;
        private char flattenedNewLine;

        public char FieldSeparator { get { return this.FieldSeparator; } }
        public char TextQualifier { get { return this.TextQualifier; } }
        public char NewRecord {  get { return this.NewRecord; } }
        public char EscapeCharacter { get { return this.EscapeCharacter; } }
        public char FlattenedNewLine { get { return this.FlattenedNewLine; } }

        public static Delimiters COMMA_QUOTE = of(',', '"', '\n', '"', '\0');
        public static Delimiters COMMA_DELIMITED = of(',', '\0', '\n', '\0', '\0');
        public static Delimiters TAB_DELIMITED = of('\t', '\0', '\n', '\0', '\0');
        public static Delimiters PIPE_CARET = of('|', '^', '\n', '^', '\0');
        public static Delimiters CONCORDANCE = of((char)20, (char)254, '\n', (char)254, (char)174);

        public static Delimiters of(char fieldSeparator, char textQualifer, char newRecord, char escapeCharacter, char flattenedNewLine)
        {
            return new Delimiters(fieldSeparator, textQualifer, newRecord, escapeCharacter, flattenedNewLine);
        }

        private Delimiters(char fieldSeparator, char textQualifier, char newRecord, char escapeCharacter, char flattenedNewLine)
        {
            this.fieldSeparator = fieldSeparator;
            this.textQualifier = textQualifier;
            this.newRecord = newRecord;
            this.escapeCharacter = escapeCharacter;
            this.flattenedNewLine = flattenedNewLine;
            checkForExceptions();
        }

        private void checkForExceptions()
        {
            if (this.fieldSeparator == this.textQualifier)
                throw new Exception("The field separator and the text qualifier can not have the same value.");
            else if (this.fieldSeparator == this.newRecord)
                throw new Exception("The field separator and the new record delimiter can not have the same value.");
            else if (this.textQualifier == this.newRecord)
                throw new Exception("The text qualifier and the new record delimiter can not have the same value.");
            else if (this.fieldSeparator == this.escapeCharacter)
                throw new Exception("The field separator and the escape character can not have the same value.");
            else if (this.escapeCharacter == this.newRecord)
                throw new Exception("The escape character and the new record delimiter can not have the same value.");
        }

    }
}
