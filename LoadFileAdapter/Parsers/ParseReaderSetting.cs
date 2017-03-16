using System.IO;

namespace LoadFileAdapter.Parsers
{
    public class ParseReaderSetting
    {
        private TextReader reader;

        public TextReader Reader { get { return reader; } }

        public ParseReaderSetting(TextReader reader)
        {
            this.reader = reader;
        }
    }
}
