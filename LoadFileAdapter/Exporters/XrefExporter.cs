using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoadFileAdapter.Exporters
{
    //todo: comments
    public class XrefExporter : IExporter<IExportXrefSettings>
    {
        public const string HEADER = "CDPath, Prefix, Number, Suffix, GS, GE, Staple, Loose, CS, CE, "
            + "LabelBypass, CustomerData, NamedFolder, NamedFiles";
        public void Export(IExportXrefSettings args)
        {
            Dictionary<Type, Action> map = new Dictionary<Type, Action>();
            map.Add(typeof(ExportXrefFileSettings), () => Export((ExportXrefFileSettings)args));
            map.Add(typeof(ExportXrefWriterSettings), () => Export((ExportXrefWriterSettings)args));
            map[args.GetType()].Invoke();
        }

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

            using (TextWriter writer = new StreamWriter(file, append, encoding))
            {
                ExportXrefWriterSettings writerArgs = new ExportXrefWriterSettings(
                    docs, writer, boxBreak, groupStart, codeStart, customerData,
                    namedFolder, namedFile, slipsheets);
                Export(writerArgs);
            }
        }

        public void Export(ExportXrefWriterSettings args)
        {
            DocumentCollection docs = args.GetDocuments();
            TextWriter writer = args.GetWriter();
            XrefTrigger boxBreak = args.GetBoxBreakTrigger();
            XrefTrigger groupStart = args.GetGroupStartTrigger();
            XrefTrigger codeStart = args.GetCodeStartTrigger();
            string customerData = args.GetCustomerData();
            string namedFolder = args.GetNamedFolder();
            string namedFile = args.GetNamedFile();
            XrefSlipSheetSettings slipsheets = args.GetSlipsheets();
            bool hasBoxing = (boxBreak.Type != XrefTrigger.TriggerType.None);
            bool waitingForGroupEnd = false;
            bool waitingForCodeEnd = false;
            int docIndex = 0;
            int boxNumber = 0;
            writer.WriteLine(HEADER);

            foreach (Document document in docs)
            {
                List<string> pages = getPageRecords(document, args, hasBoxing,
                    ref waitingForGroupEnd, ref waitingForCodeEnd, docIndex, 
                    ref boxNumber);
                // write pages
                foreach (string page in pages)
                {
                    //todo: check for and write slipsheet line                    
                    writer.WriteLine(page);
                }

                docIndex++;
            }
        }

        protected List<string> getPageRecords(Document document, IExportXrefSettings args,
            bool hasBoxing, ref bool waitingForGroupEnd, ref bool waitingForCodeEnd, int docIndex, 
            ref int boxNumber)
        {
            DocumentCollection docs = args.GetDocuments();
            XrefTrigger boxBreak = args.GetBoxBreakTrigger();
            List<string> pageRecords = new List<string>();
            Representative imageRep = null;
            Document previousDoc = getPreviousDoc(docs, docIndex);
            Document nextDoc = getNextDoc(docs, docIndex);
            // find the image representative
            foreach (Representative rep in document.Representatives)
            {
                if (rep.Type.Equals(Representative.FileType.Image))
                {
                    imageRep = rep;
                    break;
                }
            }
            // check that a rep was found
            if (imageRep == null)
            {
                // do nothing here
            }
            else
            {
                List<KeyValuePair<string, string>> imageFiles = imageRep.Files.ToList();
                // add page records                                
                for (int i = 0; i < imageFiles.Count; i++)
                {
                    string[] recordComponents = getRecordComponents(document, docs, 
                        previousDoc, nextDoc, imageRep, args, imageFiles, docIndex, i,
                        ref waitingForGroupEnd, ref waitingForCodeEnd);
                    string pageRecord = String.Join(", ", recordComponents);
                    // ghost boxing
                    if (hasBoxing)
                    {
                        string box = getGhostBox(
                            imageFiles[i].Key, document, boxBreak, previousDoc, ref boxNumber);
                        pageRecord = box + pageRecord;
                    }

                    pageRecords.Add(pageRecord);
                }
            }

            return pageRecords;
        }

        protected string[] getRecordComponents(Document document, DocumentCollection docs,
            Document previousDoc, Document nextDoc, Representative imageRep, IExportXrefSettings args,
            List<KeyValuePair<string, string>> imageFiles, int docIndex, int imageIndex,
            ref bool waitingForGroupEnd, ref bool waitingForCodeEnd)
        {
            string customerDataField = args.GetCustomerData();
            string namedFolderField = args.GetNamedFolder();
            string namedFileField = args.GetNamedFile();
            var image = imageFiles[imageIndex];
            BatesNumber bates = new BatesNumber(image.Key);
            string nextImageKey = getNextImageKey(imageFiles, docs, imageIndex, docIndex);
            Document nextImageDoc = (imageRep.Files.ContainsKey(nextImageKey))
                ? document : (String.IsNullOrEmpty(nextImageKey)) ? null : nextDoc;
            Document nextImagePreviousDoc = (imageRep.Files.ContainsKey(nextImageKey))
                ? previousDoc : (String.IsNullOrEmpty(nextImageKey)) ? null : document;
            string CDPath = (!image.Value.Substring(0, 1).Equals("\\"))
                        ? "\\" + image.Value : image.Value;
            string Prefix = bates.Prefix;
            string Number = bates.NumberAsString;
            string Suffix = (bates.HasSuffix) ? bates.SuffixAsString : String.Empty;
            bool GS = getGroupStartFlag(image.Key, document, previousDoc, args);
            if (GS) waitingForGroupEnd = true;
            bool GE = getGroupEndFlag(nextImageKey, nextImageDoc, waitingForGroupEnd,
                nextImagePreviousDoc, nextDoc, args);
            if (GE) waitingForGroupEnd = false;
            bool Staple = image.Key.Equals(document.Key);
            string Loose = "0";
            bool CS = getCodeStartFlag(document, previousDoc, args);
            if (CS) waitingForCodeEnd = true;
            bool CE = getCodeEndFlag(nextImageKey, nextImageDoc, waitingForCodeEnd,
                nextImagePreviousDoc, nextDoc, args);
            if (CE) waitingForCodeEnd = false;
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

        protected string getGhostBox(string imageKey, Document doc, XrefTrigger boxTrigger, 
            Document previousDoc, ref int boxNum)
        {
            if (doc.Key.Equals(imageKey) && isFlagNeeded(doc, boxTrigger, previousDoc))
            {                                
                boxNum++;                
            }            

            return String.Format(@"\Box{0}\..", boxNum.ToString().PadLeft(3, '0')); ;
        }

        protected string getNextImageKey(List<KeyValuePair<string, string>> imageFiles, DocumentCollection docs, 
            int imageIndex, int docIndex)
        {
            string nextImageKey = null;

            if (imageIndex < imageFiles.Count - 1)
            {
                nextImageKey = imageFiles[imageIndex + 1].Key;
            }
            else if (docIndex < docs.Count - 1)
            {
                Representative nextImageRep = docs[docIndex + 1].Representatives
                    .Where(r => (r.Type == Representative.FileType.Image)).First();
                nextImageKey = nextImageRep.Files.First().Key;
            }

            return nextImageKey;
        }

        protected string boolToString(bool b)
        {
            return b ? "1" : "0";
        }

        protected Document getPreviousDoc(DocumentCollection docs, int currentIndex)
        {
            Document previousDoc = null;

            if (currentIndex > 0)
            {
                previousDoc = docs[currentIndex - 1];
            }

            return previousDoc;
        }

        protected Document getNextDoc(DocumentCollection docs, int currentIndex)
        {
            Document nextDoc = null;

            if (currentIndex < docs.Count - 1)
            {
                nextDoc = docs[currentIndex + 1];
            }

            return nextDoc;
        }
                
        protected bool isFlagNeeded(Document doc, XrefTrigger trigger, Document previousDoc)
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
                    #region Field Value Change
                    string changeFieldValue = doc.Metadata[trigger.FieldValueChangeEventField].ToString();
                    string previousFieldValue = (previousDoc != null)
                        ? previousDoc.Metadata[trigger.FieldValueChangeEventField].ToString()
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
                                .Split(new string[] { trigger.EndingSegmentDelimiter }, StringSplitOptions.None)
                                .Reverse().Take(trigger.EndingSegmentCount).Reverse();
                            var previousValueEnd = previousFieldValue
                                .Split(new string[] { trigger.EndingSegmentDelimiter }, StringSplitOptions.None)
                                .Reverse().Take(trigger.EndingSegmentCount).Reverse();
                            result = !currentValueEnd.Equals(previousValueEnd);
                            break;
                        case XrefTrigger.FieldValueChangeOption.UseStartingSegments:
                            var currentValueStart = changeFieldValue
                                .Split(new string[] { trigger.StartingSegmentDelimiter }, StringSplitOptions.None)
                                .Reverse().Take(trigger.StartingSegmentCount).Reverse();
                            var previousValueStart = previousFieldValue
                                .Split(new string[] { trigger.StartingSegmentDelimiter }, StringSplitOptions.None)
                                .Reverse().Take(trigger.StartingSegmentCount).Reverse();
                            result = !currentValueStart.Equals(previousValueStart);
                            break;
                        default:
                            // do nothing here
                            break;
                    }
                    #endregion
                    break;
                case XrefTrigger.TriggerType.None:
                    // do nothing here
                    break;
                case XrefTrigger.TriggerType.Regex:
                    result = Regex.IsMatch(doc.Metadata[trigger.RegexField], trigger.RegexPattern);
                    break;
                default:
                    // do nothing here
                    break;
            }

            return result;
        }

        protected bool getGroupStartFlag(string imageKey, Document doc, Document previousDoc, 
            IExportXrefSettings settings)
        {
            bool result = false;

            if (imageKey.Equals(doc.Key))
            {
                result = isFlagNeeded(doc, settings.GetGroupStartTrigger(), previousDoc);
            }

            return result;
        }

        protected bool getCodeStartFlag(Document doc, Document previousDoc, IExportXrefSettings settings)
        {
            return isFlagNeeded(doc, settings.GetCodeStartTrigger(), previousDoc);
        }

        protected bool getGroupEndFlag(string nextImageKey, Document nextImageDoc, bool waitingForEnd,
            Document nextImagePreviousDoc, Document nextDoc, IExportXrefSettings settings)
        {
            bool result = false;

            if (nextDoc == null && waitingForEnd)
            {
                result = true;
            }
            else if (nextDoc == null && !waitingForEnd)
            {
                result = false;
            }
            else
            {
                result = getGroupStartFlag(nextImageKey, nextImageDoc, nextImagePreviousDoc, settings);
            }

            return result;
        }

        protected bool getCodeEndFlag(string nextImageKey, Document nextImageDoc, bool waitingForEnd,
            Document nextImagePreviousDoc, Document nextDoc, IExportXrefSettings settings)
        {
            bool result = false;

            if (nextDoc == null && waitingForEnd)
            {
                result = true;
            }
            else if (nextDoc == null && !waitingForEnd)
            {
                result = true;
            }
            else
            {
                result = getCodeStartFlag(nextImageDoc, nextImagePreviousDoc, settings);
            }

            return result;
        }

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
