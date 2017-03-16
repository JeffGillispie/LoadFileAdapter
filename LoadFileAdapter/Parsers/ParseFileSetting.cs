using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    public class ParseFileSetting
    {
        private FileInfo file;
        private Encoding encoding;

        public FileInfo File { get { return file; } }
        public Encoding Encoding { get { return encoding; } }

        public ParseFileSetting(FileInfo file, Encoding encoding)
        {
            this.file = file;
            this.encoding = encoding;
        }
    }
}
