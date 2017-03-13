﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class LfpBuilder : Builder
    {
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
        internal const string KEY_FIELD = "DocID";
        internal const string VOLUME_NAME_FIELD = "Volume Name";
        internal const string PAGE_COUNT_FIELD = "Page Count";        
        private const char VOLUME_TRIM_START = '@';
        private const char FILE_PATH_DELIM = '\\';        

        private enum Token
        {
            IM, OF
        }

        internal enum BoundaryFlag
        {
            D, C
        }
        
        public List<Document> BuildDocuments(DocumentSetBuilderArgs e)
        {
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            List<string[]> pageRecords = new List<string[]>();
            string[] nativeRecord = null;
            Document lastParent = null;
            string lastBreak = String.Empty;
            // build the documents
            foreach (string[] record in e.Records)
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
                            DocumentBuilderArgs args = DocumentBuilderArgs.GetLfpArgs(pageRecords, nativeRecord, e.PathPrefix, e.TextRepresentativeSetting);
                            Document doc = BuildDocument(args);
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
                        DocumentBuilderArgs nativeArgs = DocumentBuilderArgs.GetLfpArgs(pageRecords, nativeRecord, e.PathPrefix, e.TextRepresentativeSetting);
                        Document doc = BuildDocument(nativeArgs);
                        string key = doc.Metadata[KEY_FIELD];
                        docs.Add(key, doc);
                    }
                    // make native record equal to the current record
                    nativeRecord = record;
                }
            }
            // add last doc to the collection
            DocumentBuilderArgs lastArgs = DocumentBuilderArgs.GetLfpArgs(pageRecords, nativeRecord, e.PathPrefix, e.TextRepresentativeSetting);
            Document lastDoc = BuildDocument(lastArgs);
            string lastKey = lastDoc.Metadata[KEY_FIELD];
            // check if a relationship needs to be set
            if (pageRecords.First()[TOKEN_INDEX].Equals(Token.IM.ToString()) && pageRecords.First()[IMAGE_BOUNDARY_FLAG_INDEX].Equals(BoundaryFlag.C.ToString()))
            {
                lastDoc.SetParent(lastParent);
            }

            docs.Add(lastKey, lastDoc);            
            // return document list
            return docs.Values.ToList();
        }

        public Document BuildDocument(DocumentBuilderArgs e)
        {
            // check if this doc has images or is native only then get document properties
            string[] firstPage = (e.PageRecords.Count > 0) ? e.PageRecords.First() : null;            
            string key = (e.PageRecords.Count > 0) ? firstPage[KEY_INDEX] : e.NativeRecord[KEY_INDEX];
            string vol = (e.PageRecords.Count > 0) ? firstPage[IMAGE_VOLUME_NAME_INDEX] : e.NativeRecord[NATIVE_VOLUME_NAME_INDEX];            
            vol = vol.TrimStart(VOLUME_TRIM_START);
            int pages = e.PageRecords.Count;
            // set document properties
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add(KEY_FIELD, key);
            metadata.Add(VOLUME_NAME_FIELD, vol);
            metadata.Add(PAGE_COUNT_FIELD, pages.ToString());            
            // get representatives
            HashSet<Representative> reps = getRepresentatives(e.PageRecords, e.PathPrefex, e.TextSetting, e.NativeRecord);
            Document parent = null;
            List<Document> children = null;
            return new Document(key, parent, children, metadata, reps);
        }

        private Representative getImageRepresentative(List<string[]> pageRecords, string pathPrefix)
        {
            if (pageRecords.Count > 0)
            {
                SortedDictionary<string, FileInfo> imageFiles = new SortedDictionary<string, FileInfo>();
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
                            : Path.Combine(pathPrefix, page[IMAGE_FILE_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                        filePath = Path.Combine(filePath, page[IMAGE_FILE_NAME_INDEX]);
                        FileInfo imageFile = new FileInfo(filePath);
                        imageFiles.Add(imageKey, imageFile);
                    }
                }
                // set image rep
                return new Representative(Representative.Type.Image, imageFiles);
            }
            else
            {
                return null;
            }
        }

        private Representative getNativeRepresentative(string[] nativeRecord, string pathPrefix)
        {
            if (nativeRecord == null)
            {
                return null;
            }
            else
            {
                SortedDictionary<string, FileInfo> nativeFiles = new SortedDictionary<string, FileInfo>();
                string nativeKey = nativeRecord[KEY_INDEX];
                string filePath = String.IsNullOrEmpty(pathPrefix)
                    ? nativeRecord[NATIVE_FILE_PATH_INDEX]
                    : Path.Combine(pathPrefix, nativeRecord[NATIVE_FILE_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                filePath = Path.Combine(filePath, nativeRecord[NATIVE_FILE_NAME_INDEX]);
                FileInfo nativeFile = new FileInfo(filePath);
                nativeFiles.Add(nativeKey, nativeFile);
                return new Representative(Representative.Type.Native, nativeFiles);
            }
        }

        private Representative getTextRepresentative(List<string[]> pageRecords, string pathPrefix, StructuredRepresentativeSetting textSetting)
        {
            StructuredRepresentativeSetting.TextLevel textLevel = (textSetting != null)
                ? textSetting.RepresentativeTextLevel
                : StructuredRepresentativeSetting.TextLevel.None;
            SortedDictionary<string, FileInfo> textFiles = new SortedDictionary<string, FileInfo>();

            switch(textLevel)
            {
                case StructuredRepresentativeSetting.TextLevel.None:
                    return null;
                case StructuredRepresentativeSetting.TextLevel.Page:
                    foreach (string[] page in pageRecords)
                    {
                        string pageTextKey = page[KEY_INDEX];
                        string pageTextPath = String.IsNullOrEmpty(pathPrefix)
                            ? page[IMAGE_FILE_PATH_INDEX]
                            : Path.Combine(pathPrefix, page[IMAGE_FILE_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                        pageTextPath = Path.Combine(pageTextPath, page[IMAGE_FILE_NAME_INDEX]);
                        FileInfo pageTextFile = new FileInfo(pageTextPath);
                        textFiles.Add(pageTextKey, pageTextFile);
                    }
                    return new Representative(Representative.Type.Text, textFiles);
                case StructuredRepresentativeSetting.TextLevel.Doc:
                    string[] docRecord = pageRecords.First();
                    string docTextKey = docRecord[KEY_INDEX];
                    string docTextPath = String.IsNullOrEmpty(pathPrefix)
                        ? docRecord[IMAGE_FILE_PATH_INDEX]
                        : Path.Combine(pathPrefix, docRecord[IMAGE_FILE_PATH_INDEX].TrimStart(FILE_PATH_DELIM));
                    docTextPath = Path.Combine(docTextPath, docRecord[IMAGE_FILE_NAME_INDEX]);
                    FileInfo docTextFile = new FileInfo(docTextPath);
                    textFiles.Add(docTextKey, docTextFile);
                    return new Representative(Representative.Type.Text, textFiles);
                default:
                    return null;
            }
        }

        private HashSet<Representative> getRepresentatives(List<string[]> pageRecords, string pathPrefix, StructuredRepresentativeSetting textSetting, string[] nativeRecord)
        {
            HashSet<Representative> reps = new HashSet<Representative>();

            Representative imageRep = getImageRepresentative(pageRecords, pathPrefix);
            Representative textRep = getTextRepresentative(pageRecords, pathPrefix, textSetting);
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