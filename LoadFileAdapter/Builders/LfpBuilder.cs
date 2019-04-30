using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// A builder that builds documents from an LFP file.
    /// </summary>
    public class LfpBuilder : IBuilder
    {
        internal const string KEY_FIELD = "DocID";
        internal const string VOLUME_NAME_FIELD = "Volume Name";
        internal const string PAGE_COUNT_FIELD = "Page Count";
        private const int TOKEN_INDEX = 0;
        private const int KEY_INDEX = 1;
        private const int IMAGE_BOUNDARY_FLAG_INDEX = 2;
        private const int IMAGE_OFFSET_INDEX = 3;
        private const int IMAGE_VOLUME_NAME_INDEX = 4;
        private const int IMAGE_FILE_PATH_INDEX = 5;
        private const int IMAGE_FILE_NAME_INDEX = 6;
        private const int IMAGE_TYPE_INDEX = 7;
        private const int IMAGE_ROTATION_INDEX = 8;
        private const int NATIVE_VOLUME_NAME_INDEX = 2;
        private const int NATIVE_FILE_PATH_INDEX = 3;
        private const int NATIVE_FILE_NAME_INDEX = 4;
        private const int NATIVE_OFFSET_INDEX = 5;                
        private const char VOLUME_TRIM_START = '@';
        private const char FILE_PATH_DELIM = '\\';
        private TextBuilder textBuilder;
        private string pathPrefix;

        /// <summary>
        /// Gets or sets the builder that makes a <see cref="Representative"/> for 
        /// text files.
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
        /// Gets or sets the path prefix to prepended to any <see cref="Representative"/>
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
        /// The token used in an LFP to designate a record type.
        /// </summary>
        private enum Token
        {
            /// <summary>
            /// An image record.
            /// </summary>
            IM,
            /// <summary>
            /// A native record.
            /// </summary>
            OF
        }

        /// <summary>
        /// The flag used in an LFP to designate a boundary type.
        /// </summary>
        internal enum BoundaryFlag
        {
            /// <summary>
            /// A document break.
            /// </summary>
            D,
            /// <summary>
            /// A child document break;
            /// </summary>
            C
        }
        
        /// <summary>
        /// Builds documents from a LFP file.
        /// </summary>
        /// <param name="records">The parsed lines of a LFP file.</param>
        /// <returns>Returns a list of <see cref="Document"/>.</returns>
        public List<Document> Build(IEnumerable<string[]> records)
        {            
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            List<string[]> pageRecords = new List<string[]>();
            string[] nativeRecord = null;
            Document lastParent = null;
            string lastBreak = String.Empty;
            // build the documents
            foreach (string[] record in records)
            {
                Token token = (Token)Enum.Parse(typeof(Token), record[TOKEN_INDEX]);
                // determine if the line is an image or native
                if (token.Equals(Token.IM))
                {
                    // check for a doc break
                    if (!String.IsNullOrWhiteSpace(record[IMAGE_BOUNDARY_FLAG_INDEX]))
                    {
                        // send data to make a document if there is data to send
                        // this is a guard against the first line in the list
                        if (pageRecords.Count > 0)
                        {
                            pageRecords.Insert(0, nativeRecord);
                            Document doc = BuildDocument(pageRecords);
                            string key = doc.Metadata[KEY_FIELD];
                            BoundaryFlag docBreak = (BoundaryFlag)Enum.Parse(typeof(BoundaryFlag), lastBreak);
                            // check if document is a child
                            if (docBreak.Equals(BoundaryFlag.C))
                            {   
                                doc.SetParent(lastParent);
                            }
                            else
                            {
                                // document is a parent
                                lastParent = doc;
                            }
                            // add document to collection
                            docs.Add(key, doc);
                        }
                        // clear docPages and add new first page
                        pageRecords = new List<string[]>();
                        pageRecords.Add(record);
                        lastBreak = record[IMAGE_BOUNDARY_FLAG_INDEX];
                        // check if native belongs to this doc
                        // this is a guard against the native appearing before the first image
                        if (nativeRecord != null && !nativeRecord[KEY_INDEX].Equals(record[KEY_INDEX]))
                        {
                            nativeRecord = null;
                        }
                    }
                    else
                    {
                        // add page to document pages
                        pageRecords.Add(record);
                    }
                }
                else if (token.Equals(Token.OF))
                {
                    // check if native record is null
                    // it should be null after an image line with a doc break is read that doesn't match the key
                    // if it is not null then the native has no corresponding images so send the data to make a document
                    if (nativeRecord != null)
                    {
                        pageRecords.Insert(0, nativeRecord);
                        Document doc = BuildDocument(pageRecords);
                        string key = doc.Metadata[KEY_FIELD];
                        docs.Add(key, doc);
                    }
                    // make native record equal to the current record
                    nativeRecord = record;
                }
            }
            // add last doc to the collection
            pageRecords.Insert(0, nativeRecord);
            Document lastDoc = BuildDocument(pageRecords);
            string lastKey = lastDoc.Metadata[KEY_FIELD];
            // check if a relationship needs to be set
            if (pageRecords.First(r => r != null)[TOKEN_INDEX].Equals(Token.IM.ToString()) && 
                pageRecords.First(r => r != null)[IMAGE_BOUNDARY_FLAG_INDEX].Equals(BoundaryFlag.C.ToString()))
            {
                lastDoc.SetParent(lastParent);
            }

            docs.Add(lastKey, lastDoc);            
            // return document list
            return docs.Values.ToList();
        }

        /// <summary>
        /// Builds a <see cref="Document"/> from a LFP file.
        /// </summary>
        /// <param name="docRecords">The set of parsed lines for a single document. 
        /// The first line should be null or the native line.</param>
        /// <returns>Returns a <see cref="Document"/>.</returns>
        public Document BuildDocument(IEnumerable<string[]> docRecords)
        {
            string[] nativeRecord = docRecords.First();
            IEnumerable<string[]> pageRecords = docRecords.Skip(1);            
            // check if this doc has images or is native only then get document properties
            string[] firstPage = (pageRecords.Count() > 0) ? pageRecords.First() : null;            
            string key = (pageRecords.Count() > 0) 
                ? firstPage[KEY_INDEX] 
                : nativeRecord[KEY_INDEX];
            string vol = (pageRecords.Count() > 0) 
                ? firstPage[IMAGE_VOLUME_NAME_INDEX] 
                : nativeRecord[NATIVE_VOLUME_NAME_INDEX];            
            vol = vol.TrimStart(VOLUME_TRIM_START);
            int pages = pageRecords.Count();
            // set document properties
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add(KEY_FIELD, key);
            metadata.Add(VOLUME_NAME_FIELD, vol);
            metadata.Add(PAGE_COUNT_FIELD, pages.ToString());
            // get representatives
            HashSet<Representative> reps = new HashSet<Representative>();
            reps.Add(getImageRepresentative(pageRecords));
            reps.Add(getTextRepresentative(pageRecords));
            reps.Add(getNativeRepresentative(nativeRecord));
            reps.Remove(null);
            //HashSet<Representative> reps = getRepresentatives(pageRecords, nativeRecord);
            Document parent = null;
            HashSet<Document> children = null;
            return new Document(key, parent, children, metadata, reps);
        }

        /// <summary>
        /// Gets a documents image representative from an LFP file.
        /// </summary>
        /// <param name="pageRecords">A list of page records from an LFP file for a document.</param>
        /// <returns>Returns an image <see cref="Representative"/>.</returns>
        protected Representative getImageRepresentative(IEnumerable<string[]> pageRecords)
        {
            Representative imageRep = null;

            if (pageRecords.Count() > 0)
            {
                SortedDictionary<string, string> imageFiles = new SortedDictionary<string, string>();
                // get image files
                foreach (string[] page in pageRecords)
                {
                    int offset = int.Parse(page[IMAGE_OFFSET_INDEX]);
                    // exclude multi-page image references
                    if (offset == 0 || offset == 1)
                    {
                        string imageKey = page[KEY_INDEX];
                        string filePath = String.IsNullOrEmpty(pathPrefix)
                            ? page[IMAGE_FILE_PATH_INDEX]
                            : Path.Combine(
                                pathPrefix, 
                                page[IMAGE_FILE_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                        filePath = Path.Combine(filePath, page[IMAGE_FILE_NAME_INDEX]);                        
                        imageFiles.Add(imageKey, filePath);
                    }
                }
                
                imageRep = new Representative(Representative.FileType.Image, imageFiles);
            }           

            return imageRep;
        }

        /// <summary>
        /// Gets a documents native representative.
        /// </summary>
        /// <param name="nativeRecord">The native record field values from an LFP file.</param>
        /// <returns>Returns a native <see cref="Representative"/>.</returns>
        protected Representative getNativeRepresentative(string[] nativeRecord)
        {
            Representative nativeRep = null;

            if (nativeRecord != null)
            {                
                SortedDictionary<string, string> nativeFiles = new SortedDictionary<string, string>();
                string nativeKey = nativeRecord[KEY_INDEX];
                string nativePath = String.IsNullOrEmpty(pathPrefix)
                    ? nativeRecord[NATIVE_FILE_PATH_INDEX]
                    : Path.Combine(
                        pathPrefix, 
                        nativeRecord[NATIVE_FILE_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                nativePath = Path.Combine(nativePath, nativeRecord[NATIVE_FILE_NAME_INDEX]);                
                nativeFiles.Add(nativeKey, nativePath);
                nativeRep = new Representative(Representative.FileType.Native, nativeFiles);
            }

            return nativeRep;
        }

        /// <summary>
        /// Gets a documents text representative.
        /// </summary>
        /// <param name="pageRecords">A list of page records for a document.</param>        
        /// <returns>Returns the text <see cref="Representative"/>.</returns>
        protected Representative getTextRepresentative(IEnumerable<string[]> pageRecords)
        {
            Func<string[], KeyValuePair<string, string>> assembler = (page) =>
            {
                string key = page[KEY_INDEX];                
                string path = (String.IsNullOrEmpty(pathPrefix))
                    ? Path.Combine(
                        page[IMAGE_FILE_PATH_INDEX],
                        page[IMAGE_FILE_NAME_INDEX])
                    : Path.Combine(
                        pathPrefix, 
                        page[IMAGE_FILE_PATH_INDEX].TrimStart(FILE_PATH_DELIM),
                        page[IMAGE_FILE_NAME_INDEX]);                
                return new KeyValuePair<string, string>(key, path);
            };

            return (textBuilder != null)
                ? textBuilder.Build(pageRecords, assembler)
                : null;
        }        
    }
}
