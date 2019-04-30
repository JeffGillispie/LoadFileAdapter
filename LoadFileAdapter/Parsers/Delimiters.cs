using System;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// Characters used to parse text delimited data.
    /// </summary>
    public class Delimiters
    {
        /// <summary>
        /// Null delimiter value.
        /// </summary>
        public const char Null = '\0';
        private char fieldSeparator = ',';
        private char textQualifier = Null;
        private char newRecord = '\n';
        private char escapeCharacter = Null;
        private char flattenedNewLine = Null;

        /// <summary>
        /// Initializes a new instance of <see cref="Delimiters"/> with default
        /// values.
        /// </summary>
        private Delimiters()
        {
            this.fieldSeparator = ',';
            this.textQualifier = Null;
            this.newRecord = '\n';
            this.escapeCharacter = Null;
            this.flattenedNewLine = Null;
        }

        /// <summary>
        /// The charater used to separate fields.
        /// </summary>
        public char FieldSeparator { get { return fieldSeparator; } }        

        /// <summary>
        /// The character used to encapsulate fields.
        /// </summary>
        public char TextQualifier { get { return textQualifier; } }

        /// <summary>
        /// The character used to separate records.
        /// </summary>
        public char NewRecord { get { return newRecord; } }

        /// <summary>
        /// The character used to escape a text qualifier in qualified fields.
        /// </summary>
        public char EscapeCharacter { get { return escapeCharacter; } }

        /// <summary>
        /// A character used to flattened new record delimiters into the field
        /// value used in concordance.
        /// </summary>
        public char FlattenedNewLine { get { return flattenedNewLine; } }

        /// <summary>
        /// The standard comma quote delimiter set.
        /// </summary>
        public static Delimiters COMMA_QUOTE = Builder.Start()
            .SetTextQualifier('"')
            .SetEscapeCharacter('"')
            .Build();

        /// <summary>
        /// The standard comma delimited set.
        /// </summary>
        public static Delimiters COMMA_DELIMITED = Builder.Start()
            .Build();

        /// <summary>
        /// The standard tab delimiter set.
        /// </summary>
        public static Delimiters TAB_DELIMITED = Builder.Start()
            .SetFieldSeparator('\t')
            .Build();

        /// <summary>
        /// The standard pipe caret delimiter set.
        /// </summary>
        public static Delimiters PIPE_CARET = Builder.Start()
            .SetFieldSeparator('|')
            .SetTextQualifier('^')
            .SetEscapeCharacter('^')
            .Build();

        /// <summary>
        /// The standard concordance delimiters.
        /// </summary>
        public static Delimiters CONCORDANCE = Builder.Start()
            .SetFieldSeparator((char)20)
            .SetTextQualifier((char)254)
            .SetEscapeCharacter((char)254)
            .SetFlattenedNewLine((char)174)
            .Build();
                
        /// <summary>
        /// Validates the delimiters set do not conflict with each other.
        /// </summary>
        protected void checkForExceptions()
        {
            if (this.fieldSeparator == this.textQualifier)
                throw new Exception(
                    "The field separator and the text qualifier can not " + 
                    "have the same value.");
            else if (this.fieldSeparator == this.newRecord)
                throw new Exception(
                    "The field separator and the new record delimiter can " + 
                    "not have the same value.");
            else if (this.textQualifier == this.newRecord)
                throw new Exception(
                    "The text qualifier and the new record delimiter can " + 
                    "not have the same value.");
            else if (this.fieldSeparator == this.escapeCharacter)
                throw new Exception(
                    "The field separator and the escape character can not " + 
                    "have the same value.");
            else if (this.escapeCharacter == this.newRecord)
                throw new Exception(
                    "The escape character and the new record delimiter can " + 
                    "not have the same value.");
        }

        /// <summary>
        /// Builds an instance of <see cref="Delimiters"/>.
        /// </summary>
        public class Builder
        {
            private Delimiters instance;

            private Builder()
            {
                instance = new Delimiters();
            }

            /// <summary>
            /// Starts the builder.
            /// </summary>
            /// <returns>Returns a delimiters builder.</returns>
            public static Builder Start()
            {
                return new Builder();
            }

            /// <summary>
            /// Sets the <see cref="FieldSeparator"/> value.
            /// </summary>
            /// <param name="value">The field separator value.</param>
            /// <returns>Returns the updated builder.</returns>
            public Builder SetFieldSeparator(char value)
            {
                instance.fieldSeparator = value;
                return this;
            }

            /// <summary>
            /// Sets the <see cref="TextQualifier"/> value.
            /// </summary>
            /// <param name="value">The text qualifier value.</param>
            /// <returns>Returns the updated builder.</returns>
            public Builder SetTextQualifier(char value)
            {
                instance.textQualifier = value;
                return this;
            }

            /// <summary>
            /// Sets the <see cref="EscapeCharacter"/> value.
            /// </summary>
            /// <param name="value">The escape character value.</param>
            /// <returns>Returns the updated builder.</returns>
            public Builder SetEscapeCharacter(char value)
            {
                instance.escapeCharacter = value;
                return this;
            }

            /// <summary>
            /// Sets the <see cref="FlattenedNewLine"/> value.
            /// </summary>
            /// <param name="value">The flattened new line value.</param>
            /// <returns>Returns the updated builder.</returns>
            public Builder SetFlattenedNewLine(char value)
            {
                instance.escapeCharacter = value;
                return this;
            }

            /// <summary>
            /// Builds the <see cref="Delimiters"/> object.
            /// </summary>
            /// <returns>Returns a <see cref="Delimiters"/> instance.</returns>
            public Delimiters Build()
            {
                Delimiters instance = this.instance;
                this.instance = null;
                instance.checkForExceptions();
                return instance;
            }
        }
    }
}