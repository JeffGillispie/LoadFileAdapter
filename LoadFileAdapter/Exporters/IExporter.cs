
namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The interface for an exporter to export data from a <see cref="DocumentCollection"/>.
    /// </summary>
    /// <typeparam name="T">Export Settings</typeparam>
    public interface IExporter<T> where T: IExportSettings
    {
        /// <summary>
        /// Exports a <see cref="DocumentCollection"/>.
        /// </summary>
        /// <param name="args">Export settings.</param>
        void Export(T args);                       
    }
}
