using System.IO;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    public class ExportWriterDatSettings : ExportWriterSettings
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public ExportWriterDatSettings(TextWriter writer, DocumentCollection documents, Delimiters delimiters) : base(writer, documents)
        {
            this.delimiters = delimiters;
        }
    }
}
