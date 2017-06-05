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
        public TextFileSettingsBuilder TextSetting;

        /// <summary>
        /// Indicates if the load file path should be used to build
        /// an absolute path for representative files.
        /// </summary>
        public bool BuildAbsolutePath;

        /// <summary>
        /// Initializes a new instance of <see cref="ImgImport"/>.
        /// </summary>
        public ImgImport() : base(null, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="ImgImport"/>.
        /// </summary>
        /// <param name="file">The file to import.</param>
        /// <param name="encoding">The encoding used to read the import.</param>
        /// <param name="txtSetting">The text representative settings for the import.</param>
        /// <param name="buildAbsolutePath">Setting for building representative absolute paths.</param>
        public ImgImport(FileInfo file, Encoding encoding, 
            TextBuilder txtSetting, bool buildAbsolutePath) : 
            base(file, encoding)
        {
            this.TextSetting = new TextFileSettingsBuilder(txtSetting);
            this.BuildAbsolutePath = buildAbsolutePath;
        }                
    }
}
