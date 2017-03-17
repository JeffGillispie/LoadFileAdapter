
namespace LoadFileAdapter.Transformers
{
    public class Transformer
    {
        public void Transform(DocumentCollection docs, Edit[] edits)
        {
            foreach (Document doc in docs)
            {
                foreach (Edit edit in edits)
                {
                    edit.Transform(doc);
                }
            }
        }        
    }
}
