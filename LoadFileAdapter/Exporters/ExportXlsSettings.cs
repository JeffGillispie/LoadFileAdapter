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
        private string[] exportFields;
        private ExportXlsLinkSettings[] links;

        /// <summary>
        /// Initializes a new instance of <see cref="ExportXlsSettings"/>.
        /// </summary>
        /// <param name="documents">The documents to export.</param>
        /// <param name="file">The destination file.</param>
        /// <param name="exportFields">The fields to export.</param>
        /// <param name="links">The export hyperlink settings.</param>
        public ExportXlsSettings(DocumentCollection documents, FileInfo file,
            string[] exportFields, ExportXlsLinkSettings[] links)
        {
            this.documents = documents;
            this.file = file;
            this.exportFields = exportFields;
            this.links = links;
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

        /// <summary>
        /// Gets the fields to include in the export.
        /// </summary>
        /// <returns>Returns the fields to export.</returns>
        public string[] GetExportFields()
        {
            return this.exportFields;
        }

        /// <summary>
        /// Gets the settings to create hyperlinks.
        /// </summary>
        /// <returns>Returns a collection of <see cref="ExportXlsLinkSettings"/>.</returns>
        public ExportXlsLinkSettings[] GetLinks()
        {
            return this.links;
        }
    }
}
