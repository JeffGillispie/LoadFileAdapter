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

        public DocumentCollection GetDocuments()
        {
            return this.documents;
        }

        public FileInfo GetFile()
        {
            return this.file;
        }

        public Encoding GetEncoding()
        {
            return this.encoding;
        }

        public string GetVolumeName()
        {
            return this.volumeName;
        }
    }
}
