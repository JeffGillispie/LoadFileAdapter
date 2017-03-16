using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    public abstract class ExportFileSetting
    {
        private DocumentCollection documents;
        private FileInfo file;
        private Encoding encoding;

        public DocumentCollection Documents { get { return documents; } }
        public FileInfo File { get { return file; } }
        public Encoding Encoding { get { return encoding; } }

        public ExportFileSetting(DocumentCollection documents, FileInfo file, Encoding encoding)
        {
            this.documents = documents;
            this.file = file;
            this.encoding = encoding;
        }
    }
}
