
namespace LoadFileAdapter.Transformers
{
    /// <summary>
    /// Executes a series of edits on a series of documents.
    /// </summary>
    public class Transformer
    {
        /// <summary>
        /// Performs a collection of edits on a <see cref="DocumentCollection"/>.
        /// </summary>
        /// <param name="docs">The collection of <see cref="Document"/>s to edit.</param>
        /// <param name="edits">The edits to perform on teh <see cref="DocumentCollection"/>.</param>
        public void Transform(DocumentCollection docs, Transformation[] edits)
        {
            foreach (Document doc in docs)
            {
                foreach (Transformation edit in edits)
                {
                    edit.Transform(doc);
                }
            }
        }        
    }
}
