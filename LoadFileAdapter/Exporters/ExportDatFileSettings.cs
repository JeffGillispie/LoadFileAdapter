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

        public Delimiters GetDelimiters()
        {
            return this.delimiters;
        }

        public string[] GetExportFields()
        {
            return this.exportFields;
        }
    }
}
