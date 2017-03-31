using System.IO;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to write the text delimited metadata fields of a document collection.
    /// </summary>
    public class ExportDatWriterSettings : IExportWriterSettings, IExportDatSettings
    {
        private DocumentCollection documents;
        private TextWriter writer;
        private Delimiters delimiters;
        private string[] exportFields;
                
        /// <summary>
        /// Initializes a new instance of <see cref="ExportDatWriterSettings"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> used to write the DAT file.</param>
        /// <param name="documents">The document collection to export.</param>
        /// <param name="delimiters">The delimiters to use in the export.</param>
        /// <param name="exportFields">The metadata fields to export.</param>
        public ExportDatWriterSettings(TextWriter writer, DocumentCollection documents, 
            Delimiters delimiters, string[] exportFields)
        {
            this.documents = documents;
            this.writer = writer;
            this.delimiters = delimiters;
            this.exportFields = exportFields;
        }

        public DocumentCollection GetDocuments()
        {
            return this.documents;
        }

        public TextWriter GetWriter()
        {
            return this.writer;
        }

        public Delimiters GetDelimiters()
        {
            return this.delimiters;
        }

        public string[] GetExportFields()
        {
            return this.exportFields;
        }
    }
}
