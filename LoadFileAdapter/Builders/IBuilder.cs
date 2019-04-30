using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Builds load file data into documents.
    /// </summary>
    public interface IBuilder
    {
        /// <summary>
        /// Builds a list of documents.
        /// </summary>
        /// <param name="records">The records to build into documents.</param>
        /// <returns>Returns a list of <see cref="Document"/>.</returns>
        List<Document> Build(IEnumerable<string[]> records);

        /// <summary>
        /// Builds a single document.
        /// </summary>
        /// <param name="docRecords">Document Records</param>
        /// <returns>Returns a <see cref="Document"/>.</returns>
        Document BuildDocument(IEnumerable<string[]> docRecords);
    }
}