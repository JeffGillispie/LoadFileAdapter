using System.IO;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to write data for an image load file.
    /// </summary>
    public class ExportWriterImageSettings : ExportWriterSettings
    {
        private string volumeName;

        /// <summary>
        /// The volume name to use in the export.
        /// </summary>
        public string VolumeName { get { return volumeName; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ExportWriterImageSettings"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> used to write image load file data.</param>
        /// <param name="documents">The document collection to export.</param>
        /// <param name="volumeName">The volume name to use in the export.</param>
        public ExportWriterImageSettings(
            TextWriter writer, DocumentCollection documents, string volumeName) : 
            base(writer, documents)
        {
            this.volumeName = volumeName;
        }
    }
}
