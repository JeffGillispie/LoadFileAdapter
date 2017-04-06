using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains the instructions for exporting an excel file and is used in serializing
    /// to XML and deserializing the instructions from XML.
    /// </summary>
    public class XlsExport : Export
    {
        [XmlIgnore]
        public override int CodePage
        {
            get
            {
                return base.Encoding.CodePage;
            }

            set
            {
                base.Encoding = Encoding.GetEncoding(value);
            }
        }

        /// <summary>
        /// The fields to export.
        /// </summary>
        [XmlArray("Fields")]
        [XmlArrayItem(typeof(string), ElementName ="Field")]
        public string[] ExportFields = null;

        /// <summary>
        /// The hyperlink settings for this export.
        /// </summary>
        public Hyperlink[] Hyperlinks = null; 

        /// <summary>
        /// Initializes a new instance of <see cref="XlsExport"/>.
        /// </summary>
        public XlsExport() : base()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="XlsExport"/>.
        /// </summary>
        /// <param name="file">The destination file.</param>
        /// <param name="exportFields">The fields to export.</param>
        /// <param name="links">Hyperlink settings of the export.</param>
        public XlsExport(FileInfo file, string[] exportFields, ExportXlsLinkSettings[] links) : 
            base(file, Encoding.Default)
        {
            this.ExportFields = exportFields;
            this.Hyperlinks = links.Select(l => new Hyperlink(l)).ToArray();
        }
    }
}
