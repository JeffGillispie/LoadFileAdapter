using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// The interface for a builder that builds a document collection from a load file.
    /// </summary>
    /// <typeparam name="T">Document collection build settings.</typeparam>
    /// <typeparam name="S">Document build settings.</typeparam>
    public interface IBuilder<T, S>
        where T: BuildDocCollectionSettings
        where S: BuildDocSettings
    {
        /// <summary>
        /// Builds a list of documents.
        /// </summary>
        /// <param name="args">Document collection build settings.</param>
        /// <returns></returns>
        List<Document> BuildDocuments(T args);

        /// <summary>
        /// Builds a single document.
        /// </summary>
        /// <param name="args">Document build settings.</param>
        /// <returns></returns>
        Document BuildDocument(S args);
    }
}
