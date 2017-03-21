using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    public class ExportFileImageSettings : ExportFileSettings
    {
        private string volumeName;

        public string VolumeName { get { return volumeName; } }

        public ExportFileImageSettings(DocumentCollection documents, FileInfo file, Encoding encoding, string volumeName) : 
            base(documents, file, encoding)
        {
            this.volumeName = volumeName;
        }
    }
}
