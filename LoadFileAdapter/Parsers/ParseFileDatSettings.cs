using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    public class ParseFileDatSettings : ParseFileSettings
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public ParseFileDatSettings(FileInfo file, Encoding encoding, Delimiters delimiters) : base(file, encoding)
        {
            this.delimiters = delimiters;
        }
    }
}
