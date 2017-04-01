using System.IO;
using System.Text;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to export a document collection to a DAT file.
    /// </summary>
    public class ExportDatFileSettings : IExportFileSettings, IExportDatSettings
    {
        private DocumentCollection documents;
        private FileInfo file;
        private Encoding encoding;
        private Delimiters delimiters;
        private string[] exportFields;
                
        /// <summary>
        /// Initializes a new instance of <see cref="ExportDatFileSettings"/>.
        /// </summary>
        /// <param name="documents">The documents to export.</param>
        /// <param name="file">The destination of the exported DAT file.</param>
        /// <param name="encoding">The encoding used in the DAT file.</param>
        /// <param name="delimiters">The delimiters used in the DAT file.</param>
        /// <param name="exportFields">The fields to export to the DAT file.</param>
        public ExportDatFileSettings(DocumentCollection documents, 
            FileInfo file, Encoding encoding, 
            Delimiters delimiters, string[] exportFields)        
        {
            this.documents = documents;
            this.file = file;
            this.encoding = encoding;
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
        /// Gets the destination of the export.
        /// </summary>
        /// <returns>Returns the destination <see cref="FileInfo"/>.</returns>
        public FileInfo GetFile()
        {
            return this.file;
        }

        /// <summary>
        /// Gets the encoding used to write the export.
        /// </summary>
        /// <returns>Returns the <see cref="Encoding"/> of the export.</returns>
        public Encoding GetEncoding()
        {
            return this.encoding;
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
        /// <returns>Returns the fields to be exported.</returns>
        public string[] GetExportFields()
        {
            return this.exportFields;
        }
    }
}
