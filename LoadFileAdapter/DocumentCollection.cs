using System.Collections;
using System.Collections.Generic;
using LoadFileAdapter.Transformers;

namespace LoadFileAdapter
{
    /// <summary>
    /// Represents a collection of <see cref="Document"/>.
    /// </summary>
    public class DocumentCollection : IEnumerable<Document>
    {
        private List<Document> documentList = new List<Document>();
        private Dictionary<string, Document> documentGlossary = new Dictionary<string, Document>();                
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
            // do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCollection"/> class.
        /// </summary>
        /// <param name="documents">The documents used to populate the collection.</param>
        public DocumentCollection(IEnumerable<Document> documents) : this()
        {
            AddRange(documents);
        }
        
        /// <summary>
        /// The count of documents in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.documentList.Count;
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
                return this.documentList[index];
            }

            set
            {
                string key = this.documentList[index].Key;
                this.documentGlossary[key] = value;
                this.documentList[index] = value;
                propertyReset();
            }
        }

        public Document this[string key]
        {
            get
            {
                return this.documentGlossary[key];
            }

            set
            {
                int index = this.documentList.BinarySearch(this.documentGlossary[key]);
                this.documentGlossary[key] = value;
                this.documentList[index] = value;
                propertyReset();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.documentGlossary.GetEnumerator();
        }

        IEnumerator<Document> IEnumerable<Document>.GetEnumerator()
        {
            return this.documentGlossary.Values.GetEnumerator();
        }
        
        /// <summary>
        /// Adds a <see cref="Document"/> to the collection.
        /// </summary>
        /// <param name="doc">The document to add to the collection.</param>
        public void Add(Document document)
        {
            AddRange(new Document[] { document });
        }

        /// <summary>
        /// Adds a series of documents to the collection.
        /// </summary>
        /// <param name="documents">The documents to add to the collection.</param>
        public void AddRange(IEnumerable<Document> documents)
        {
            foreach (Document document in documents)
            {
                if (this.documentGlossary.ContainsKey(document.Key))
                {
                    Overlayer overlayer = new Overlayer(true, true, true);
                    Document original = this.documentGlossary[document.Key];
                    Document newDoc = overlayer.Overlay(original, document);
                    int index = this.documentList.BinarySearch(original);
                    this.documentList[index] = newDoc;
                    this.documentGlossary[document.Key] = newDoc;
                }
                else
                {
                    this.documentList.Add(document);
                    this.documentGlossary.Add(document.Key, document);
                }
            }

            propertyReset();
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
            this.propertiesAreUncounted = true;            
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

            foreach (Document doc in this.documentList)
            {
                if (doc.Representatives != null)
                {
                    foreach (var representative in doc.Representatives)
                    {
                        if (representative.Type == Representative.FileType.Image)
                        {
                            imageCount += representative.Files.Count;
                        }
                        else if (representative.Type == Representative.FileType.Native)
                        {
                            nativeCount += representative.Files.Count;
                        }
                        else if (representative.Type == Representative.FileType.Text)
                        {
                            textCount += representative.Files.Count;
                        }
                    }
                }

                if (doc.Parent != null)
                {
                    this.childCount++;
                }
                else if (doc.Children != null)
                {
                    this.parentCount++;
                }
                else
                {
                    this.standAloneCount++;
                }
            }

            this.propertiesAreUncounted = false;
        }        
    }
}
