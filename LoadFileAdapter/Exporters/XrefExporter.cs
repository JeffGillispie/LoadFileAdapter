using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Exports a document collection to an XREF.
    /// </summary>
    public class XrefExporter : IExporter
    {
        /// <summary>
        /// The header line of an XREF file.
        /// </summary>
        public const string HEADER = "CDPath, Prefix, Number, Suffix, GS, GE, Staple, Loose, CS, CE, "
            + "LabelBypass, CustomerData, NamedFolder, NamedFiles";
        protected Switch boxTrigger;
        protected SlipSheets slipsheets;
        protected bool waitingForGroupEnd = false;
        protected bool waitingForCodeEnd = false;
        protected int boxNumber = 0;
        protected DocumentCollection docs;
        private const int GROUP_START_INDEX = 4;
        private const int STAPLE_INDEX = 6;
        private const int CODE_START_INDEX = 8;
        private const int CUSTOMER_DATA_INDEX = 11;
        private const int NAMED_FOLDER_INDEX = 12;
        private const int NAMED_FILES_INDEX = 13;
        private DirectoryInfo volumeDirectory;
        private FileInfo file;
        private Encoding encoding;
        private TextWriter writer;        
        private string customerDataField;
        private string namedFolderField;
        private string namedFileField;
        private Switch groupStartTrigger;
        private Switch codeStartTrigger;
        
        protected XrefExporter()
        {
            // do nothing here
        }

        ~XrefExporter()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }
                       
        /// <summary>
        /// Exports documents to a XREF file.
        /// </summary>
        /// <param name="docs">The documents to export.</param>
        public void Export(DocumentCollection docs)
        {
            this.docs = docs;
            slipsheets.GenerateSlipSheets(docs);                                        
            writer.WriteLine(HEADER);
            // write docs
            for (int i = 0; i < this.docs.Count; i++)
            {                
                List<string> pages = getPageRecords(i, slipsheets);
                // write pages
                foreach (string page in pages)
                {                  
                    writer.WriteLine(page);
                }            
            }
        }

        /// <summary>
        /// Gets all page records for the specified document.
        /// </summary>
        /// <param name="docIndex">The index of the current document in the collection.</param>
        /// <param name="slipsheets">The slipsheets for the document collection .</param>
        /// <returns>Returns all records including slipsheets for the specified document.</returns>
        protected List<string> getPageRecords(int docIndex, SlipSheets slipsheets)
        {
            Document document = this.docs[docIndex];            
            List<string> pageRecords = new List<string>();
            Representative imageRep = document.Representatives
                .Where(r => r.Type.Equals(Representative.FileType.Image)).FirstOrDefault();
            Document previousDoc = getPreviousDoc(docIndex);
            Document nextDoc = getNextDoc(docIndex);            
            // check that a rep was found
            if (imageRep != null)
            {            
                List<KeyValuePair<string, string>> imageFiles = imageRep.Files.ToList();
                // add page records                                
                for (int i = 0; i < imageFiles.Count; i++)
                {
                    string imageKey = imageFiles[i].Key;
                    string[] recordComponents = getRecordComponents(docIndex, i);
                    string pageRecord = String.Join(", ", recordComponents);
                    bool hasSlipsheet = false;
                                        
                    if (i == 0)
                    {
                        string gs = recordComponents[GROUP_START_INDEX];
                        string cs = recordComponents[CODE_START_INDEX];
                        string custData = recordComponents[CUSTOMER_DATA_INDEX];
                        string namedFolder = recordComponents[NAMED_FOLDER_INDEX];
                        string namedFiles = recordComponents[NAMED_FILES_INDEX];
                        string ssLine = slipsheets.GetSlipSheetXrefLine(
                            document, gs, cs, custData, namedFolder, namedFiles);
                        hasSlipsheet = !String.IsNullOrEmpty(ssLine);
                        
                        if (hasSlipsheet)
                        {
                            ssLine = getGhostBoxLine(imageKey, ssLine, docIndex, false);
                            pageRecords.Add(ssLine);
                            recordComponents[GROUP_START_INDEX] = "0";
                            recordComponents[CODE_START_INDEX] = "0";

                            if (slipsheets.IsBindSlipsheets)
                            {
                                recordComponents[STAPLE_INDEX] = "0";
                            }

                            pageRecord = String.Join(", ", recordComponents);
                        }                        
                    }

                    pageRecord = getGhostBoxLine(imageKey, pageRecord, docIndex, hasSlipsheet);
                    pageRecords.Add(pageRecord);
                }
            }
                        
            return pageRecords;
        }

        /// <summary>
        /// Takes an XREF record and prepends a ghost box for the specified document if one is triggered.
        /// The box number is incremented each time the box trigger occurs.
        /// </summary>
        /// <param name="imageKey">The imagekey of the current record.</param>
        /// <param name="pageRecord">The XREF record to ghost box.</param>        
        /// <param name="docIndex">The index of the current document in the collection being exported.</param>
        /// <param name="hasSlipsheet">Indicates if the document has a slipsheet and prevents the box number
        /// from being incremented.</param>
        /// <returns>Returns the unaltered page record if the trigger type is none. Otherwise the CDPath value
        /// is prepended with a ghost box.</returns>
        protected string getGhostBoxLine(string imageKey, string pageRecord, int docIndex, bool hasSlipsheet)
        {
            if (boxTrigger != null && boxTrigger.Type != Switch.SwitchType.None)
            {
                Document doc = this.docs[docIndex];
                Document previousDoc = getPreviousDoc(docIndex);

                if (doc.Key.Equals(imageKey) && !hasSlipsheet && boxTrigger.IsTriggered(doc, previousDoc))                    
                {
                    this.boxNumber++;
                }

                string box = String.Format(@"\Box{0}\..", this.boxNumber.ToString().PadLeft(3, '0')); ;

                pageRecord = box + pageRecord;
            }

            return pageRecord;
        }

        /// <summary>
        /// Gets all the components to make a single XREF record for the specified document.
        /// </summary>        
        /// <param name="docIndex">The index of the current document in the collection to export.</param>
        /// <param name="imageIndex">The index of the current image of the current document.</param>
        /// <returns>Returns an array of XREF field values for the specified document.</returns>
        protected string[] getRecordComponents(int docIndex, int imageIndex)
        {
            Document document = this.docs[docIndex];
            Document previousDoc = getPreviousDoc(docIndex);
            Representative imageRep = document.Representatives
                .Where(r => r.Type.Equals(Representative.FileType.Image))
                .FirstOrDefault();
            List<KeyValuePair<string, string>> imageFiles = imageRep.Files.ToList();            
            var image = imageFiles[imageIndex];
            BatesNumber bates = new BatesNumber(image.Key);            
            string CDPath = image.Value;
            string Prefix = bates.Prefix;
            string Number = bates.NumberAsString;
            string Suffix = (bates.HasSuffix) ? bates.SuffixAsString : String.Empty;
            bool GS = getGroupStartFlag(image.Key, docIndex, groupStartTrigger);
            if (GS) this.waitingForGroupEnd = true;
            bool GE = getGroupEndFlag(docIndex, imageIndex, groupStartTrigger);
            if (GE) this.waitingForGroupEnd = false;
            bool Staple = image.Key.Equals(document.Key);
            string Loose = "0";
            bool CS = getCodeStartFlag(document, previousDoc, codeStartTrigger);
            if (CS) this.waitingForCodeEnd = true;
            bool CE = getCodeEndFlag(docIndex, imageIndex, codeStartTrigger);
            if (CE) this.waitingForCodeEnd = false;
            string LabelBypass = "0";
            string CustomerData = getCustomValue(image.Key, document, customerDataField);
            string NamedFolder = getCustomValue(image.Key, document, namedFolderField);
            string NamedFiles = getCustomValue(image.Key, document, namedFileField);
            string[] recordComponents = new string[] { CDPath, Prefix, Number, Suffix,
                        boolToString(GS), boolToString(GE), boolToString(Staple), Loose,
                        boolToString(CS), boolToString(CE), LabelBypass,
                        CustomerData, NamedFolder, NamedFiles };
            return recordComponents;
        }
        
        /// <summary>
        /// Gets the next image key in the document collection being exported.
        /// </summary>
        /// <param name="imageIndex">The index of the current image in the current document.</param>
        /// <param name="docIndex">The index of the current document in the collection.</param>
        /// <returns>Returns the next image key in the current document if one exists or the
        /// first image key in the next document, or null if this is the last image in the 
        /// last document.</returns>
        protected string getNextImageKey(int imageIndex, int docIndex)
        {
            string nextImageKey = null;
            Document doc = this.docs[docIndex];
            Representative imageRep = doc.Representatives
                .Where(r => r.Type.Equals(Representative.FileType.Image))
                .FirstOrDefault();
            List<KeyValuePair<string, string>> imageFiles = imageRep.Files.ToList();

            if (imageIndex < imageFiles.Count - 1)
            {
                nextImageKey = imageFiles[imageIndex + 1].Key;
            }
            else if (docIndex < this.docs.Count - 1)
            {
                Representative nextImageRep = docs[docIndex + 1].Representatives
                    .Where(r => (r.Type == Representative.FileType.Image)).FirstOrDefault();
                nextImageKey = nextImageRep.Files.FirstOrDefault().Key;
            }

            return nextImageKey;
        }

        /// <summary>
        /// Converts a bool value to an expected XREF bool value.
        /// </summary>
        /// <param name="b">The value to convert.</param>
        /// <returns>Returns '1' if true or '0' if false.</returns>
        protected static string boolToString(bool b)
        {
            return b ? "1" : "0";
        }

        protected Document getPreviousDoc(int currentIndex)
        {
            Document previousDoc = null;

            if (currentIndex > 0)
            {
                previousDoc = this.docs[currentIndex - 1];
            }

            return previousDoc;
        }

        /// <summary>
        /// Gets the next document in the collection being exported.
        /// </summary>
        /// <param name="currentIndex">The index of the current document in the collection.</param>
        /// <returns>Returns the next document in the collection.</returns>
        protected Document getNextDoc(int currentIndex)
        {
            Document nextDoc = null;

            if (currentIndex < this.docs.Count - 1)
            {
                nextDoc = this.docs[currentIndex + 1];
            }

            return nextDoc;
        }
                
        /// <summary>
        /// Gets the value of the group start flag for the target document and image.
        /// </summary>
        /// <param name="imageKey">The image key of the current record.</param>
        /// <param name="docIndex">The index of the current document in the collection being exported.</param>
        /// <param name="settings">The XREF export settings.</param>
        /// <returns>Returns true if the image key matches the doc key and a flag is needed based on the 
        /// group start trigger.</returns>
        protected bool getGroupStartFlag(string imageKey, int docIndex, Switch trigger)
        {
            bool result = false;
            Document doc = this.docs[docIndex];
            Document previousDoc = getPreviousDoc(docIndex);

            if (imageKey.Equals(doc.Key) && trigger != null)
            {
                result = trigger.IsTriggered(doc, previousDoc);
            }

            return result;
        }

        /// <summary>
        /// Gets the value of the code start flag for the target document.
        /// </summary>
        /// <param name="doc">The document to test for a code start.</param>
        /// <param name="previousDoc">The previous document in the collection being exported.</param>
        /// <param name="settings">The XREF export settings.</param>
        /// <returns>Returns true if a flag is needed based on the code start trigger settings.</returns>
        protected bool getCodeStartFlag(Document doc, Document previousDoc, Switch trigger)
        {
            bool result = false;

            if (trigger != null)
            {
                result = trigger.IsTriggered(doc, previousDoc);
            }

            return result; 
        }

        /// <summary>
        /// Gets the value of the group end flag for the target document and image.
        /// </summary>
        /// <param name="docIndex">The index of the current document in the collection being exported.</param>
        /// <param name="imageIndex">The index of the current image in the current document.</param>
        /// <param name="settings">The XREF export settings.</param>
        /// <returns>Returns true if this is the last document and we are waiting for a group end flag.
        /// Returns true if the next document has a group start flag. Otherwise false is returned.</returns>
        protected bool getGroupEndFlag(int docIndex, int imageIndex, Switch trigger)
        {
            bool result = false;
            string nextImageKey = getNextImageKey(imageIndex, docIndex);
            Document doc = this.docs[docIndex];
            Document previousDoc = getPreviousDoc(docIndex);
            Document nextDoc = getNextDoc(docIndex);
            Representative imageRep = doc.Representatives
                .Where(r => r.Type.Equals(Representative.FileType.Image))
                .FirstOrDefault();
            int nextImageDocIndex = (nextImageKey != null && imageRep.Files.ContainsKey(nextImageKey)) 
                ? docIndex : docIndex++;
            
            if (nextDoc == null && this.waitingForGroupEnd)
            {
                result = true;
            }
            else if (nextDoc == null && !this.waitingForGroupEnd)
            {
                result = false;
            }
            else
            {
                result = getGroupStartFlag(nextImageKey, nextImageDocIndex, trigger);
            }

            return result;
        }

        /// <summary>
        /// Gets the value of the code end flag for the target document and image.
        /// </summary>
        /// <param name="docIndex">The index of the current document in the collection being exported.</param>
        /// <param name="imageIndex">The index of the current image in the current document.</param>
        /// <param name="settings">The XREF export settings.</param>
        /// <returns>Returns true if this is the last document and we are waiting for a code end flag.
        /// Returns true if the next document will have a code start flag. Otherwise false is returned.</returns>
        protected bool getCodeEndFlag(int docIndex, int imageIndex, Switch trigger)
        {
            bool result = false;
            Document doc = this.docs[docIndex];
            Document previousDoc = getPreviousDoc(docIndex);
            Document nextDoc = getNextDoc(docIndex);
            Representative imageRep = doc.Representatives
                .Where(r => r.Type.Equals(Representative.FileType.Image))
                .FirstOrDefault();
            string nextImageKey = getNextImageKey(imageIndex, docIndex);            
            Document nextImageDoc = (String.IsNullOrEmpty(nextImageKey))
                ? null : (imageRep.Files.ContainsKey(nextImageKey)) ? doc : nextDoc;
            Document nextImagePreviousDoc = (String.IsNullOrEmpty(nextImageKey))
                ? null : (imageRep.Files.ContainsKey(nextImageKey)) ? previousDoc : doc;

            if (nextDoc == null && this.waitingForCodeEnd)
            {
                result = true;
            }
            else if (nextDoc == null && !this.waitingForCodeEnd)
            {
                result = false;
            }
            else
            {
                result = getCodeStartFlag(nextImageDoc, nextImagePreviousDoc, trigger);
            }

            return result;
        }

        /// <summary>
        /// Retrieves the appropriate value for the corresponding image key and custom field.
        /// </summary>
        /// <param name="imageKey">The image key of the current record.</param>
        /// <param name="doc">The document the image key belongs to.</param>
        /// <param name="customValueField">The metadata field containing the target value.</param>
        /// <returns>Returns an empty string if the image key doesn't match the document key.
        /// Otherwise it returns the value in the target metadata field.</returns>
        protected string getCustomValue(string imageKey, Document doc, string customValueField)
        {
            string result = String.Empty;

            if (doc.Key.Equals(imageKey) && 
                !String.IsNullOrEmpty(customValueField) && 
                doc.Metadata.ContainsKey(customValueField))
            {
                result = doc.Metadata[customValueField];
            }

            return result;
        }

        /// <summary>
        /// Builds a <see cref="XrefExporter"/> instance.
        /// </summary>
        public class Builder
        {            
            private XrefExporter instance;

            private Builder()
            {
                instance = new XrefExporter();
            }

            /// <summary>
            /// Starts the process of building a <see cref="XrefExporter"/>.
            /// </summary>
            /// <param name="file">The destination XREF file.</param>
            /// <param name="encoding">The encoding of the export file.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public static Builder Start(FileInfo file, Encoding encoding)
            {
                Builder builder = new Builder();
                bool append = false;
                builder.instance.file = file;
                builder.instance.encoding = encoding;
                builder.instance.writer = new StreamWriter(file.FullName, append, encoding);
                builder.instance.volumeDirectory = file.Directory;
                builder.instance.CreateDestination(file);
                return builder;
            }

            /// <summary>
            /// Starts the process of building a <see cref="XrefExporter"/>.
            /// </summary>
            /// <param name="writer">The <see cref="TextWriter"/> used to write the export.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public static Builder Start(TextWriter writer)
            {
                Builder builder = new Builder();
                builder.instance.writer = writer;
                return builder;
            }

            /// <summary>
            /// Sets the customer data field.
            /// </summary>
            /// <param name="value">Metadata field name of the field containing customer data values.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetCustomerDataField(string value)
            {
                instance.customerDataField = value;
                return this;
            }

            /// <summary>
            /// Sets the named folder field.
            /// </summary>
            /// <param name="value">Metadata field name of the field containing named folder values.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetNamedFolderField(string value)
            {
                instance.namedFolderField = value;
                return this;
            }

            /// <summary>
            /// Sets the named file field.
            /// </summary>
            /// <param name="value">Metadata field name of the field containing named file values.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetNamedFileField(string value)
            {
                instance.namedFileField = value;
                return this;
            }

            /// <summary>
            /// Sets the group start trigger.
            /// </summary>
            /// <param name="value">The trigger that activates a group start.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetGroupStartTrigger(Switch value)
            {
                instance.groupStartTrigger = value;
                return this;
            }

            /// <summary>
            /// Sets the code start trigger.
            /// </summary>
            /// <param name="value">The trigger that activates a code start.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetCodeStartTrigger(Switch value)
            {
                instance.codeStartTrigger = value;
                return this;
            }

            /// <summary>
            /// Sets the box trigger.
            /// </summary>
            /// <param name="value">The trigger that causes the use of ghost boxing 
            /// and increments the box number.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetBoxTrigger(Switch value)
            {
                instance.boxTrigger = value;
                return this;
            }

            /// <summary>
            /// Sets the <see cref="SlipSheets"/> object that controls the creation of 
            /// slipsheets in the export.
            /// </summary>
            /// <param name="value">The slipsheets value.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetSlipsheets(SlipSheets value)
            {
                instance.slipsheets = value;
                return this;
            }

            /// <summary>
            /// Builds a <see cref="XrefExporter"/>.
            /// </summary>
            /// <returns>Returns a <see cref="XrefExporter"/>.</returns>
            public XrefExporter Build()
            {
                XrefExporter instance = this.instance;
                this.instance = null;
                return instance;
            }
        }
    }
}
