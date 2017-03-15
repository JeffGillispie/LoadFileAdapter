using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    public class TextDelimitedParseFileParameters : ParseFileParameters
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public TextDelimitedParseFileParameters(FileInfo file, Encoding encoding, Delimiters delimiters) : base(file, encoding)
        {
            this.delimiters = delimiters;
        }
    }
}
