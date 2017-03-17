using System.Collections.Generic;

namespace LoadFileAdapter
{
    public class Document
    {
        private string key;
        private Document parent;
        private List<Document> children;
        private Dictionary<string, string> metadata;
        private HashSet<Representative> representatives;

        public Document(string key, 
            Document parent, 
            List<Document> children, 
            Dictionary<string, string> metadata, 
            HashSet<Representative> representatives)
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
            
            if (!parent.Children.Contains(this))
            {
                parent.Children.Add(this);
            }
        }
        
        public void SetRepresentatives(HashSet<Representative> reps)
        {
            this.representatives = reps;
        }
    }
}
