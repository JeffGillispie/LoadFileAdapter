using System.IO;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Settings used to export a document collection to a file.
    /// </summary>
    public interface IExportFileSettings : IExportSettings
    {
        /// <summary>
        /// The destination of an export.
        /// </summary>
        FileInfo GetFile();

        /// <summary>
        /// The encoding used in the export file.
        /// </summary>
        Encoding GetEncoding();        
    }
}
