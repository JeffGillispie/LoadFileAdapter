using System.IO;
using System.Text;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains the instructions for an image import. It is also used to serialize 
    /// instructions and deserialize instructions from XML.
    /// </summary>
    public class ImgImport : Import
    {
        /// <summary>
        /// The text representative settings for the image import.
        /// </summary>
        public TextFileInfo TextBuilder;

        /// <summary>
        /// Indicates if the load file path should be used to build
        /// an absolute path for representative files.
        /// </summary>
        public bool BuildAbsolutePath;

        /// <summary>
        /// Initializes a new instance of <see cref="ImgImport"/>.
        /// </summary>
        protected ImgImport() : base(null, null)
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ImgImport"/>.
        /// </summary>
        /// <param name="file">The file to import.</param>
        /// <param name="encoding">The encoding used to read the import.</param>
        /// <param name="txtSetting">The text representative settings for the import.</param>
        /// <param name="buildAbsolutePath">Setting for building representative absolute paths.</param>
        protected ImgImport(FileInfo file, Encoding encoding, 
            TextBuilder txtSetting, bool buildAbsolutePath) : 
            base(file, encoding)
        {
            this.TextBuilder = new TextFileInfo(txtSetting);
        }

        public override Importers.IImporter BuildImporter() { return null; }
    }
}
