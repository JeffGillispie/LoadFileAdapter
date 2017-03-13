using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class OptBuilder : Builder
    {
        private const int IMAGE_KEY_INDEX = 0;
        private const int VOLUME_NAME_INDEX = 1;
        private const int FULL_PATH_INDEX = 2;
        private const int DOC_BREAK_INDEX = 3;
        private const int BOX_BREAK_INDEX = 4;
        private const int FOLDER_BREAK_INDEX = 5;
        private const int PAGE_COUNT_INDEX = 6;
        private const string TRUE_VALUE = "Y";
        internal const string IMAGE_KEY_FIELD = "DocID";
        internal const string VOLUME_NAME_FIELD = "Volume Name";
        internal const string PAGE_COUNT_FIELD = "Page Count";
        internal const string BOX_BREAK_FIELD = "Box Break";
        internal const string FOLDER_BREAK_FIELD = "Folder Break";
        private const char FILE_PATH_DELIM = '\\';
        
        public List<Document> BuildDocuments(DocumentSetBuilderArgs e)
        {   
            // setup for building         
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            List<string[]> pageRecords = new List<string[]>();
            // build the documents
            foreach (string[] record in e.Records)
            {
                // check for a doc break
                if (record[DOC_BREAK_INDEX].ToUpper().Equals(TRUE_VALUE))
                {
                    // send data to make a document
                    if (pageRecords.Count > 0)
                    {
                        DocumentBuilderArgs args = DocumentBuilderArgs.GetOptArgs(pageRecords, e.PathPrefix, e.TextRepresentativeSetting);
                        Document doc = BuildDocument(args);
                        string key = doc.Metadata[IMAGE_KEY_FIELD];
                        docs.Add(key, doc);
                    }
                    // clear docPages and add new first page
                    pageRecords = new List<string[]>();
                    pageRecords.Add(record);
                }
                else
                {
                    // add page to document pages
                    pageRecords.Add(record);
                }
            }
            // add last doc to the collection
            DocumentBuilderArgs lastArgs = DocumentBuilderArgs.GetOptArgs(pageRecords, e.PathPrefix, e.TextRepresentativeSetting);
            Document lastDoc = BuildDocument(lastArgs);
            string lastKey = lastDoc.Metadata[IMAGE_KEY_FIELD];
            docs.Add(lastKey, lastDoc);
            return docs.Values.ToList();
        }

        public Document BuildDocument(DocumentBuilderArgs e)
        {            
            // get document properties
            string[] pageOne = e.PageRecords.First();
            string key = pageOne[IMAGE_KEY_INDEX];
            string vol = pageOne[VOLUME_NAME_INDEX];
            string box = pageOne[BOX_BREAK_INDEX];
            string dir = pageOne[FOLDER_BREAK_INDEX];
            int pages = e.PageRecords.Count;
            // set document properties
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add(IMAGE_KEY_FIELD, key);
            metadata.Add(VOLUME_NAME_FIELD, vol);
            metadata.Add(PAGE_COUNT_FIELD, pages.ToString());
            //metadata.Add(BOX_BREAK_FIELD, box); // extraneous meta
            //metadata.Add(FOLDER_BREAK_FIELD, dir); // extraneous meta
            // build the representatives
            Representative imageRep = getImageRepresentative(e.PageRecords, e.PathPrefex);          
            Representative textRep = getTextRepresentative(e.PageRecords, e.PathPrefex, e.TextSetting);
            HashSet<Representative> reps = new HashSet<Representative>();
            reps.Add(imageRep);
            if (textRep.Files.Count > 0)
                reps.Add(textRep);
            // no family relationships in an opt
            Document parent = null;
            List<Document> children = null;
            return new Document(key, parent, children, metadata, reps);
        }

        private Representative getImageRepresentative(List<string[]> pageRecords, string pathPrefix)
        {
            SortedDictionary<string, FileInfo> imageFiles = new SortedDictionary<string, FileInfo>();
            // add image files
            pageRecords.ForEach(page => {
                string imageKey = page[IMAGE_KEY_INDEX];
                string filePath = (String.IsNullOrEmpty(pathPrefix))
                    ? page[FULL_PATH_INDEX]
                    : Path.Combine(pathPrefix, page[FULL_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                FileInfo imageFile = new FileInfo(filePath);
                imageFiles.Add(imageKey, imageFile);
            });
            return new Representative(Representative.Type.Image, imageFiles);
        }

        private Representative getTextRepresentative(List<string[]> pageRecords, string pathPrefix, StructuredRepresentativeSetting textSetting)
        {
            SortedDictionary<string, FileInfo> textFiles = new SortedDictionary<string, FileInfo>();
            StructuredRepresentativeSetting.TextLevel textLevel = (textSetting != null)
                ? textSetting.RepresentativeTextLevel
                : StructuredRepresentativeSetting.TextLevel.None;
            // add text files
            switch (textLevel)
            {
                case StructuredRepresentativeSetting.TextLevel.None:
                    // do nothing here
                    break;
                case StructuredRepresentativeSetting.TextLevel.Page:
                    pageRecords.ForEach(page => {
                        string pageTextKey = page[IMAGE_KEY_INDEX];
                        string pageTextFilePath = textSetting.GetTextPathFromImagePath(page[FULL_PATH_INDEX]);
                        pageTextFilePath = String.IsNullOrEmpty(pathPrefix)
                            ? pageTextFilePath
                            : Path.Combine(pathPrefix, pageTextFilePath.TrimStart(FILE_PATH_DELIM));
                        FileInfo pageTextFile = new FileInfo(pageTextFilePath);
                        textFiles.Add(pageTextKey, pageTextFile);
                    });
                    break;
                case StructuredRepresentativeSetting.TextLevel.Doc:
                    string docTextKey = pageRecords.First()[IMAGE_KEY_INDEX];
                    string docTextFilePath = textSetting.GetTextPathFromImagePath(pageRecords.First()[FULL_PATH_INDEX]);
                    docTextFilePath = String.IsNullOrEmpty(pathPrefix)
                            ? docTextFilePath
                            : Path.Combine(pathPrefix, docTextFilePath.TrimStart(FILE_PATH_DELIM));
                    FileInfo docTextFile = new FileInfo(docTextFilePath);
                    textFiles.Add(docTextKey, docTextFile);
                    break;
                default:
                    // do nothing here
                    break;
            }
            // return a text rep
            return new Representative(Representative.Type.Text, textFiles);
        }
    }
}
