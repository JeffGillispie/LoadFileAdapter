using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// A builder that builds documents from a DAT file.
    /// </summary>
    public class DatBuilder : IBuilder
    {        
        private const string DEFAULT_CHILD_SEPARATOR = ";";
        private string[] header;
        private string keyColumnName;
        private string parentColumnName;
        private string childColumnName;
        private string childSeparator;
        private string pathPrefix;
        private IEnumerable<RepresentativeBuilder> repBuilders;
        private bool hasHeader = true;

        /// <summary>
        /// Gets or sets the column name of the field that holds the document key.
        /// </summary>
        public string KeyColumnName
        {
            get
            {
                return this.keyColumnName;
            }

            set
            {
                this.keyColumnName = value;
            }
        }

        /// <summary>
        /// Gets or sets the column name of the field that holds the parent key value.
        /// </summary>
        public string ParentColumnName
        {
            get
            {
                return this.parentColumnName;
            }

            set
            {
                this.parentColumnName = value;
            }
        }

        /// <summary>
        /// Gets or sets the column name of the field that holds a delimited list
        /// of child keys.
        /// </summary>
        public string ChildColumnName
        {
            get
            {
                return this.childColumnName;
            }

            set
            {
                this.childColumnName = value;
            }
        }

        /// <summary>
        /// Gets or sets the separator of the child key values in the children field.
        /// </summary>
        public string ChildSeparator
        {
            get
            {
                return this.childSeparator;
            }

            set
            {
                this.childSeparator = value;
            }
        }

        /// <summary>
        /// Gets or sets the path prefix to be prepended to the file path of
        /// <see cref="Representative"/> files.
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
        /// Gets or sets the collection or <see cref="RepresentativeBuilder"/> objects.
        /// Which make <see cref="Representative"/> objects for each <see cref="Document"/>.
        /// </summary>
        public IEnumerable<RepresentativeBuilder> RepresentativeBuilders
        {
            get
            {
                return this.repBuilders;
            }

            set
            {
                this.repBuilders = value;
            }
        }

        /// <summary>
        /// Gets or sets an indicator that he first record is a header.
        /// </summary>
        public bool HasHeader
        {
            get
            {
                return this.hasHeader;
            }

            set
            {
                this.hasHeader = value;
            }
        }

        /// <summary>
        /// Sets the header value.
        /// </summary>
        /// <param name="header"></param>
        protected void SetHeader(string[] header)
        {
            this.header = header;
        }

        /// <summary>
        /// Builds documents from a DAT file.
        /// </summary>
        /// <param name="records">The parsed lines of a DAT file.</param>
        /// <returns>Returns a list of <see cref="Document"/>.</returns>
        public List<Document> Build(IEnumerable<string[]> records)
        {            
            this.header = GetHeader(records.First(), HasHeader);
            Dictionary<string, Document> docs = new Dictionary<string, Document>();
            Dictionary<string, Document> paternity = new Dictionary<string, Document>();
            childSeparator = (String.IsNullOrWhiteSpace(childSeparator)) 
                ? DEFAULT_CHILD_SEPARATOR 
                : childSeparator;
            // build the documents
            int i = -1;
            foreach (string[] record in records)
            {
                i++;
                // check if we need to skip this line
                if (HasHeader && i == 0)
                {
                    continue; // skip header line
                }
                // build a document                
                Document doc = BuildDocument(record);
                // set the parent and child values
                settleFamilyDrama(doc, docs, paternity);
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

        /// <summary>
        /// Builds a <see cref="Document"/> from a DAT file.
        /// </summary>
        /// <param name="docRecords">The document record.</param>
        /// <returns>Returns a <see cref="Document"/>.</returns>
        public Document BuildDocument(IEnumerable<string[]> docRecords)
        {
            if (docRecords.Count() != 1)
            {
                throw new Exception("Unexpected record value.");
            }

            string[] fields = docRecords.First();
            return BuildDocument(fields);
        }

        /// <summary>
        /// Builds a <see cref="Document"/> from a DAT file.
        /// </summary>
        /// <param name="fields">The parsed metadata values.</param>
        /// <returns>Returns a <see cref="Document"/>.</returns>
        public Document BuildDocument(string[] fields)
        {            
            // validate the field count
            if (header.Length != fields.Length)
                throw new Exception("The document record does not contain the correct number of fields.");
            // setup for building
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            HashSet<Representative> reps = new HashSet<Representative>();
            // populate the metadata
            for (int i = 0; i < header.Length; i++)
            {
                string fieldName = header[i];
                string fieldValue = fields[i];                
                metadata.Add(fieldName, fieldValue);
            }

            // populate key, if there is no key column name 
            // the value in the first column is expected to be the key
            string keyValue = (String.IsNullOrEmpty(keyColumnName))
                ? fields.First()
                : metadata[keyColumnName];
            // populate representatives
            if (repBuilders != null)
            {
                foreach (RepresentativeBuilder builder in repBuilders)
                {
                    var rep = builder.Build(keyValue, metadata, pathPrefix);

                    if (rep != null)
                    {
                        reps.Add(rep);
                    }
                }
            }
            // default values
            Document parent = null;
            HashSet<Document> children = null;
            return new Document(keyValue, parent, children, metadata, reps);
        }

        /// <summary>
        /// Gets the header value.
        /// </summary>
        /// <param name="firstRecord">The first record from a DAT file.</param>
        /// <param name="hasHeader">Indicates if the first record is a header.</param>
        /// <returns>Returns a field list or a list of numbered columns.</returns>
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
                    header[i] = "Column " + (i + 1);
                }
            }

            return header;
        }

        /// <summary>
        /// Sets the parent and children fields of a document.
        /// </summary>        
        /// <param name="doc">Sets family relationships on this document.</param>
        /// <param name="docs">A map of keys to documents.</param>
        /// <param name="paternity">A map of child keys to parent documents.</param>
        protected void settleFamilyDrama(
            Document doc, Dictionary<string, Document> docs, Dictionary<string, Document> paternity)
        {
            if (!String.IsNullOrEmpty(parentColumnName))
            {
                setFamilyFromParent(doc, docs, paternity);
            }
            else if (!String.IsNullOrEmpty(childColumnName))
            {
                setFamilyFromChildren(doc, paternity);
            }
            else
            {
                // no family data
                // do nothing here
            }
        }

        /// <summary>
        /// Sets the parent and child values of a document based on a parent key.
        /// </summary>
        /// <param name="doc">A <see cref="Document"/></param>
        /// <param name="docs">A map of keys to a document.</param>
        /// <param name="paternity">A map of child keys to parent documents.</param>        
        protected void setFamilyFromParent(
            Document doc, Dictionary<string, Document> docs, Dictionary<string, Document> paternity)
        {
            string parentKey = String.Empty;

            if (parentColumnName != null && doc.Metadata.ContainsKey(parentColumnName))
            {
                parentKey = doc.Metadata[parentColumnName];
            }
            // check that the parentKey doesn't refer to itself
            if (String.IsNullOrWhiteSpace(parentKey) || parentKey.Equals(doc.Key))
            {
                // the parentid value refers to itself or there is no parent
                // do nothing here
            }
            else
            {
                Document parent = null;

                try
                {
                    parent = docs[parentKey];
                }
                catch (KeyNotFoundException )
                {
                    string msg = String.Format(
                        "Broken Families, the parent ({0}) is missing for the document ({1}).",
                        parentKey, doc.Key);
                    throw new Exception(msg);
                }
                                
                // a parent exists
                doc.SetParent(parent);
                // validate relationships if both parent and child fields exists
                if (!String.IsNullOrWhiteSpace(childColumnName))
                {                    
                    // check for replationships that are not reciprocal
                    if (!parent.Metadata[childColumnName].Contains(doc.Key))
                    {
                        string msg = String.Format(
                            "Broken families, the parent ({0}) disowns a child document ({1}).", 
                            parentKey, doc.Key);
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
            // log paternity so we can check for children who disown their parent
            string childrenLine = String.Empty;

            if (childColumnName != null && doc.Metadata.ContainsKey(childColumnName))
            {
                childrenLine = doc.Metadata[childColumnName];
            }

            if (!String.IsNullOrWhiteSpace(childrenLine))
            {
                string[] childKeys = childrenLine.Split(
                    new string[] { childSeparator },
                    StringSplitOptions.RemoveEmptyEntries);
                // the child docs haven't been added yet so we'll record the relationships and add them later
                foreach (string childKey in childKeys)
                {
                    paternity.Add(childKey, doc); // paternity maps childKey >> parentDoc
                }
            }
        }

        /// <summary>
        /// Sets the parent and child values of a document based on a delimited list of child keys.
        /// </summary>
        /// <param name="doc">A <see cref="Document"/></param>        
        /// <param name="paternity">A map of child keys to parent documents.</param>
        protected void setFamilyFromChildren(Document doc, Dictionary<string, Document> paternity)
        {
            // we don't have a parent column name but we have a child column name
            string childrenLine = string.Empty;

            if (doc.Metadata.ContainsKey(childColumnName))
            {
                childrenLine = doc.Metadata[childColumnName];
            }

            if (String.IsNullOrEmpty(childrenLine))
            {
                // no child data
                // do nothing here
            }
            else
            {
                string[] childKeys = childrenLine.Split(
                    new string[] { childSeparator }, 
                    StringSplitOptions.RemoveEmptyEntries);
                // the child docs haven't been added yet so we'll record the relationship and add them later
                foreach (string childKey in childKeys)
                {
                    paternity.Add(childKey, doc); // paternity maps childKey >> parentDoc
                }                
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