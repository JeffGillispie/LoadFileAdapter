using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Parsers
{
    public class Delimiters
    {
        public const char Null = '\0';
        private char fieldSeparator = ',';
        private char textQualifier = Null;
        private char newRecord = '\n';
        private char escapeCharacter = Null;
        private char flattenedNewLine = Null;
        
        public char FieldSeparator { get { return this.fieldSeparator; } }
        public char TextQualifier { get { return this.textQualifier; } }
        public char NewRecord { get { return this.newRecord; } }
        public char EscapeCharacter { get { return this.escapeCharacter; } }
        public char FlattenedNewLine { get { return this.flattenedNewLine; } }
        
        public static Delimiters COMMA_QUOTE = of(',', '"', '\n', '"', Null);
        public static Delimiters COMMA_DELIMITED = of(',', Null, '\n', Null, Null);
        public static Delimiters TAB_DELIMITED = of('\t', Null, '\n', Null, Null);
        public static Delimiters PIPE_CARET = of('|', '^', '\n', '^', Null);
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
