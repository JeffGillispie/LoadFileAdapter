using System.IO;

namespace LoadFileAdapter.Exporters
{
    public abstract class ExportWriterSetting
    {
        private TextWriter writer;
        private DocumentCollection documents;

        public TextWriter Writer { get { return writer; } }
        public DocumentCollection Documents { get { return documents; } }

        public ExportWriterSetting(TextWriter writer, DocumentCollection documents)
        {
            this.writer = writer;
            this.documents = documents;
        }
    }
}
