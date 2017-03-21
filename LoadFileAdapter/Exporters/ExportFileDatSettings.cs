using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    public class ExportFileDatSettings : ExportFileSettings
    {
        private Delimiters delimiters;
        private string[] exportFields;

        public Delimiters Delimiters { get { return delimiters; } }
        public string[] ExportFields { get { return exportFields; } }

        public ExportFileDatSettings(DocumentCollection documents, 
            FileInfo file, Encoding encoding, Delimiters delimiters, string[] exportFields) :
            base(documents, file, encoding)
        {
            this.delimiters = delimiters;
            this.exportFields = exportFields;
        }
    }
}
