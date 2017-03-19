using System.Collections.Generic;

namespace LoadFileAdapter
{
    /// <summary>
    /// Represents a document from a load file. Holds the document key, parent, 
    /// a list of children, a collection of metadata key / value pairs, and a
    /// set of all file representations of that document.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// The document key or DOCID value of this document.
        /// </summary>
        private string key;
        /// <summary>
        /// The parent document value of this document.
        /// </summary>
        private Document parent;
        /// <summary>
        /// A list of child documents of this document.
        /// </summary>
        private List<Document> children;
        /// <summary>
        /// A collection of metadata key / value pairs belonging to this document.
        /// </summary>
        private Dictionary<string, string> metadata;
        /// <summary>
        /// The set of file representations for this document.
        /// </summary>
        private HashSet<Representative> representatives;

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="key">The document key or DOCID.</param>
        /// <param name="parent">The parent document.</param>
        /// <param name="children">A list of child documents.</param>
        /// <param name="metadata">A collection of metadata key / value pairs.</param>
        /// <param name="representatives">A set of file representations of this document.</param>
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

        /// <summary>
        /// The document key or DOCID value of the document.
        /// </summary>
        public string Key
        {
            get
            {
                return this.key;
            }            
        }

        /// <summary>
        /// The parent document.
        /// </summary>
        public Document Parent
        {
            get
            {
                return this.parent;
            }            
        }

        /// <summary>
        /// A list of child documents.
        /// </summary>
        public List<Document> Children
        {
            get
            {
                return this.children;
            }            
        }

        /// <summary>
        /// A collection of metadata key / value pairs. Where the key is the metadat field name and the value is the field value.
        /// </summary>
        public Dictionary<string, string> Metadata
        {
            get
            {
                return this.metadata;
            }
        }

        /// <summary>
        /// The set of all file representations of the document.
        /// These can be of the type <see cref="Representative.Type"/>.
        /// </summary>
        public HashSet<Representative> Representatives
        {
            get
            {
                return this.representatives;
            }
        }

        /// <summary>
        /// Sets the parent of the document.
        /// </summary>
        /// <param name="parent">The parent document.</param>
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
        
        /// <summary>
        /// Sets the representatives of the document.
        /// </summary>
        /// <param name="reps">A set of all file representations of the document.</param>
        public void SetRepresentatives(HashSet<Representative> reps)
        {
            this.representatives = reps;
        }        
    }
}
