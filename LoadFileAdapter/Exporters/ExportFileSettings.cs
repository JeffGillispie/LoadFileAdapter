using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Settings used to export a document collection to a file.
    /// </summary>
    public abstract class ExportFileSettings
    {
        private DocumentCollection documents;
        private FileInfo file;
        private Encoding encoding;

        /// <summary>
        /// The document collection to export.
        /// </summary>
        public DocumentCollection Documents { get { return documents; } }

        /// <summary>
        /// The destination of an export.
        /// </summary>
        public FileInfo File { get { return file; } }

        /// <summary>
        /// The encoding used in the export file.
        /// </summary>
        public Encoding Encoding { get { return encoding; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ExportFileSettings"/>.
        /// </summary>
        /// <param name="documents">The document collection to export.</param>
        /// <param name="file">The destination of the export.</param>
        /// <param name="encoding">The encoding to use in the export.</param>
        public ExportFileSettings(DocumentCollection documents, FileInfo file, Encoding encoding)
        {
            this.documents = documents;
            this.file = file;
            this.encoding = encoding;
        }
    }
}
