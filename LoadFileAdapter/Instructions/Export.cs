using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NLog;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains export instructions for a <see cref="DocumentCollection"/>.
    /// Used to serialize instructions and deserialize instructions from XML.
    /// </summary>
    public abstract class Export
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The destination of the export.
        /// This field is ignored for serialization and replaced
        /// with the FilePath property.
        /// </summary>
        [XmlIgnore]
        private FileInfo file;

        /// <summary>
        /// The <see cref="Encoding"/> used to write the export.
        /// This field is ignored for serialization and replaced
        /// with the CodePage property.
        /// </summary>
        [XmlIgnore]
        public Encoding Encoding;

        /// <summary>
        /// Path to the folder that contains the file to export.
        /// </summary>
        public string TargetPath;

        /// <summary>
        ///  Optional folder name for the file to export.
        /// </summary>
        public string TargetFolderName;

        /// <summary>
        /// The extension without the period of the file to export.
        /// </summary>
        public string TargetExtension;

        /// <summary>
        /// The suffix for the file to export.
        /// </summary>
        public string TargetSuffix;

        /// <summary>
        /// The base name of the file to export.
        /// </summary>
        public string TargetName;

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
        /// The file to export. The field is ignored for 
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
                else if (!string.IsNullOrWhiteSpace(TargetPath)
                      && !string.IsNullOrWhiteSpace(TargetName)
                      && !string.IsNullOrWhiteSpace(TargetExtension))
                {
                    string folderPath = TargetPath.TrimEnd('\\') + "\\" + TargetFolderName?.TrimStart('\\') ?? "";
                    //logger.Trace("Folder Path: {0}", folderPath);
                    string fileName = TargetName + (TargetSuffix ?? "") + "." + TargetExtension;
                    //logger.Trace("File Name: {0}", fileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    //logger.Trace("File Path: {0}", filePath);
                    result = new FileInfo(filePath);
                }

                return result;
            }

            set
            {
                file = value;
            }
        }

        /// <summary>
        /// The destination for the export path used to manage the File field.
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
        /// The <see cref="Encoding"/> code page used to manage the Encoding field.
        /// </summary>
        public virtual int CodePage
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

        public abstract Exporters.IExporter BuildExporter();
    }
}
