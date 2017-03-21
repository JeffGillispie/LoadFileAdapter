using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    public class ExportFileDatSettings : ExportFileSettings
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public ExportFileDatSettings(DocumentCollection documents, FileInfo file, Encoding encoding, Delimiters delimiters) :
            base(documents, file, encoding)
        {
            this.delimiters = delimiters;
        }
    }
}
