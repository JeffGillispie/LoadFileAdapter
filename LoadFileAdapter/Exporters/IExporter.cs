using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    public interface IExporter<T, S> 
        where T: ExportFileSetting
        where S: ExportWriterSetting
    {
        void Export(T args);
        void Export(S args);
    }
}
