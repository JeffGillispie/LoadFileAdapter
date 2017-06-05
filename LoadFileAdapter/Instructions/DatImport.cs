using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains the instructions used to import a DAT file.
    /// Instructions are used in serializing and deserializing settings to XML.
    /// </summary>
    public class DatImport : Import
    {
        /// <summary>
        /// The delimiters used to parse the import.
        /// </summary>
        public DelimitersBuilder Delimiters = new DelimitersBuilder(LoadFileAdapter.Parsers.Delimiters.CONCORDANCE);

        /// <summary>
        /// Indicates if the import has a header.
        /// </summary>
        public bool HasHeader = true;

        /// <summary>
        /// The name of the DocID field.
        /// </summary>
        public string KeyColumnName = null;

        /// <summary>
        /// The name of the ParentID field.
        /// </summary>
        public string ParentColumnName = null;

        /// <summary>
        /// The name of the AttachIDs field.
        /// </summary>
        public string ChildColumnName = null;

        /// <summary>
        /// The delimiter used in the AttachIDs field.
        /// </summary>
        public string ChildColumnDelimiter = null;

        /// <summary>
        /// Representative files in the DAT file.
        /// </summary>
        [XmlArray("LinkedFiles")]
        [XmlArrayItem(typeof(RepresentativeSettings), ElementName = "LinkedFile")]
        public RepresentativeSettings[] LinkedFiles = null;

        /// <summary>
        /// Initializes a new instance of <see cref="DatImport"/>.
        /// </summary>
        public DatImport() : base(null, null)
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DatImport"/>.
        /// </summary>
        /// <param name="file">The file to import.</param>
        /// <param name="encoding">The encoding used to read the import.</param>
        /// <param name="delimiters">The delimiters used to parse the import.</param>
        /// <param name="hasHeader">Indicates if the import contains a header.</param>
        /// <param name="keyColName">The name of the DocID field.</param>
        /// <param name="parentColName">The name of the ParentID field.</param>
        /// <param name="childColName">The name of the AttachIDs field.</param>
        /// <param name="childColDelim">The delimiter used in the AttachIDs field.</param>
        /// <param name="linkedFiles">Representatives in the DAT file.</param>
        public DatImport(FileInfo file, Encoding encoding, Delimiters delimiters, bool hasHeader, 
            string keyColName, string parentColName, string childColName, string childColDelim,
            RepresentativeBuilder[] linkedFiles) :
            base(file, encoding)
        {
            this.Delimiters = new DelimitersBuilder(delimiters);
            this.HasHeader = hasHeader;
            this.KeyColumnName = keyColName;
            this.ParentColumnName = parentColName;
            this.ChildColumnName = childColName;
            this.ChildColumnDelimiter = childColDelim;
            this.LinkedFiles = (linkedFiles != null) ? linkedFiles.Select(f => new RepresentativeSettings(f)).ToArray() : null;
        }               
    }
}
