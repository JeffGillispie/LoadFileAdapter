using System.IO;
using System.Text;
using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    public class OptExport : ImgExport
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OptExport"/>.
        /// </summary>
        public OptExport() : base()
        {
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OptExport"/>.
        /// </summary>
        /// <param name="file">The file to export.</param>
        /// <param name="encoding">The encoding used to write the export."/></param>
        /// <param name="volName">The volume name of the export.</param>
        public OptExport(FileInfo file, Encoding encoding, string volName) 
            : base(file, encoding, volName)
        {
            // do nothing here
        }

        public override IExporter BuildExporter()
        {
            return OptExporter.Builder
                .Start(File, Encoding)
                .SetVolumeName(VolumeName ?? File.Name)
                .Build();
        }
    }
}
