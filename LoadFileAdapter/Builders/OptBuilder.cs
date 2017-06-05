using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// A builder that builds documents from an OPT file.
    /// </summary>
    public class OptBuilder : IBuilder
    {
        internal const string IMAGE_KEY_FIELD = "DocID";
        internal const string VOLUME_NAME_FIELD = "Volume Name";
        internal const string PAGE_COUNT_FIELD = "Page Count";
        internal const string BOX_BREAK_FIELD = "Box Break";
        internal const string FOLDER_BREAK_FIELD = "Folder Break";
        private const int IMAGE_KEY_INDEX = 0;
        private const int VOLUME_NAME_INDEX = 1;
        private const int FULL_PATH_INDEX = 2;
        private const int DOC_BREAK_INDEX = 3;
        private const int BOX_BREAK_INDEX = 4;
        private const int FOLDER_BREAK_INDEX = 5;
        private const int PAGE_COUNT_INDEX = 6;
        private const string TRUE_VALUE = "Y";        
        private const char FILE_PATH_DELIM = '\\';
        private string pathPrefix;
        private TextBuilder textBuilder;

        /// <summary>
        /// Gets or sets the builder that makes a <see cref="Representative"/> for 
        /// a text file.
        /// </summary>
        public TextBuilder TextBuilder
        {
            get
            {
                return this.textBuilder;
            }

            set
            {
                this.textBuilder = value;
            }
        }

        /// <summary>
        /// Gets or sets the path prefix to prepend to any <see cref="Representative"/>
        /// file paths.
        /// </summary>
        public string PathPrefix
        {
            get
            {
                return this.pathPrefix;
            }

            set
            {
                this.pathPrefix = value;
            }
        }

        /// <summary>
        /// Builds documents from an OPT file.
        /// </summary>
        /// <param name="records">The parsed lines of an OPT file.</param>
        /// <returns></returns>
        public List<Document> Build(IEnumerable<string[]> records)
        {
            // setup for building            
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            List<string[]> pages = new List<string[]>();
            // build the documents
            foreach (string[] record in records)
            {
                // check for a doc break
                if (record[DOC_BREAK_INDEX].ToUpper().Equals(TRUE_VALUE))
                {
                    // send data to make a document
                    if (pages.Count > 0)
                    {
                        Document doc = BuildDocument(pages);
                        string key = doc.Metadata[IMAGE_KEY_FIELD];
                        docs.Add(key, doc);
                    }
                    // clear docPages and add new first page
                    pages = new List<string[]>();
                    pages.Add(record);
                }
                else
                {
                    // add page to document pages
                    pages.Add(record);
                }
            }
            // add last doc to the collection
            Document lastDoc = BuildDocument(pages);
            string lastKey = lastDoc.Metadata[IMAGE_KEY_FIELD];
            docs.Add(lastKey, lastDoc);
            return docs.Values.ToList();
        }

        /// <summary>
        /// Builds a <see cref="Document"/> from an OPT file.
        /// </summary>
        /// <param name="pages">The parsed lines for a document.</param>
        /// <returns>Returns a <see cref="Document"/>.</returns>
        public Document BuildDocument(IEnumerable<string[]> pages)
        {
            // get document properties
            string[] pageOne = pages.First();
            string key = pageOne[IMAGE_KEY_INDEX];
            string vol = pageOne[VOLUME_NAME_INDEX];
            string box = pageOne[BOX_BREAK_INDEX];
            string dir = pageOne[FOLDER_BREAK_INDEX];
            int pagesCount = pages.Count();
            // set document properties
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add(IMAGE_KEY_FIELD, key);
            metadata.Add(VOLUME_NAME_FIELD, vol);
            metadata.Add(PAGE_COUNT_FIELD, pagesCount.ToString());
            //metadata.Add(BOX_BREAK_FIELD, box); // extraneous meta
            //metadata.Add(FOLDER_BREAK_FIELD, dir); // extraneous meta
            // build the representatives
            Representative imageRep = getImageRepresentative(pages);          
            Representative textRep = getTextRepresentative(pages);
            HashSet<Representative> reps = new HashSet<Representative>();
            reps.Add(imageRep);            
            reps.Add(textRep);
            reps.Remove(null);
            // no family relationships in an opt
            Document parent = null;
            HashSet<Document> children = null;
            return new Document(key, parent, children, metadata, reps);
        }

        /// <summary>
        /// Obtains the image representative for a document.
        /// </summary>
        /// <param name="pages">The page records from an OPT file for a document.</param>
        /// <returns>Returns an image file <see cref="Representative"/>.</returns>
        protected Representative getImageRepresentative(IEnumerable<string[]> pages)
        {
            SortedDictionary<string, string> imageFiles = new SortedDictionary<string, string>();
            // add image files
            foreach (string[] page in pages)
            {             
                string imageKey = page[IMAGE_KEY_INDEX];
                string imagePath = (String.IsNullOrEmpty(pathPrefix))
                    ? page[FULL_PATH_INDEX]
                    : Path.Combine(pathPrefix, page[FULL_PATH_INDEX].TrimStart(FILE_PATH_DELIM));                
                imageFiles.Add(imageKey, imagePath);
            }

            return new Representative(Representative.FileType.Image, imageFiles);
        }

        /// <summary>
        /// Obtains the text representative for a document.
        /// </summary>
        /// <param name="pages">The page records from an OPT file for a document.</param>        
        /// <returns>Returns a text file <see cref="Representative"/> of a document.</returns>
        protected Representative getTextRepresentative(IEnumerable<string[]> pages)
        {
            Func<string[], KeyValuePair<string, string>> assembler = (page) =>
            {
                string key = page[IMAGE_KEY_INDEX];
                string path = (String.IsNullOrEmpty(pathPrefix))
                    ? page[FULL_PATH_INDEX]
                    : Path.Combine(pathPrefix, page[FULL_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                return new KeyValuePair<string, string>(key, path);
            };
                        
            return (textBuilder != null)
                ? textBuilder.Build(pages, assembler)
                : null;
        }
    }
}