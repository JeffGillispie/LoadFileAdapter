using System.IO;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// The settings used to parse data read from a DAT.
    /// </summary>
    public class ParseReaderDatSettings : ParseReaderSettings
    {
        private Delimiters delimiters;

        /// <summary>
        /// The delimiters used to parse data.
        /// </summary>
        public Delimiters Delimiters { get { return delimiters; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ParseReaderSettings"/>.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> used to read data
        /// to be parsed.</param>
        /// <param name="delimiters">The delimiters used to parse data.</param>
        public ParseReaderDatSettings(TextReader reader, Delimiters delimiters) : base(reader)
        {
            this.delimiters = delimiters;
        }
    }
}
