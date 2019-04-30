using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LoadFileAdapter.Instructions
{   
    /// <summary>
    /// Instructions for importing a load file.
    /// </summary>
    public abstract class Import
    {
        /// <summary>
        /// The file to import. The field is ignored for 
        /// serialization and replaced by the FilePath
        /// property.
        /// </summary>                    
        [XmlIgnore]
        private FileInfo file;

        /// <summary>
        /// Path to the folder that contains the file to import.
        /// </summary>
        public string TargetPath;

        /// <summary>
        /// The extension without the period of the file to import.
        /// </summary>
        public string TargetExtension;

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
        /// The file to import. The field is ignored for 
        /// serialization and replaced by the FilePath
        /// </summary>
        [XmlIgnore]
        public FileInfo File
        {
            get
            {
                FileInfo result = null;

                if (file != null)
                {
                    result = file;
                }
                else if (!string.IsNullOrWhiteSpace(TargetPath) && !string.IsNullOrWhiteSpace(TargetExtension))
                {
                    DirectoryInfo dir = new DirectoryInfo(TargetPath);
                    string search = string.Format("*.{0}", TargetExtension);
                    FileInfo[] files = dir.GetFiles(search, SearchOption.TopDirectoryOnly);

                    if (files.Length == 1)
                    {
                        result = files[0];
                    }
                    else if (files.Length == 0)
                    {
                        string msg = string.Format(
                            "No files were found with the extension {0} in the folder {1}.", 
                            TargetExtension, TargetPath);
                        throw new Exception(msg);
                    }
                    else
                    {
                        string msg = string.Format(
                            "Multiple files were found with the extension {0} in the folder {1}. {2}",
                            TargetExtension, TargetPath, String.Join(", ", files.Select(x => x.FullName).ToList()));
                        throw new Exception(msg);
                    }
                }

                return result;
            }

            set
            {
                file = value;
            }
        }
        
        /// <summary>
        /// The path to the import file. It is also used to 
        /// manage the File field.
        /// </summary>
        public string FilePath
        {
            get
            {
                return this.File?.FullName;
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
                return this.Encoding?.CodePage ?? Encoding.Default.CodePage;
            }

            set
            {
                this.Encoding = Encoding.GetEncoding(value);
            }
        }
                
        public abstract Importers.IImporter BuildImporter();        
    }
}
