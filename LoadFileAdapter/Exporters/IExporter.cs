
namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Exports a <see cref="DocumentCollection"/>.
    /// </summary>
    public interface IExporter
    {
        /// <summary>
        /// Exports a <see cref="DocumentCollection"/>.
        /// </summary>
        /// <param name="docs">The documents to export.</param>
        void Export(DocumentCollection docs);                       
    }
}
