using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    public class ParseFileSettings
    {
        private FileInfo file;
        private Encoding encoding;

        public FileInfo File { get { return file; } }
        public Encoding Encoding { get { return encoding; } }

        public ParseFileSettings(FileInfo file, Encoding encoding)
        {
            this.file = file;
            this.encoding = encoding;
        }
    }
}
