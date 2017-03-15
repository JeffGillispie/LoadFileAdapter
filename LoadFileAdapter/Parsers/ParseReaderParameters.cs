using System.IO;

namespace LoadFileAdapter.Parsers
{
    public class ParseReaderParameters
    {
        private TextReader reader;

        public TextReader Reader { get { return reader; } }

        public ParseReaderParameters(TextReader reader)
        {
            this.reader = reader;
        }
    }
}
