using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// A builder that builds documents from an LFP file.
    /// </summary>
    public class LfpBuilder : IBuilder<BuildDocCollectionImageSettings, BuildDocLfpSettings>
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
        /// Builds a list of documents from an LFP file.
        /// </summary>
        /// <param name="args">Build settings for a document collection.</param>
        /// <returns>Returns a list of <see cref="Document"/> built from an LFP file.</returns>
        public List<Document> BuildDocuments(BuildDocCollectionImageSettings args)
        {
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            List<string[]> pageRecords = new List<string[]>();
            string[] nativeRecord = null;
            Document lastParent = null;
            string lastBreak = String.Empty;
            // build the documents
            foreach (string[] record in args.Records)
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
                            BuildDocLfpSettings docArgs = new BuildDocLfpSettings(
                                pageRecords, nativeRecord, args.TextSettings, args.PathPrefix);
                            Document doc = BuildDocument(docArgs);
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
                        BuildDocLfpSettings nativeArgs = new BuildDocLfpSettings(
                            pageRecords, nativeRecord, args.TextSettings, args.PathPrefix);
                        Document doc = BuildDocument(nativeArgs);
                        string key = doc.Metadata[KEY_FIELD];
                        docs.Add(key, doc);
                    }
                    // make native record equal to the current record
                    nativeRecord = record;
                }
            }
            // add last doc to the collection            
            BuildDocLfpSettings lastArgs = new BuildDocLfpSettings(
                pageRecords, nativeRecord, args.TextSettings, args.PathPrefix);
            Document lastDoc = BuildDocument(lastArgs);
            string lastKey = lastDoc.Metadata[KEY_FIELD];
            // check if a relationship needs to be set
            if (pageRecords.First()[TOKEN_INDEX].Equals(Token.IM.ToString()) && 
                pageRecords.First()[IMAGE_BOUNDARY_FLAG_INDEX].Equals(BoundaryFlag.C.ToString()))
            {
                lastDoc.SetParent(lastParent);
            }

            docs.Add(lastKey, lastDoc);            
            // return document list
            return docs.Values.ToList();
        }

        /// <summary>
        /// Builds a document from an LFP file.
        /// </summary>
        /// <param name="args">Build settings for an LFP file.</param>
        /// <returns>Returns a <see cref="Document"/> built from an LFP file.</returns>
        public Document BuildDocument(BuildDocLfpSettings args)
        {
            // check if this doc has images or is native only then get document properties
            string[] firstPage = (args.PageRecords.Count > 0) ? args.PageRecords.First() : null;            
            string key = (args.PageRecords.Count > 0) 
                ? firstPage[KEY_INDEX] 
                : args.NativeRecord[KEY_INDEX];
            string vol = (args.PageRecords.Count > 0) 
                ? firstPage[IMAGE_VOLUME_NAME_INDEX] 
                : args.NativeRecord[NATIVE_VOLUME_NAME_INDEX];            
            vol = vol.TrimStart(VOLUME_TRIM_START);
            int pages = args.PageRecords.Count;
            // set document properties
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add(KEY_FIELD, key);
            metadata.Add(VOLUME_NAME_FIELD, vol);
            metadata.Add(PAGE_COUNT_FIELD, pages.ToString());            
            // get representatives
            HashSet<Representative> reps = getRepresentatives(
                args.PageRecords, args.PathPrefix, args.TextSettings, args.NativeRecord);
            Document parent = null;
            HashSet<Document> children = null;
            return new Document(key, parent, children, metadata, reps);
        }

        /// <summary>
        /// Gets a documents image representative from an LFP file.
        /// </summary>
        /// <param name="pageRecords">A list of page records from an LFP file for a document.</param>
        /// <param name="pathPrefix">The value to prepend to representative paths.</param>
        /// <returns>Returns an image <see cref="Representative"/>.</returns>
        protected Representative getImageRepresentative(List<string[]> pageRecords, string pathPrefix)
        {
            if (pageRecords.Count > 0)
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
                // set image rep
                return new Representative(Representative.FileType.Image, imageFiles);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a documents native representative.
        /// </summary>
        /// <param name="nativeRecord">The native record field values from an LFP file.</param>
        /// <param name="pathPrefix">The value to preprend to representative paths.</param>
        /// <returns>Returns a native <see cref="Representative"/>.</returns>
        protected Representative getNativeRepresentative(string[] nativeRecord, string pathPrefix)
        {
            if (nativeRecord == null)
            {
                return null;
            }
            else
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
                return new Representative(Representative.FileType.Native, nativeFiles);
            }
        }

        /// <summary>
        /// Gets a documents text representative.
        /// </summary>
        /// <param name="pageRecords">A list of page records for a document.</param>
        /// <param name="pathPrefix">The value to preprend to representative paths.</param>
        /// <param name="textSetting">The text representative settings.</param>
        /// <returns>Returns the text <see cref="Representative"/>.</returns>
        protected Representative getTextRepresentative(
            List<string[]> pageRecords, string pathPrefix, TextRepresentativeSettings textSetting)
        {
            TextRepresentativeSettings.TextLevel textLevel = (textSetting != null)
                ? textSetting.FileLevel
                : TextRepresentativeSettings.TextLevel.None;
            SortedDictionary<string, string> textFiles = new SortedDictionary<string, string>();

            switch(textLevel)
            {
                case TextRepresentativeSettings.TextLevel.None:
                    // do nothing here
                    return null;
                case TextRepresentativeSettings.TextLevel.Page:
                    foreach (string[] page in pageRecords)
                    {
                        string pageTextKey = page[KEY_INDEX];
                        string pageTextPath = String.IsNullOrEmpty(pathPrefix)
                            ? page[IMAGE_FILE_PATH_INDEX]
                            : Path.Combine(
                                pathPrefix, 
                                page[IMAGE_FILE_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                        string filename = page[IMAGE_FILE_NAME_INDEX];                        
                        pageTextPath = Path.Combine(pageTextPath, filename);
                        pageTextPath = textSetting.GetTextPathFromImagePath(pageTextPath);
                        textFiles.Add(pageTextKey, pageTextPath);
                    }
                    return new Representative(Representative.FileType.Text, textFiles);
                case TextRepresentativeSettings.TextLevel.Doc:
                    string[] docRecord = pageRecords.First();
                    string docTextKey = docRecord[KEY_INDEX];
                    string docTextPath = String.IsNullOrEmpty(pathPrefix)
                        ? docRecord[IMAGE_FILE_PATH_INDEX]
                        : Path.Combine(
                            pathPrefix, 
                            docRecord[IMAGE_FILE_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                    docTextPath = Path.Combine(docTextPath, docRecord[IMAGE_FILE_NAME_INDEX]);
                    docTextPath = textSetting.GetTextPathFromImagePath(docTextPath);
                    textFiles.Add(docTextKey, docTextPath);
                    return new Representative(Representative.FileType.Text, textFiles);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get any image, native, or text representatives from the LFP data
        /// and text setting.
        /// </summary>
        /// <param name="pageRecords">A list of LFP page records for a document.</param>
        /// <param name="pathPrefix">The value to preprend to representative paths.</param>
        /// <param name="textSettings">The text representative settings.</param>
        /// <param name="nativeRecord">The native record field values for this document.</param>
        /// <returns>Returns a set of <see cref="Representative"/>.</returns>
        protected HashSet<Representative> getRepresentatives(
            List<string[]> pageRecords, string pathPrefix, 
            TextRepresentativeSettings textSettings, string[] nativeRecord)
        {
            HashSet<Representative> reps = new HashSet<Representative>();

            Representative imageRep = getImageRepresentative(pageRecords, pathPrefix);
            Representative textRep = getTextRepresentative(pageRecords, pathPrefix, textSettings);
            Representative nativeRep = getNativeRepresentative(nativeRecord, pathPrefix);

            if (imageRep != null)
                reps.Add(imageRep);

            if (textRep != null)
                reps.Add(textRep);

            if (nativeRep != null)
                reps.Add(nativeRep);

            return reps;
        }
    }
}
