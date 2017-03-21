using System.IO;

namespace LoadFileAdapter.Exporters
{
    public class ExportWriterImageSettings : ExportWriterSettings
    {
        private string volumeName;

        public string VolumeName { get { return volumeName; } }

        public ExportWriterImageSettings(TextWriter writer, DocumentCollection documents, string volumeName) : base(writer, documents)
        {
            this.volumeName = volumeName;
        }
    }
}
