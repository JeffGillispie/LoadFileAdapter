using System;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// Characters used to parse text delimited data.
    /// </summary>
    public class Delimiters
    {
        public const char Null = '\0';
        private readonly char fieldSeparator = ',';
        private readonly char textQualifier = Null;
        private char newRecord = '\n';
        private char escapeCharacter = Null;
        private char flattenedNewLine = Null;
        
        /// <summary>
        /// The charater used to separate fields.
        /// </summary>
        public char FieldSeparator { get { return this.fieldSeparator; } }        

        /// <summary>
        /// The character used to encapsulate fields.
        /// </summary>
        public char TextQualifier { get { return this.textQualifier; } }

        /// <summary>
        /// The character used to separate records.
        /// </summary>
        public char NewRecord { get { return this.newRecord; } }

        /// <summary>
        /// The character used to escape a text qualifier in qualified fields.
        /// </summary>
        public char EscapeCharacter { get { return this.escapeCharacter; } }

        /// <summary>
        /// A character used to flattened new record delimiters into the field
        /// value used in concordance.
        /// </summary>
        public char FlattenedNewLine { get { return this.flattenedNewLine; } }
        
        /// <summary>
        /// The standard comma quote delimiter set.
        /// </summary>
        public static Delimiters COMMA_QUOTE = of(',', '"', '\n', '"', Null);

        /// <summary>
        /// The standard comma delimited set.
        /// </summary>
        public static Delimiters COMMA_DELIMITED = of(',', Null, '\n', Null, Null);

        /// <summary>
        /// The standard tab delimiter set.
        /// </summary>
        public static Delimiters TAB_DELIMITED = of('\t', Null, '\n', Null, Null);

        /// <summary>
        /// The standard pipe caret delimiter set.
        /// </summary>
        public static Delimiters PIPE_CARET = of('|', '^', '\n', '^', Null);

        /// <summary>
        /// The standard concordance delimiters.
        /// </summary>
        public static Delimiters CONCORDANCE = of((char)20, (char)254, '\n', (char)254, (char)174);
        
        /// <summary>
        /// Creates a <see cref="Delimiters"/> instance.
        /// </summary>
        /// <param name="fieldSeparator">The character used to separate fields.</param>
        /// <param name="textQualifer">The character used to encapsulate fields.</param>
        /// <param name="newRecord">The character used to separate records.</param>
        /// <param name="escapeCharacter">The character used to escape a text qualifier in a qualified field.</param>
        /// <param name="flattenedNewLine">The character used to flattened line breaks in a field.</param>
        /// <returns></returns>
        public static Delimiters of(
            char fieldSeparator, char textQualifer, char newRecord, char escapeCharacter, char flattenedNewLine)
        {
            return new Delimiters(fieldSeparator, textQualifer, newRecord, escapeCharacter, flattenedNewLine);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Delimiters"/>.
        /// </summary>
        /// <param name="fieldSeparator">The character used to separate fields.</param>
        /// <param name="textQualifier">The character used to encapsulate fields.</param>
        /// <param name="newRecord">the character used to separate records.</param>
        /// <param name="escapeCharacter">The character used to escape a text qualifier in a qualified field.</param>
        /// <param name="flattenedNewLine">The character used to flattened line breaks in a field.</param>
        protected Delimiters(
            char fieldSeparator, char textQualifier, char newRecord, char escapeCharacter, char flattenedNewLine)
        {
            this.fieldSeparator = fieldSeparator;
            this.textQualifier = textQualifier;
            this.newRecord = newRecord;
            this.escapeCharacter = escapeCharacter;
            this.flattenedNewLine = flattenedNewLine;
            checkForExceptions();
        }

        /// <summary>
        /// Validates the delimiters set do not conflict with each other.
        /// </summary>
        protected void checkForExceptions()
        {
            if (this.fieldSeparator == this.textQualifier)
                throw new Exception(
                    "The field separator and the text qualifier can not have the same value.");
            else if (this.fieldSeparator == this.newRecord)
                throw new Exception(
                    "The field separator and the new record delimiter can not have the same value.");
            else if (this.textQualifier == this.newRecord)
                throw new Exception(
                    "The text qualifier and the new record delimiter can not have the same value.");
            else if (this.fieldSeparator == this.escapeCharacter)
                throw new Exception(
                    "The field separator and the escape character can not have the same value.");
            else if (this.escapeCharacter == this.newRecord)
                throw new Exception(
                    "The escape character and the new record delimiter can not have the same value.");
        }        
    }
}
