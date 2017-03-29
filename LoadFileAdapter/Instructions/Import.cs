using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace LoadFileAdapter.Instructions
{   
    public abstract class Import
    {
        /// <summary>
        /// The file to import. The field is ignored for 
        /// serialization and replaced by the FilePath
        /// property.
        /// </summary>                    
        [XmlIgnore]
        public FileInfo File;

        /// <summary>
        /// The encoding used to read the import file. The 
        /// field is ignored for serialization and replaced 
        /// by the CodePage property.
        /// </summary>
        [XmlIgnore] 
        public Encoding Encoding;

        /// <summary>
        /// Initializes a new instance of <see cref="Import"/>.
        /// </summary>
        /// <param name="file">The file to import.</param>
        /// <param name="encoding">The encoding used to read the file.</param>
        public Import(FileInfo file, Encoding encoding)
        {
            this.File = file;
            this.Encoding = encoding;
        }
        
        /// <summary>
        /// The path to the import file. It is also used to 
        /// manage the File field.
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
        /// The code page of the <see cref="Encoding"/> used to 
        /// import data. It also is used to manage the Encoding
        /// field.
        /// </summary>
        public int CodePage
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
