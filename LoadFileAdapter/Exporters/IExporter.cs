
namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The interface for an exporter to export data from a document collection.
    /// </summary>
    /// <typeparam name="T">export file setting</typeparam>
    /// <typeparam name="S">export writer settings</typeparam>
    public interface IExporter<T, S> 
        where T: ExportFileSettings
        where S: ExportWriterSettings
    {
        /// <summary>
        /// Uses a <see cref="FileInfo"/> object as the destination to export data 
        /// from a document collection.
        /// </summary>
        /// <param name="args">The <see cref="ExportFileSettings"/> used to export data.</param>
        void Export(T args);

        /// <summary>
        /// Uses a <see cref="TextWriter"/> to export data from a document collection.
        /// </summary>
        /// <param name="args">The <see cref="ExportWriterSettings"/> used to export data.</param>
        void Export(S args);
    }
}
