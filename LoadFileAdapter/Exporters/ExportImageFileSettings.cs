using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to export an image load file.
    /// </summary>
    public class ExportImageFileSettings : IExportFileSettings, IExportImageSettings
    {
        private DocumentCollection documents;
        private FileInfo file;
        private Encoding encoding;
        private string volumeName;

        /// <summary>
        /// Initializes a new instance of a <see cref="ExportImageFileSettings"/>.
        /// </summary>
        /// <param name="documents">The document collection to export.</param>
        /// <param name="file">The export destination.</param>
        /// <param name="encoding">The encoding to use in the file.</param>
        /// <param name="volumeName">The volume name to use in the file.</param>
        public ExportImageFileSettings(DocumentCollection documents, FileInfo file, 
            Encoding encoding, string volumeName)
        {
            this.documents = documents;
            this.file = file;
            this.encoding = encoding;
            this.volumeName = volumeName;
        }

        /// <summary>
        /// Gets the documents to be exported.
        /// </summary>
        /// <returns>Returns a <see cref="DocumentCollection"/>.</returns>
        public DocumentCollection GetDocuments()
        {
            return this.documents;
        }

        /// <summary>
        /// Gets the destination file.
        /// </summary>
        /// <returns>Returns the destination <see cref="FileInfo"/> object.</returns>
        public FileInfo GetFile()
        {            
            return this.file;
        }

        /// <summary>
        /// Gets the encoding used to write the export.
        /// </summary>
        /// <returns>Returns the export's <see cref="Encoding"/>.</returns>
        public Encoding GetEncoding()
        {
            return this.encoding;
        }

        /// <summary>
        /// Gets the export volume name.
        /// </summary>
        /// <returns>Returns the name of the export volume.</returns>
        public string GetVolumeName()
        {
            return this.volumeName;
        }
    }
}
