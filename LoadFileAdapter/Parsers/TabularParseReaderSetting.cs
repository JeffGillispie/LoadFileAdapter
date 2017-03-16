using System.IO;

namespace LoadFileAdapter.Parsers
{
    public class TabularParseReaderSetting : ParseReaderSetting
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public TabularParseReaderSetting(TextReader reader, Delimiters delimiters) : base(reader)
        {
            this.delimiters = delimiters;
        }
    }
}
