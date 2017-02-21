using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class TextDelimitedBuilder : Builder
    {
        private const char FILE_PATH_DELIM = '\\';
        private const string DEFAULT_CHILD_SEPARATOR = ";";

        public List<Document> BuildDocuments(DocumentSetBuilderArgs e)
        {
            string[] header = GetHeader(e.Records.First(), e.HasHeader);
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            string childSeparator = (String.IsNullOrWhiteSpace(e.ChildColumnDelimiter)) ? DEFAULT_CHILD_SEPARATOR : e.ChildColumnDelimiter;
            // build the documents
            for (int i = 0; i < e.Records.Count; i++)
            {
                string[] record = e.Records[i];
                // check if we need to skip this line
                if (e.HasHeader && i == 0)
                {
                    continue; // skip header line
                }
                // build a document
                DocumentBuilderArgs args = DocumentBuilderArgs.GetTextDelimitedArgs(record, header, e.KeyColumnName, e.RepresentativeColumnInfo, e.PathPrefix);
                Document doc = BuildDocument(args);
                // set the parent and child values
                settleFamilyDrama(e.ParentColumnName, e.ChildColumnName, childSeparator, doc, docs, paternity);
                // add the document to the collection
                docs.Add(doc.Key, doc);
            }
            // check for children that have disowned their parent
            // this can only be known after all children have been imported
            if (paternity.Count > 0)
            {
                string msg = String.Format(
                    "Broken families, {0} children have disowned their parents.\n{1}", 
                    paternity.Count, 
                    String.Join(", ", paternity.Keys.ToList()));
                throw new Exception(msg);
            }
            // return document list
            return docs.Values.ToList();
        }

        public Document BuildDocument(DocumentBuilderArgs e)
        {
            // validate the field count
            if (e.Header.Length != e.DocumentRecord.Length)
                throw new Exception("The document record does not contains the correct number of fields.");
            // setup for building
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            HashSet<Representative> reps = null;
            // populate the metadata
            for (int i = 0; i < e.Header.Length; i++)
            {
                string fieldName = e.Header[i];
                string fieldValue = e.DocumentRecord[i];                
                metadata.Add(fieldName, fieldValue);
            }

            // populate key, if there is no key column name the value in the first column is expected to be the key
            string keyValue = (String.IsNullOrEmpty(e.KeyColumnName))
                ? e.DocumentRecord.First()
                : metadata[e.KeyColumnName];
            // populate representatives
            if (e.RepresentativeColumnInformation != null)
            {
                reps = new HashSet<Representative>();

                foreach (SemiStructuredRepresentativeSetting info in e.RepresentativeColumnInformation)
                {
                    SortedDictionary<string, FileInfo> files = new SortedDictionary<string, FileInfo>();
                    // this format will only have one file per rep
                    string filePath = (String.IsNullOrEmpty(e.PathPrefex))
                        ? metadata[info.RepresentativeColumn]
                        : Path.Combine(e.PathPrefex, metadata[info.RepresentativeColumn].TrimStart(FILE_PATH_DELIM));                    
                    FileInfo file = new FileInfo(filePath);
                    files.Add(keyValue, file);
                    Representative rep = new Representative(info.RepresentativeType, files);
                    reps.Add(rep);
                }
            }
            // default values
            Document parent = null;
            List<Document> children = null;
            return new Document(keyValue, parent, children, metadata, reps);
        }

        public string[] GetHeader(string[] firstRecord, bool hasHeader)
        {
            string[] header = new string[firstRecord.Length];
            // check if the supplied values are the header
            if (hasHeader)
            {
                header = firstRecord;
            }
            else
            {
                // create arbitrary column names
                for (int i = 0; i < firstRecord.Length; i++)
                {
                    header[i] = "Column " + i;
                }
            }

            return header;
        }

        private void settleFamilyDrama(string parentColName, string childColName, string childSeparator, 
            Document doc, Dictionary<string, Document> docs, Dictionary<string, Document> paternity)
        {
            if (!String.IsNullOrEmpty(parentColName))
            {
                setFamilyFromParent(doc, docs, paternity, parentColName, childColName, childSeparator);
            }
            else if (!String.IsNullOrEmpty(childColName))
            {
                setFamilyFromChildren(doc, childColName, childSeparator, paternity);
            }
            else
            {
                // no family data
                // do nothing here
            }
        }

        private void setFamilyFromParent(Document doc, Dictionary<string, Document> docs, Dictionary< string, Document> paternity, 
            string parentColName, string childColName, string childSeparator)
        {
            // if we have a parent column
            string parentKey = doc.Metadata[parentColName];
            // check that the parentKey doesn't refer to itself
            if (String.IsNullOrWhiteSpace(parentKey) || parentKey.Equals(doc.Key))
            {
                // the parentid value refers to itself or there is no parent
                // do nothing here
            }
            else
            {
                Document parent = docs[parentKey];
                // check if there is no parent
                if (parent == null)
                {
                    string msg = String.Format("Broken Families, the parent ({0}) is missing for the document ({1}).", parentKey, doc.Key);
                    throw new Exception(msg);
                }
                else
                {
                    // a parent exists
                    doc.SetParent(parent);
                    // validate relationships if both parent and child fields exists
                    if (!String.IsNullOrWhiteSpace(childColName))
                    {
                        // log paternity so we can check for children who disown their parent
                        string childrenLine = doc.Metadata[childColName];

                        if (String.IsNullOrWhiteSpace(childrenLine))
                        {
                            string[] childKeys = childrenLine.Split(new string[] { childSeparator }, StringSplitOptions.RemoveEmptyEntries);
                            // the child docs haven't been added yet so we'll record the relationships and add them later
                            foreach (string childKey in childKeys)
                            {
                                paternity.Add(childKey, doc); // paternity maps childKey >> parentDoc
                            }
                        }
                        // check for replationships that are not reciprocal
                        if (!parent.Metadata[childColName].Contains(doc.Key))
                        {
                            string msg = String.Format("Broken families, the parent ({0}) disowns a child document ({1})", parentKey, doc.Key);
                            throw new Exception(msg);
                        }
                        else
                        {
                            // the relationship is reciprocal
                            // we'll check for orphans later
                            paternity.Remove(doc.Key);
                        }
                    }
                }
            }
        }

        private void setFamilyFromChildren(Document doc, string childColName, string childSeparator, Dictionary<string, Document> paternity)
        {
            // we don't have a parent column name but we have a child column name
            string childrenLine = doc.Metadata[childColName];

            if (String.IsNullOrEmpty(childrenLine))
            {
                // no child data
                // do nothing here
            }
            else
            {
                string[] childKeys = childrenLine.Split(new string[] { childSeparator }, StringSplitOptions.RemoveEmptyEntries);
                // the child docs haven't been added yet so we'll record the relationship and add them later
                foreach (string childKey in childKeys)
                {
                    paternity.Add(childKey, doc); // paternity maps childKey >> parentDoc
                }
                // now check for the paternity of this document and add the parent
                // paternity maps childKey >> parentDoc
                if (paternity.ContainsKey(doc.Key))
                {
                    Document parent = paternity[doc.Key]; // note: the parent doc has already been confirmed
                    doc.SetParent(parent);
                    paternity.Remove(doc.Key); // needs to be removed for the disowned parent check
                }
            }
        }
    }
}
