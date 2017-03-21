using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    public interface IExporter<T, S> 
        where T: ExportFileSettings
        where S: ExportWriterSettings
    {
        void Export(T args);
        void Export(S args);
    }
}
