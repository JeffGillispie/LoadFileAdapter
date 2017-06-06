using System.IO;
using System.Text;
using System.Xml.Serialization;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains the instructions for exporting a DAT file and is used in serializing
    /// to XML and deserializing the instructions from XML.
    /// </summary>
    public class DatExport : Export
    {
        /// <summary>
        /// The delimiters to use in the DAT export.
        /// </summary>
        public Separators Delimiters = new Separators(Parsers.Delimiters.CONCORDANCE);

        /// <summary>
        /// The fields to export.
        /// </summary>
        [XmlArray("Fields")]
        [XmlArrayItem(typeof(string), ElementName = "Field")]
        public string[] ExportFields = null;

        /// <summary>
        /// Initializes a new instance of <see cref="DatExport"/>
        /// </summary>
        public DatExport() : base(null, null)
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DatExport"/>.
        /// </summary>
        /// <param name="file">The file to export.</param>
        /// <param name="encoding">The encoding of the export file.</param>
        /// <param name="delimiters">The delimiters to use in the export file.</param>
        /// <param name="exportFields">The fields to export.</param>
        public DatExport(FileInfo file, Encoding encoding, Delimiters delimiters, string[] exportFields) : 
            base(file, encoding)
        {
            this.Delimiters = new Separators(delimiters);
            this.ExportFields = exportFields; 
        }
        
        public override Exporters.IExporter BuildExporter()
        {
            return Exporters.DatExporter.Builder
                .Start(File, Encoding, ExportFields)
                .SetDelimiters(Delimiters.ToDelimiters())                
                .Build();
        }                
    }
}
