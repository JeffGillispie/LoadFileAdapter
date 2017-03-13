using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter
{
    public class DocumentSet : IEnumerable<Document>
    {
        private List<Document> docs = new List<Document>();
        
        public void AddDocuments(List<Document> docs)
        {
            this.docs.AddRange(docs);            
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
        /*
        public Document First()
        {
            return this.docs.First();
        }

        public Document Last()
        {
            return this.docs.Last();
        }

        private class DocEnumerator : IEnumerator
        {
            public List<Document> docs;
            int position = -1;

            public DocEnumerator(List<Document> docs)
            {
                this.docs = docs;
            }

            private IEnumerator<Document> getEnumerator()
            {
                return (IEnumerator<Document>)this;
            }

            public bool MoveNext()
            {
                this.position++;
                return (this.position < this.docs.Count);
            }

            public void Reset()
            {
                this.position = -1;
            }

            object IEnumerator.Current
            {
                get
                {                    
                    return this.docs[this.position];                    
                }
            }
        }        
        */


        IEnumerator IEnumerable.GetEnumerator()
        {
            //return new DocEnumerator(this.docs);
            return this.docs.GetEnumerator();
        }

        IEnumerator<Document> IEnumerable<Document>.GetEnumerator()
        {
            return this.docs.GetEnumerator();
        }
    }
}
