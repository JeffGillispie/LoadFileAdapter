using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// A collection of slipsheets.
    /// </summary>
    public class SlipSheets
    {
        private Dictionary<string, SlipSheet> slipsheetLookup;
        private DocumentCollection documents;
        private XrefSlipSheetSettings slipsheetSettings;
        private DirectoryInfo volumeDirectory;

        /// <summary>
        /// The count of created slipsheets.
        /// </summary>
        public int Count { get { return slipsheetLookup.Count; } }

        /// <summary>
        /// The XREF slipsheet settings used to create slipsheets.
        /// </summary>
        public XrefSlipSheetSettings Settings { get { return slipsheetSettings; } }

        /// <summary>
        /// Indexor for the collection of slipsheets.
        /// </summary>
        /// <param name="index">The index of the target slipsheet.</param>
        /// <returns>Returns a slipsheet.</returns>
        public SlipSheet this[int index]
        {
            get
            {
                string key = slipsheetLookup.Keys.ElementAt(index);
                return slipsheetLookup[key];
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SlipSheets"/>.
        /// </summary>
        /// <param name="docs">The document collection to create slipsheets for.</param>
        /// <param name="settings">The XREF slipsheet settings.</param>
        /// <param name="vol">The volume directory used when saving slipsheets.</param>
        public SlipSheets(DocumentCollection docs, XrefSlipSheetSettings settings, DirectoryInfo vol)
        {
            this.documents = docs;
            this.slipsheetSettings = settings;
            this.slipsheetLookup = new Dictionary<string, SlipSheet>();
            this.volumeDirectory = vol;
            generateSlipSheets();
        }

        /// <summary>
        /// Gets the slipsheet text for a specific document.
        /// </summary>
        /// <param name="doc">The target document.</param>
        /// <returns>Returns the text for a slipsheet.</returns>
        protected string getSlipSheetText(Document doc)
        {
            StringBuilder text = new StringBuilder();

            foreach (var kvp in this.slipsheetSettings.FieldList)
            {
                if (this.slipsheetSettings.IsUseFieldLabels)
                {
                    text.Append(kvp.Value + ": ");
                }

                text.Append(doc.Metadata[kvp.Key].ToString() + Environment.NewLine);
            }

            return text.ToString();
        }

        /// <summary>
        /// Generates slipsheet for a document collection.
        /// </summary>
        protected void generateSlipSheets()
        {
            Document previousDoc = null;
            
            foreach (Document doc in this.documents)
            {                
                if (XrefExporter.IsFlagNeeded(doc, this.slipsheetSettings.Trigger, previousDoc))
                {
                    string text = getSlipSheetText(doc);
                    slipsheetLookup.Add(doc.Key, new SlipSheet(doc.Key, text));
                }

                previousDoc = doc;
            }
        }

        /// <summary>
        /// Gets the slipsheet image of a specific document.
        /// </summary>
        /// <param name="docid">The docid of the target document.</param>
        /// <returns>Returns an image if the document has a slipsheet.
        /// Otherwise returns null.</returns>
        public Image GetSlipSheetImage(string docid)
        {
            Image img = null;

            if (this.slipsheetLookup.ContainsKey(docid))
            {
                SlipSheet ss = this.slipsheetLookup[docid];
                img = ss.GetImage(this.slipsheetSettings);
            }

            return img;
        }

        /// <summary>
        /// Tests if a document has a slipsheet.
        /// </summary>
        /// <param name="docid">The docid value of the target document.</param>
        /// <returns>Returns true if the document has a slipsheet, otherwise false.</returns>
        public bool HasSlipSheet(string docid)
        {
            return this.slipsheetLookup.ContainsKey(docid);
        }

        /// <summary>
        /// Gets the XREF record for a specific slipsheet.
        /// </summary>
        /// <param name="doc">The target document.</param>
        /// <param name="gs">The group start value for the record.</param>
        /// <param name="cs">The code start value for the record.</param>
        /// <param name="custData">The customer data value for the record.</param>
        /// <param name="namedFolder">The named folder value for the record.</param>
        /// <param name="namedFile">The named file value for the record.</param>
        /// <returns>Returns an XREF record.</returns>
        public string GetSlipSheetXrefLine(Document doc, 
            string gs, string cs, string custData, string namedFolder, string namedFile)
        {
            string line = String.Empty;

            if (HasSlipSheet(doc.Key))
            {
                string ssImageKey = String.Format("{0}.001", BatesNumber.PreviousNumber(doc.Key));
                BatesNumber ssBates = new BatesNumber(ssImageKey);
                string imagePath = doc.Representatives
                    .Where(r => r.Type == Representative.FileType.Image).First()
                    .Files.First().Value;
                FileInfo imageFile = new FileInfo(imagePath);
                String path = (this.slipsheetSettings.IsPlaceInFolder) 
                    ? String.Format("\\{0}\\{1}.TIF", this.slipsheetSettings.SlipsheetFolder, ssImageKey) 
                    : Path.Combine(imageFile.Directory.FullName, String.Format("{0}.TIF", ssImageKey));

                line = String.Format(
                    "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}",
                    path,
                    ssBates.Prefix,
                    ssBates.NumberAsString,
                    ssBates.SuffixAsString,
                    gs,
                    0, // group end
                    1, // staple
                    0, // loose
                    cs,
                    0, // code end
                    1, // label bypass
                    custData,
                    namedFolder,
                    namedFile);

                writeSlipsheet(path, this.slipsheetLookup[doc.Key]);
            }

            return line;
        }

        /// <summary>
        /// Saves a slipsheet to the provided destination. The destination will 
        /// be created if it does not exist. The path value is prepended with the 
        /// volume directory.
        /// </summary>
        /// <param name="path">The destination to write the slipsheets.</param>
        /// <param name="slipsheet">The slipsheet to create.</param>
        protected void writeSlipsheet(string path, SlipSheet slipsheet)
        {
            path = path.TrimStart('\\');
            string ssPath = (this.volumeDirectory != null) 
                ? Path.Combine(this.volumeDirectory.FullName, path) 
                : path;
            FileInfo ssFile = new FileInfo(ssPath);

            if (!Directory.Exists(ssFile.Directory.FullName))
            {
                Directory.CreateDirectory(ssFile.Directory.FullName);
            }

            slipsheet.SaveImage(this.slipsheetSettings, ssPath);
        }
    }
}
