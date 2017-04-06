using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains export instructions for a <see cref="DocumentCollection"/>.
    /// Used to serialize instructions and deserialize instructions from XML.
    /// </summary>
    public abstract class Export
    {
        /// <summary>
        /// The destination of the export.
        /// This field is ignored for serialization and replaced
        /// with the FilePath property.
        /// </summary>
        [XmlIgnore]
        public FileInfo File;

        /// <summary>
        /// The <see cref="Encoding"/> used to write the export.
        /// This field is ignored for serialization and replaced
        /// with the CodePage property.
        /// </summary>
        [XmlIgnore]
        public Encoding Encoding;

        /// <summary>
        /// Initializes a new instances of <see cref="Export"/>.
        /// </summary>
        public Export()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Export"/>.
        /// </summary>
        /// <param name="file">The destination of the export.</param>
        /// <param name="encoding">The <see cref="Encoding"/> used to write the export.</param>
        public Export(FileInfo file, Encoding encoding)
        {
            this.File = file;
            this.Encoding = encoding;
        }

        /// <summary>
        /// The destination for the export path used to manage the File field.
        /// </summary>
        public string FilePath
        {
            get
            {
                return this.File.FullName;
            }

            set
            {
                this.File = new FileInfo(value);
            }
        }

        /// <summary>
        /// The <see cref="Encoding"/> code page used to manage the Encoding field.
        /// </summary>
        public virtual int CodePage
        {
            get
            {
                return this.Encoding.CodePage;
            }

            set
            {
                this.Encoding = Encoding.GetEncoding(value);
            }
        }
    }
}
