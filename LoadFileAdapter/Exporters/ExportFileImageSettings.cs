using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to export an image load file.
    /// </summary>
    public class ExportFileImageSettings : ExportFileSettings
    {
        private string volumeName;

        /// <summary>
        /// The export volume name.
        /// </summary>
        public string VolumeName { get { return volumeName; } }

        /// <summary>
        /// Initializes a new instance of a <see cref="ExportFileImageSettings"/>.
        /// </summary>
        /// <param name="documents">The document collection to export.</param>
        /// <param name="file">The export destination.</param>
        /// <param name="encoding">The encoding to use in the file.</param>
        /// <param name="volumeName">The volume name to use in the file.</param>
        public ExportFileImageSettings(DocumentCollection documents, FileInfo file, 
            Encoding encoding, string volumeName) : 
            base(documents, file, encoding)
        {
            this.volumeName = volumeName;
        }
    }
}
