using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    public class TabularParseFileSetting : ParseFileSetting
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public TabularParseFileSetting(FileInfo file, Encoding encoding, Delimiters delimiters) : base(file, encoding)
        {
            this.delimiters = delimiters;
        }
    }
}
