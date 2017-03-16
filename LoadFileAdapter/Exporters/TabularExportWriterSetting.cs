using System.IO;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    public class TabularExportWriterSetting : ExportWriterSetting
    {
        private Delimiters delimiters;

        public Delimiters Delimiters { get { return delimiters; } }

        public TabularExportWriterSetting(TextWriter writer, DocumentCollection documents, Delimiters delimiters) : base(writer, documents)
        {
            this.delimiters = delimiters;
        }
    }
}
