using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter
{
    public class Document : ICloneable
    {
        private string key;
        private Document parent;
        private List<Document> children;
        private Dictionary<string, string> metadata;
        private HashSet<Representative> representatives;

        public Document(string key, Document parent, List<Document> children, Dictionary<string, string> metadata, HashSet<Representative> representatives)
        {
            this.key = key;
            this.parent = parent;
            this.children = children;
            this.metadata = metadata;
            this.representatives = representatives;
        }

        public string Key
        {
            get
            {
                return this.key;
            }            
        }

        public Document Parent
        {
            get
            {
                return this.parent;
            }            
        }

        public List<Document> Children
        {
            get
            {
                return this.children;
            }            
        }

        public Dictionary<string, string> Metadata
        {
            get
            {
                return this.metadata;
            }
        }

        public HashSet<Representative> Representatives
        {
            get
            {
                return this.representatives;
            }
        }

        public void SetParent(Document parent)
        {
            this.parent = parent;

            if (parent.children == null)
            {
                parent.children = new List<Document>();
            }
            // now add this document as a child to the parent
            if (!parent.Children.Contains(this))
                parent.Children.Add(this);
        }
        
        public object Clone()
        {
            return new Document(
                (string)this.key.Clone(), 
                (Document)this.parent.Clone(), 
                new List<Document>(this.children), 
                new Dictionary<string, string>(this.metadata), 
                new HashSet<Representative>(this.representatives)
                );
        }
        
        public void Transform(Transformers.MetaDataEdit edit)
        {
            Document doc = edit.Transform(this);
            this.key = doc.Key;
            this.parent = doc.Parent;
            this.children = doc.Children;
            this.metadata = doc.Metadata;
            this.representatives = doc.Representatives;
        }
    }
}
