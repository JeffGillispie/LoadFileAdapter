using System.IO;
using System.Text;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Contains the export instructions to export an image load file such as an OPT or LFP.
    /// It is also used to serialize instructions and deserialize instructions from XML.
    /// </summary>
    public class ImgExport : Export
    {
        /// <summary>
        /// The volume name of the export.
        /// </summary>
        public string VolumeName = null;

        /// <summary>
        /// Initializes a new instance of <see cref="ImgExport"/>.
        /// </summary>
        protected ImgExport() : base()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ImgExport"/>.
        /// </summary>
        /// <param name="file">The file to export.</param>
        /// <param name="encoding">The encoding used to write the export."/></param>
        /// <param name="volName">The volume name of the export.</param>
        protected ImgExport(FileInfo file, Encoding encoding, string volName) : base(file, encoding)
        {
            this.VolumeName = volName;
        }

        public override Exporters.IExporter BuildExporter() { return null; }
    }
}
