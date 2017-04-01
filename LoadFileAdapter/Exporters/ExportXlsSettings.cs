using System.IO;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Export settings for an excel file.
    /// </summary>
    public class ExportXlsSettings : IExportSettings 
    {
        private DocumentCollection documents;
        private FileInfo file;

        /// <summary>
        /// Initializes a new instance of <see cref="ExportXlsSettings"/>.
        /// </summary>
        /// <param name="documents">The documents to export.</param>
        /// <param name="file">The destination file.</param>
        public ExportXlsSettings(DocumentCollection documents, FileInfo file)
        {
            this.documents = documents;
            this.file = file;
        }

        /// <summary>
        /// Gets the documents to export.
        /// </summary>
        /// <returns>Returns a <see cref="DocumentCollection"/>.</returns>
        public DocumentCollection GetDocuments()
        {
            return this.documents;
        }

        /// <summary>
        /// Gets the destination file.
        /// </summary>
        /// <returns>Returns the destination <see cref="FileInfo"/>.</returns>
        public FileInfo GetFile()
        {
            return this.file;
        }
    }
}
