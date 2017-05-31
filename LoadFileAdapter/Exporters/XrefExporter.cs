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
    public class XrefExporter : IExporter<IExportXrefSettings>
    {
        /// <summary>
        /// The header line of an XREF file.
        /// </summary>
        public const string HEADER = "CDPath, Prefix, Number, Suffix, GS, GE, Staple, Loose, CS, CE, "
            + "LabelBypass, CustomerData, NamedFolder, NamedFiles";
        private const int GROUP_START_INDEX = 4;
        private const int STAPLE_INDEX = 6;
        private const int CODE_START_INDEX = 8;
        private const int CUSTOMER_DATA_INDEX = 11;
        private const int NAMED_FOLDER_INDEX = 12;
        private const int NAMED_FILES_INDEX = 13;        
        private bool waitingForGroupEnd = false;
        private bool waitingForCodeEnd = false;
        private int boxNumber = 0;
        private DirectoryInfo volumeDirectory = null;
        private DocumentCollection docs = null;

        /// <summary>
        /// Exports a document collection to an XREF.
        /// </summary>
        /// <param name="args">The XREF export settings to use.</param>
        public void Export(IExportXrefSettings args)
        {            
            Dictionary<Type, Action> map = new Dictionary<Type, Action>();
            map.Add(typeof(ExportXrefFileSettings),   () => Export((ExportXrefFileSettings)args));
            map.Add(typeof(ExportXrefWriterSettings), () => Export((ExportXrefWriterSettings)args));
            map[args.GetType()].Invoke();
        }

        /// <summary>
        /// Exports a document collection to an XREF.
        /// </summary>
        /// <param name="args">The XREF export file settings to use.</param>
        public void Export(ExportXrefFileSettings args)
        {
            bool append = false;
            string file = args.GetFile().FullName;
            Encoding encoding = args.GetEncoding();
            DocumentCollection docs = args.GetDocuments();
            XrefTrigger boxBreak = args.GetBoxBreakTrigger();
            XrefTrigger groupStart = args.GetGroupStartTrigger();
            XrefTrigger codeStart = args.GetCodeStartTrigger();
            string customerData = args.GetCustomerData();
            string namedFolder = args.GetNamedFolder();
            string namedFile = args.GetNamedFile();
            XrefSlipSheetSettings slipsheets = args.GetSlipsheets();
            this.volumeDirectory = args.GetFile().Directory;

            using (TextWriter writer = new StreamWriter(file, append, encoding))
            {                
                ExportXrefWriterSettings writerArgs = new ExportXrefWriterSettings(
                    docs, writer, boxBreak, groupStart, codeStart, customerData,
                    namedFolder, namedFile, slipsheets);
                Export(writerArgs);
            }
        }

        /// <summary>
        /// Exports a document collection to an XREF.
        /// </summary>
        /// <param name="args">The XREF export writer settings to use.</param>
        public void Export(ExportXrefWriterSettings args)
        {
            this.docs = args.GetDocuments();
            TextWriter writer = args.GetWriter();          
            XrefSlipSheetSettings ssSettings = args.GetSlipsheets();
            SlipSheets slipsheets = new SlipSheets(docs, ssSettings, this.volumeDirectory);                        
            writer.WriteLine(HEADER);
            // write docs
            for (int i = 0; i < this.docs.Count; i++)
            {                
                List<string> pages = getPageRecords(args, i, slipsheets);
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
        /// <param name="args">The XREF export settings.</param>
        /// <param name="docIndex">The index of the current document in the collection.</param>
        /// <param name="slipsheets">The slipsheets for the document collection .</param>
        /// <returns>Returns all records including slipsheets for the specified document.</returns>
        protected List<string> getPageRecords(IExportXrefSettings args, int docIndex, SlipSheets slipsheets)
        {
            Document document = this.docs[docIndex];
            XrefTrigger boxBreak = args.GetBoxBreakTrigger();
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
                    string[] recordComponents = getRecordComponents(args, docIndex, i);
                    string pageRecord = String.Join(", ", recordComponents);
                                        
                    if (i == 0)
                    {
                        string gs = recordComponents[GROUP_START_INDEX];
                        string cs = recordComponents[CODE_START_INDEX];
                        string custData = recordComponents[CUSTOMER_DATA_INDEX];
                        string namedFolder = recordComponents[NAMED_FOLDER_INDEX];
                        string namedFiles = recordComponents[NAMED_FILES_INDEX];
                        string ssLine = slipsheets.GetSlipSheetXrefLine(
                            document, gs, cs, custData, namedFolder, namedFiles);

                        if (!String.IsNullOrEmpty(ssLine))
                        {
                            pageRecords.Add(ssLine);
                            recordComponents[GROUP_START_INDEX] = "0";
                            recordComponents[CODE_START_INDEX] = "0";

                            if (slipsheets.Settings.IsBindSlipsheets)
                            {
                                recordComponents[STAPLE_INDEX] = "0";
                            }

                            pageRecord = String.Join(", ", recordComponents);
                        }                        
                    }

                    pageRecord = getGhostBoxLine(
                        imageFiles[i].Key, pageRecord, boxBreak, docIndex);
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
        /// <param name="boxTrigger">The box trigger settings.</param>
        /// <param name="docIndex">The index of the current document in the collection being exported.</param>
        /// <returns>Returns the unaltered page record if the trigger type is none. Otherwise the CDPath value
        /// is prepended with a ghost box.</returns>
        protected string getGhostBoxLine(string imageKey, string pageRecord, XrefTrigger boxTrigger, int docIndex)
        {
            if ((boxTrigger.Type != XrefTrigger.TriggerType.None))
            {
                Document doc = this.docs[docIndex];
                Document previousDoc = getPreviousDoc(docIndex);

                if (doc.Key.Equals(imageKey) && isFlagNeeded(doc, boxTrigger, previousDoc))
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
        /// <param name="args">The XREF export settings.</param>
        /// <param name="docIndex">The index of the current document in the collection to export.</param>
        /// <param name="imageIndex">The index of the current image of the current document.</param>
        /// <returns>Returns an array of XREF field values for the specified document.</returns>
        protected string[] getRecordComponents(IExportXrefSettings args, int docIndex, int imageIndex)
        {
            Document document = this.docs[docIndex];
            Document previousDoc = getPreviousDoc(docIndex);
            Representative imageRep = document.Representatives
                .Where(r => r.Type.Equals(Representative.FileType.Image))
                .FirstOrDefault();
            List<KeyValuePair<string, string>> imageFiles = imageRep.Files.ToList();
            string customerDataField = args.GetCustomerData();
            string namedFolderField = args.GetNamedFolder();
            string namedFileField = args.GetNamedFile();
            var image = imageFiles[imageIndex];
            BatesNumber bates = new BatesNumber(image.Key);            
            string CDPath = (!image.Value.Substring(0, 1).Equals("\\"))
                        ? "\\" + image.Value : image.Value;
            string Prefix = bates.Prefix;
            string Number = bates.NumberAsString;
            string Suffix = (bates.HasSuffix) ? bates.SuffixAsString : String.Empty;
            bool GS = getGroupStartFlag(image.Key, docIndex, args);
            if (GS) this.waitingForGroupEnd = true;
            bool GE = getGroupEndFlag(docIndex, imageIndex, args);
            if (GE) this.waitingForGroupEnd = false;
            bool Staple = image.Key.Equals(document.Key);
            string Loose = "0";
            bool CS = getCodeStartFlag(document, previousDoc, args);
            if (CS) this.waitingForCodeEnd = true;
            bool CE = getCodeEndFlag(docIndex, imageIndex, args);
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
        /// Tests if the target document has a field value change event.
        /// </summary>
        /// <param name="doc">The document to test.</param>
        /// <param name="previousDoc">The previous document in the collection being exported.</param>
        /// <param name="trigger">The trigger settings.</param>
        /// <returns>Returns true if there is no change option and the target metadata of the current
        /// document is different from the previous document. Returns true if the change option is 
        /// strip file name and directory name of the current field value is different from
        /// the previous field value. Returns true if the change option is use ending segments
        /// and the specified ending segments of the current doc are different from the ending
        /// segments of the previous doc. Returns true if the change option is starting segments
        /// and the specified starting segments fo the current doc are different from the starting 
        /// segments of the previous doc. Otherwise returns false.</returns>
        internal static bool hasFieldValueChange(Document doc, Document previousDoc, XrefTrigger trigger)
        {
            bool result = false;
            string changeFieldValue = doc.Metadata[trigger.FieldName].ToString();
            string previousFieldValue = (previousDoc != null)
                ? previousDoc.Metadata[trigger.FieldName].ToString()
                : String.Empty;

            switch (trigger.ChangeOption)
            {
                case XrefTrigger.FieldValueChangeOption.None:
                    result = !changeFieldValue.Equals(previousFieldValue);
                    break;
                case XrefTrigger.FieldValueChangeOption.StripFileName:
                    string currentDir = Path.GetDirectoryName(changeFieldValue);
                    string previousDir = Path.GetDirectoryName(previousFieldValue);
                    result = !currentDir.Equals(previousDir);
                    break;
                case XrefTrigger.FieldValueChangeOption.UseEndingSegments:
                    var currentValueEnd = changeFieldValue
                        .Split(new string[] { trigger.SegmentDelimiter }, StringSplitOptions.None)
                        .Reverse().Take(trigger.SegmentCount).Reverse();
                    var previousValueEnd = previousFieldValue
                        .Split(new string[] { trigger.SegmentDelimiter }, StringSplitOptions.None)
                        .Reverse().Take(trigger.SegmentCount).Reverse();
                    result = !currentValueEnd.Equals(previousValueEnd);
                    break;
                case XrefTrigger.FieldValueChangeOption.UseStartingSegments:
                    var currentValueStart = changeFieldValue
                        .Split(new string[] { trigger.SegmentDelimiter }, StringSplitOptions.None)
                        .Take(trigger.SegmentCount);
                    var previousValueStart = previousFieldValue
                        .Split(new string[] { trigger.SegmentDelimiter }, StringSplitOptions.None)
                        .Take(trigger.SegmentCount);
                    result = !currentValueStart.Equals(previousValueStart);
                    break;
                default:
                    // do nothing here
                    break;
            }

            return result;
        }
                
        /// <summary>
        /// Tests if a flag is needed for the supplied document and trigger.
        /// </summary>
        /// <param name="doc">The document to test.</param>
        /// <param name="trigger">The trigger that signals a flag is needed.</param>
        /// <param name="previousDoc">Used to test if a flag is needed if the trigger is a field value change trigger.</param>
        /// <returns>Returns true if the trigger is a family trigger and the doc key matches the parent doc key.
        /// Returns true if the trigger is a regex trigger and target metadata field matches the trigger pattern.
        /// Returns true if the trigger is a field value change trigger and the target metadata field is different 
        /// from the metadata in the previous document. Otherwise it returns false.</returns>
        internal static bool isFlagNeeded(Document doc, XrefTrigger trigger, Document previousDoc)
        {
            bool result = false;
            string docid = doc.Key;
            string parentid = (doc.Parent != null) ? doc.Parent.Key : String.Empty;

            switch (trigger.Type)
            {
                case XrefTrigger.TriggerType.Family:
                    result = (docid.Equals(parentid) || String.IsNullOrEmpty(parentid));
                    break;
                case XrefTrigger.TriggerType.FieldValueChange:
                    result = hasFieldValueChange(doc, previousDoc, trigger);
                    break;
                case XrefTrigger.TriggerType.None:
                    // do nothing here
                    break;
                case XrefTrigger.TriggerType.Regex:
                    result = Regex.IsMatch(doc.Metadata[trigger.FieldName], trigger.RegexPattern);
                    break;
                default:
                    // do nothing here
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the value of the group start flag for the target document and image.
        /// </summary>
        /// <param name="imageKey">The image key of the current record.</param>
        /// <param name="docIndex">The index of the current document in the collection being exported.</param>
        /// <param name="settings">The XREF export settings.</param>
        /// <returns>Returns true if the image key matches the doc key and a flag is needed based on the 
        /// group start trigger.</returns>
        protected bool getGroupStartFlag(string imageKey, int docIndex, IExportXrefSettings settings)
        {
            bool result = false;
            Document doc = this.docs[docIndex];
            Document previousDoc = getPreviousDoc(docIndex);

            if (imageKey.Equals(doc.Key))
            {
                result = isFlagNeeded(doc, settings.GetGroupStartTrigger(), previousDoc);
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
        protected bool getCodeStartFlag(Document doc, Document previousDoc, IExportXrefSettings settings)
        {
            return isFlagNeeded(doc, settings.GetCodeStartTrigger(), previousDoc);
        }

        /// <summary>
        /// Gets the value of the group end flag for the target document and image.
        /// </summary>
        /// <param name="docIndex">The index of the current document in the collection being exported.</param>
        /// <param name="imageIndex">The index of the current image in the current document.</param>
        /// <param name="settings">The XREF export settings.</param>
        /// <returns>Returns true if this is the last document and we are waiting for a group end flag.
        /// Returns true if the next document has a group start flag. Otherwise false is returned.</returns>
        protected bool getGroupEndFlag(int docIndex, int imageIndex, IExportXrefSettings settings)
        {
            bool result = false;
            string nextImageKey = getNextImageKey(imageIndex, docIndex);
            Document doc = this.docs[docIndex];
            Document previousDoc = getPreviousDoc(docIndex);
            Document nextDoc = getNextDoc(docIndex);
            Representative imageRep = doc.Representatives
                .Where(r => r.Type.Equals(Representative.FileType.Image))
                .FirstOrDefault();
            int nextImageDocIndex = (imageRep.Files.ContainsKey(nextImageKey)) ? docIndex : docIndex++;
            
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
                result = getGroupStartFlag(nextImageKey, nextImageDocIndex, settings);
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
        protected bool getCodeEndFlag(int docIndex, int imageIndex, IExportXrefSettings settings)
        {
            bool result = false;
            Document doc = this.docs[docIndex];
            Document previousDoc = getPreviousDoc(docIndex);
            Document nextDoc = getNextDoc(docIndex);
            Representative imageRep = doc.Representatives
                .Where(r => r.Type.Equals(Representative.FileType.Image))
                .FirstOrDefault();
            string nextImageKey = getNextImageKey(imageIndex, docIndex);
            Document nextImageDoc = (imageRep.Files.ContainsKey(nextImageKey))
                ? doc : (String.IsNullOrEmpty(nextImageKey)) ? null : nextDoc;
            Document nextImagePreviousDoc = (imageRep.Files.ContainsKey(nextImageKey))
                ? previousDoc : (String.IsNullOrEmpty(nextImageKey)) ? null : doc;

            if (nextDoc == null && this.waitingForCodeEnd)
            {
                result = true;
            }
            else if (nextDoc == null && !this.waitingForCodeEnd)
            {
                result = true;
            }
            else
            {
                result = getCodeStartFlag(nextImageDoc, nextImagePreviousDoc, settings);
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

            if (doc.Key.Equals(imageKey))
            {
                result = doc.Metadata[customValueField];
            }

            return result;
        }
    }
}
