using System.IO;

namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// The settings used parse data read from a <see cref="TextReader"/>.
    /// </summary>
    public class ParseReaderSettings
    {
        private TextReader reader;

        /// <summary>
        /// The reader used to read data to be parsed.
        /// </summary>
        public TextReader Reader { get { return reader; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ParseReaderSettings"/>.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> used to read data
        /// to be parsed.</param>
        public ParseReaderSettings(TextReader reader)
        {
            this.reader = reader;
        }
    }
}
