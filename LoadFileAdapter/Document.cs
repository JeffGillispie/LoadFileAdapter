using System;
using System.Collections;
using System.Collections.Generic;

namespace LoadFileAdapter
{
    /// <summary>
    /// Represents a document from a load file. Holds the document key, parent, 
    /// a list of children, a collection of metadata key / value pairs, and a
    /// set of all linked files.
    /// </summary>
    public class Document : IComparable
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
        /// A set of child documents of this document.
        /// </summary>
        private HashSet<Document> children;
        /// <summary>
        /// A collection of metadata field names and field values belonging to this document.
        /// </summary>
        private Dictionary<string, string> metadata;
        /// <summary>
        /// The set of representatives files.
        /// </summary>
        private HashSet<Representative> representatives;

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="key">The document key or DOCID.</param>
        /// <param name="parent">The parent document.</param>
        /// <param name="children">A set of child documents.</param>
        /// <param name="metadata">A collection of metadata key / value pairs.</param>
        /// <param name="representatives">A set of representative files.</param>
        public Document(string key, 
            Document parent, 
            HashSet<Document> children, 
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
        public HashSet<Document> Children
        {
            get
            {
                return this.children;
            }            
        }

        /// <summary>
        /// A collection of metadata field names and field values.
        /// </summary>
        public Dictionary<string, string> Metadata
        {
            get
            {
                return this.metadata;
            }
        }

        /// <summary>
        /// The set of representative files.
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
        /// Sets the parent of the document and adds this document as a child to the parent.
        /// </summary>
        /// <param name="parent">The parent document.</param>
        public void SetParent(Document parent)
        {
            // this setter was added to support the creation of document
            // collections in the builders
            this.parent = parent;

            if (parent != null)
            {
                if (parent.children == null)
                {
                    parent.children = new HashSet<Document>();
                }

                parent.Children.Add(this);
            }
        }
        
        /// <summary>
        /// Sets the representative files of the document.
        /// </summary>
        /// <param name="representatives">A set of representative files.</param>
        public void SetLinkedFiles(HashSet<Representative> representatives)
        {
            // this setter was added to support transformations
            this.representatives = representatives;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Document doc = obj as Document;

            if (doc != null)
            {
                return this.key.CompareTo(doc.key);
            }
            else
                throw new ArgumentException("Object is not a Document");            
        }
    }
}
