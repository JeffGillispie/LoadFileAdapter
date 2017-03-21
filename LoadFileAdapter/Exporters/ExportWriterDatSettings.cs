using System.IO;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    public class ExportWriterDatSettings : ExportWriterSettings
    {
        private Delimiters delimiters;
        private string[] exportFields;

        public Delimiters Delimiters { get { return delimiters; } }
        public string[] ExportFields { get { return exportFields; } }

        public ExportWriterDatSettings(TextWriter writer, 
            DocumentCollection documents, Delimiters delimiters, string[] exportFields) : 
            base(writer, documents)
        {
            this.delimiters = delimiters;
            this.exportFields = exportFields;
        }
    }
}
