using System.IO;

namespace LoadFileAdapter.Exporters
{
    public class ImageExportWriterSetting : ExportWriterSetting
    {
        private string volumeName;

        public string VolumeName { get { return volumeName; } }

        public ImageExportWriterSetting(TextWriter writer, DocumentCollection documents, string volumeName) : base(writer, documents)
        {
            this.volumeName = volumeName;
        }
    }
}
