using System.IO;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to write load file data.
    /// </summary>
    public abstract class ExportWriterSettings
    {
        private TextWriter writer;
        private DocumentCollection documents;

        /// <summary>
        /// The <see cref="TextWriter"/> used to write the data.
        /// </summary>
        public TextWriter Writer { get { return writer; } }

        /// <summary>
        /// the document collection to export.
        /// </summary>
        public DocumentCollection Documents { get { return documents; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ExportWriterSettings"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> used to export data.</param>
        /// <param name="documents">The document collection to export.</param>
        public ExportWriterSettings(TextWriter writer, DocumentCollection documents)
        {
            this.writer = writer;
            this.documents = documents;
        }
    }
}
