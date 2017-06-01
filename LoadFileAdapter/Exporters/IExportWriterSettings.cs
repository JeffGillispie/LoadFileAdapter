using System.IO;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The settings used to write load file data.
    /// </summary>
    public interface IExportWriterSettings : IExportSettings
    {
        /// <summary>
        /// The <see cref="TextWriter"/> used to write the data.
        /// </summary>
        TextWriter GetWriter();                
    }
}
