
namespace LoadFileAdapter.Transformers
{
    public class Transformer
    {
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
