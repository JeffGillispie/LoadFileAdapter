
namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// The settings used to parse a line from a DAT file.
    /// </summary>
    public class ParseLineDatSettings : ParseLineSettings
    {
        private Delimiters delimiters;

        /// <summary>
        /// The delimiters used to parse a line.
        /// </summary>
        public Delimiters Delimiters { get { return delimiters; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ParseLineDatSettings"/>.
        /// </summary>
        /// <param name="line">The line to be parsed.</param>
        /// <param name="delimiters">The delimiters used to parse the line.</param>
        public ParseLineDatSettings(string line, Delimiters delimiters) : base(line)
        {
            this.delimiters = delimiters;
        }
    }
}
