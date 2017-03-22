using System.Collections;
using System.Collections.Generic;

namespace LoadFileAdapter
{
    /// <summary>
    /// Represents a collection of <see cref="Document"/>.
    /// </summary>
    public class DocumentCollection : IEnumerable<Document>
    {
        private List<Document> docs = new List<Document>();        
        private int imageCount = -1;
        private int textCount = -1;
        private int nativeCount = -1;
        private int parentCount = -1;
        private int childCount = -1;
        private int standAloneCount = -1;
        private bool propertiesAreUncounted = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCollection"/> class.
        /// </summary>
        public DocumentCollection()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCollection"/> class.
        /// </summary>
        /// <param name="documents">The documents used to populate the collection.</param>
        public DocumentCollection(IEnumerable<Document> documents)
        {
            AddRange(documents);
        }

        /// <summary>
        /// Adds a <see cref="Document"/> to the collection.
        /// </summary>
        /// <param name="doc">The document to add to the collection.</param>
        public void Add(Document doc)
        {
            this.docs.Add(doc);
            propertyReset();
        }

        /// <summary>
        /// Adds a series of documents to the collection.
        /// </summary>
        /// <param name="documents">The documents to add to the collection.</param>
        public void AddRange(IEnumerable<Document> documents)
        {
            this.docs.AddRange(documents);
            propertyReset();            
        }

        /// <summary>
        /// The count of documents in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.docs.Count;
            }
        }

        /// <summary>
        /// The count of linked image files.
        /// </summary>
        public int ImageCount
        {
            get
            {
                if (propertiesAreUncounted)
                {
                    countPropertyValues();
                }

                return this.imageCount;
            }
        }

        /// <summary>
        /// The count of linked text files.
        /// </summary>
        public int TextCount
        {
            get
            {
                if (propertiesAreUncounted)
                {
                    countPropertyValues();
                }

                return this.textCount;
            }
        }

        /// <summary>
        /// The count of linked native files.
        /// </summary>
        public int NativeCount
        {
            get
            {
                if (propertiesAreUncounted)
                {
                    countPropertyValues();
                }

                return this.nativeCount;
            }
        }

        /// <summary>
        /// The count of documents that have children and 
        /// are also not children of other documents.
        /// </summary>
        public int ParentCount
        {
            get
            {
                if (propertiesAreUncounted)
                {
                    countPropertyValues();
                }

                return this.parentCount;
            }
        }

        /// <summary>
        /// The count of documents with parents.
        /// </summary>
        public int ChildCount
        {
            get
            {
                if (propertiesAreUncounted)
                {
                    countPropertyValues();
                }

                return this.childCount;
            }
        }

        /// <summary>
        /// The count of documents without parents and without children.
        /// </summary>
        public int StandAloneCount
        {
            get
            {
                if (propertiesAreUncounted)
                {
                    countPropertyValues();
                }

                return this.standAloneCount;
            }
        }
        
        public Document this[int index]
        {
            get
            {
                return this.docs[index];
            }

            set
            {
                this.docs[index] = value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.docs.GetEnumerator();
        }

        IEnumerator<Document> IEnumerable<Document>.GetEnumerator()
        {
            return this.docs.GetEnumerator();
        }

        /// <summary>
        /// Resets the value of the collection properties to -1.
        /// </summary>
        protected void propertyReset()
        {            
            this.imageCount = -1;
            this.textCount = -1;
            this.nativeCount = -1;
            this.parentCount = -1;
            this.childCount = -1;
            this.standAloneCount = -1;            
        }

        /// <summary>
        /// Populates the values of the collection's properties.
        /// </summary>
        protected void countPropertyValues()
        {
            this.imageCount = 0;
            this.textCount = 0;
            this.nativeCount = 0;
            this.parentCount = 0;
            this.childCount = 0;
            this.standAloneCount = 0;

            foreach (Document doc in this.docs)
            {
                foreach (var rep in doc.LinkedFiles)
                {
                    if (rep.Type == LinkedFile.FileType.Image)
                    {
                        imageCount += rep.Files.Count;
                    }
                    else if (rep.Type == LinkedFile.FileType.Native)
                    {
                        nativeCount += rep.Files.Count;
                    }
                    else if (rep.Type == LinkedFile.FileType.Text)
                    {
                        textCount += rep.Files.Count;
                    }
                }

                if (doc.Parent != null)
                {
                    childCount++;
                }
                else if (doc.Children != null)
                {
                    parentCount++;
                }
                else
                {
                    standAloneCount++;
                }
            }
        }
    }
}
