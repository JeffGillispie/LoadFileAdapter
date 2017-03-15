using System.IO;
using System.Text;

namespace LoadFileAdapter.Parsers
{
    public class ParseFileParameters
    {
        private FileInfo file;
        private Encoding encoding;

        public FileInfo File { get { return file; } }
        public Encoding Encoding { get { return encoding; } }

        public ParseFileParameters(FileInfo file, Encoding encoding)
        {
            this.file = file;
            this.encoding = encoding;
        }
    }
}
