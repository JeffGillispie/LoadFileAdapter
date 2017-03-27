using System.IO;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to write the text delimited metadata fields of a document collection.
    /// </summary>
    public class ExportWriterDatSettings : ExportWriterSettings
    {
        private Delimiters delimiters;
        private string[] exportFields;

        /// <summary>
        /// The delimiters used in the DAT file.
        /// </summary>
        public Delimiters Delimiters { get { return delimiters; } }

        /// <summary>
        /// The names of the metadata fields to export.
        /// </summary>
        public string[] ExportFields { get { return exportFields; } }

        /// <summary>
        /// Initializes a new instance of <see cref="ExportWriterDatSettings"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> used to write the DAT file.</param>
        /// <param name="documents">The document collection to export.</param>
        /// <param name="delimiters">The delimiters to use in the export.</param>
        /// <param name="exportFields">The metadata fields to export.</param>
        public ExportWriterDatSettings(TextWriter writer, 
            DocumentCollection documents, Delimiters delimiters, 
            string[] exportFields) : base(writer, documents)
        {
            this.delimiters = delimiters;
            this.exportFields = exportFields;
        }
    }
}
