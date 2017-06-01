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

        /// <summary>
        /// Gets the documents to export from the export settings.
        /// </summary>
        /// <returns>Returns a <see cref="DocumentCollection"/>.</returns>
        public DocumentCollection GetDocuments()
        {
            return this.documents;
        }

        /// <summary>
        /// Gets the writer used to write the export.
        /// </summary>
        /// <returns>Returns a <see cref="TextWriter"/>.</returns>
        public TextWriter GetWriter()
        {
            return this.writer;
        }

        /// <summary>
        /// Gets the delimiters used to write the export.
        /// </summary>
        /// <returns>Returns the export <see cref="Delimiters"/>.</returns>
        public Delimiters GetDelimiters()
        {
            return this.delimiters;
        }

        /// <summary>
        /// Gets the fields to export.
        /// </summary>
        /// <returns>Returns the export fields.</returns>
        public string[] GetExportFields()
        {
            return this.exportFields;
        }
    }
}
