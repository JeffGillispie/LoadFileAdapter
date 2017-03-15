using System.IO;

namespace LoadFileAdapter.Parsers
{
    public class TextDelimitedParseReaderParameters : ParseReaderParameters
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public TextDelimitedParseReaderParameters(TextReader reader, Delimiters delimiters) : base(reader)
        {
            this.delimiters = delimiters;
        }
    }
}
