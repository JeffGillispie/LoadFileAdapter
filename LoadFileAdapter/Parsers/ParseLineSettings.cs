
namespace LoadFileAdapter.Parsers
{
    /// <summary>
    /// The settings to parse a generic line.
    /// </summary>
    public class ParseLineSettings
    {
        private string line;

        /// <summary>
        /// The line to be parsed.
        /// </summary>
        public string Line { get { return line; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ParseLineSettings"/>.
        /// </summary>
        /// <param name="line">The line to be parsed.</param>
        public ParseLineSettings(string line)
        {
            this.line = line;
        }
    }
}
