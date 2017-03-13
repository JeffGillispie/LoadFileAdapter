﻿using System;
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
        private int imageCount = -1;
        private int textCount = -1;
        private int nativeCount = -1;
        private int parentCount = -1;
        private int childCount = -1;
        private int standAloneCount = -1;
        
        public void AddDocuments(IEnumerable<Document> docs)
        {
            this.docs.AddRange(docs);
            
            if (this.imageCount != -1)
            {
                countValues();
            }
        }

        public int Count
        {
            get
            {
                return this.docs.Count;
            }
        }

        public int ImageCount
        {
            get
            {
                if (this.imageCount == -1)
                {
                    countValues();
                }

                return this.imageCount;
            }
        }

        public int TextCount
        {
            get
            {
                if (this.textCount == -1)
                {
                    countValues();
                }

                return this.textCount;
            }
        }

        public int NativeCount
        {
            get
            {
                if (this.nativeCount == -1)
                {
                    countValues();
                }

                return this.nativeCount;
            }
        }

        public int ParentCount
        {
            get
            {
                if (this.parentCount == -1)
                {
                    countValues();
                }

                return this.parentCount;
            }
        }

        public int ChildCount
        {
            get
            {
                if (this.childCount == -1)
                {
                    countValues();
                }

                return this.childCount;
            }
        }

        public int StandAloneCount
        {
            get
            {
                if (this.standAloneCount == -1)
                {
                    countValues();
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

        private void countValues()
        {
            this.imageCount = 0;
            this.textCount = 0;
            this.nativeCount = 0;
            this.parentCount = 0;
            this.childCount = 0;
            this.standAloneCount = 0;

            foreach (Document doc in this.docs)
            {
                foreach (var rep in doc.Representatives)
                {
                    if (rep.RepresentativeType == Representative.Type.Image)
                    {
                        imageCount += rep.Files.Count;
                    }
                    else if (rep.RepresentativeType == Representative.Type.Native)
                    {
                        nativeCount += rep.Files.Count;
                    }
                    else if (rep.RepresentativeType == Representative.Type.Text)
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
