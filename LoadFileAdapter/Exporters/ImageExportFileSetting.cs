using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    public class ImageExportFileSetting : ExportFileSetting
    {
        private string volumeName;

        public string VolumeName { get { return volumeName; } }

        public ImageExportFileSetting(DocumentCollection documents, FileInfo file, Encoding encoding, string volumeName) : 
            base(documents, file, encoding)
        {
            this.volumeName = volumeName;
        }
    }
}
