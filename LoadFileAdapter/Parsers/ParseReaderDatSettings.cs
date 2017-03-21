using System.IO;

namespace LoadFileAdapter.Parsers
{
    public class ParseReaderDatSettings : ParseReaderSettings
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public ParseReaderDatSettings(TextReader reader, Delimiters delimiters) : base(reader)
        {
            this.delimiters = delimiters;
        }
    }
}
