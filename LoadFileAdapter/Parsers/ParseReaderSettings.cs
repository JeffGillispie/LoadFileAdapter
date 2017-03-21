using System.IO;

namespace LoadFileAdapter.Parsers
{
    public class ParseReaderSettings
    {
        private TextReader reader;

        public TextReader Reader { get { return reader; } }

        public ParseReaderSettings(TextReader reader)
        {
            this.reader = reader;
        }
    }
}
