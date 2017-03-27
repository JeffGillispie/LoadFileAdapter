using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to export a document collection to a DAT file.
    /// </summary>
    public class ExportFileDatSettings : ExportFileSettings
    {
        private Delimiters delimiters;
        private string[] exportFields;

        /// <summary>
        /// The delimiters used in a DAT file export.
        /// </summary>
        public Delimiters Delimiters { get { return delimiters; } }

        /// <summary>
        /// The fields exported to a DAT file.
        /// </summary>
        public string[] ExportFields { get { return exportFields; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ExportFileDatSettings"/>.
        /// </summary>
        /// <param name="documents">The documents to export.</param>
        /// <param name="file">The destination of the exported DAT file.</param>
        /// <param name="encoding">The encoding used in the DAT file.</param>
        /// <param name="delimiters">The delimiters used in the DAT file.</param>
        /// <param name="exportFields">The fields to export to the DAT file.</param>
        public ExportFileDatSettings(DocumentCollection documents, 
            FileInfo file, Encoding encoding, Delimiters delimiters, 
            string[] exportFields) :
            base(documents, file, encoding)
        {
            this.delimiters = delimiters;
            this.exportFields = exportFields;
        }
    }
}
