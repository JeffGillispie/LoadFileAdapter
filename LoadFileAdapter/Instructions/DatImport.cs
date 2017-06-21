using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LoadFileAdapter.Parsers;
using LoadFileAdapter.Importers;

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
        public Separators Delimiters = new Separators(Parsers.Delimiters.CONCORDANCE);

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
        [XmlArrayItem(typeof(RepresentativeInfo), ElementName = "LinkedFile")]
        public RepresentativeInfo[] LinkedFiles = null;

        /// <summary>
        /// Fields to prepend with the folder path of the input file.
        /// </summary>
        [XmlArray("FolderPrependFields")]
        [XmlArrayItem(typeof(string), ElementName = "FieldName")]
        public string[] FolderPrependFields = null;

        /// <summary>
        /// Representative types to prepend with the folder path of the input file.
        /// </summary>
        [XmlArray("FolderPrependLinks")]
        [XmlArrayItem(typeof(Representative.FileType), ElementName = "LinkType")]
        public Representative.FileType[] FolderPrependLinks = null;

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
        /// <param name="keyColName">The name of the DocID field.</param>        
        public DatImport(FileInfo file, Encoding encoding, Delimiters delimiters, string keyColName) : base(file, encoding)
        {
            this.Delimiters = new Separators(delimiters);
            this.KeyColumnName = keyColName;
        }
        
        public override IImporter BuildImporter()
        {
            DatImporter importer = new DatImporter(Delimiters.ToDelimiters());
            importer.Builder.HasHeader = HasHeader;
            importer.Builder.KeyColumnName = KeyColumnName;
            importer.Builder.ParentColumnName = ParentColumnName;
            importer.Builder.ChildColumnName = ChildColumnName;
            importer.Builder.ChildSeparator = ChildColumnDelimiter;
            importer.Builder.RepresentativeBuilders = (LinkedFiles != null)
                ? LinkedFiles.Select(f => f.GetBuilder()).ToList()
                : null;
            importer.FolderPrependFields = FolderPrependFields;
            importer.FolderPrependLinks = FolderPrependLinks;
            return importer;
        }               
    }
}
