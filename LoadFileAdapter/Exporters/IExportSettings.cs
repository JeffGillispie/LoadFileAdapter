
namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Basic export settings.
    /// </summary>
    public interface IExportSettings
    {

        /// <summary>
        /// Gets the document collection to export.
        /// </summary>
        DocumentCollection GetDocuments();
    }
}
