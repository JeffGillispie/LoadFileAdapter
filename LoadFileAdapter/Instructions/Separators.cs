using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains instructions on what delimiters to use to read an import or to use in
    /// writing an export. It is also used to serialize/deserialize data to/from XML.
    /// </summary>
    public class Separators
    {
        /// <summary>
        /// This character is used to separate fields.
        /// </summary>
        public char FieldSeparator = ',';

        /// <summary>
        /// This character is used to encapsulate field values.
        /// </summary>
        public char TextQualifier = Delimiters.Null;

        /// <summary>
        /// This character is used to separate records.
        /// </summary>
        public char NewRecord = '\n';

        /// <summary>
        /// This character can preceed a text qualifier in order to escape it and include it
        /// in the field value.
        /// </summary>
        public char EscapeCharacter = Delimiters.Null;

        /// <summary>
        /// This character is used with concordance to flatten records that contain a newline
        /// into a single line.
        /// </summary>
        public char FlattenedNewLine = Delimiters.Null;

        /// <summary>
        /// Initializes a new instance of <see cref="Separators"/>.
        /// </summary>
        public Separators()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Separators"/>.
        /// </summary>
        /// <param name="delims">The <see cref="Delimiters"/> used to initialize the object.</param>
        public Separators(Delimiters delims)
        {
            this.FieldSeparator = delims.FieldSeparator;
            this.TextQualifier = delims.TextQualifier;
            this.NewRecord = delims.NewRecord;
            this.EscapeCharacter = delims.EscapeCharacter;
            this.FlattenedNewLine = delims.FlattenedNewLine;
        }

        /// <summary>
        /// Gets <see cref="Delimiters"/> from a <see cref="Separators"/>.
        /// </summary>
        /// <returns>Returns a set of <see cref="Delimiters"/>.</returns>
        public Delimiters ToDelimiters()
        {
            return Delimiters.Builder.Start()
                .SetFieldSeparator(FieldSeparator)
                .SetTextQualifier(TextQualifier)
                .SetEscapeCharacter(EscapeCharacter)
                .SetFlattenedNewLine(FlattenedNewLine)
                .Build();            
        }
    }
}
