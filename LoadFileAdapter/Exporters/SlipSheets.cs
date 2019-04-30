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
        private Dictionary<string, SlipSheet> slipsheetLookup = new Dictionary<string, SlipSheet>();                       
        private DirectoryInfo destinationDir;
        private bool useFieldLabels = true;
        private bool bindSlipsheets = false;        
        private SlipSheet.HorizontalPlacementOption horizontalPlacement = SlipSheet.HorizontalPlacementOption.Left;
        private SlipSheet.VerticalPlacementOption verticalPlacement = SlipSheet.VerticalPlacementOption.Top;
        private string slipsheetFolderName;
        private Font slipsheetFont = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        private int resolution = 300;
        private Dictionary<string, string> aliasMap;
        private Switch trigger;

        private SlipSheets()
        {
            // do nothing here
        }

        /// <summary>
        /// The count of created slipsheets.
        /// </summary>
        public int Count { get { return slipsheetLookup.Count; } }

        /// <summary>
        /// Determines if slipsheets should become the first page of the document.
        /// </summary>
        public bool IsBindSlipsheets { get { return bindSlipsheets; } }

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
        /// Gets the slipsheet text for a specific document.
        /// </summary>
        /// <param name="doc">The target document.</param>
        /// <returns>Returns the text for a slipsheet.</returns>
        protected string getSlipSheetText(Document doc)
        {
            StringBuilder text = new StringBuilder();

            foreach (var kvp in aliasMap)
            {
                if (useFieldLabels)
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
        public void GenerateSlipSheets(DocumentCollection docs)
        {
            Document previousDoc = null;
            
            foreach (Document doc in docs)
            {                
                if (trigger != null && trigger.IsTriggered(doc, previousDoc))
                {
                    string text = getSlipSheetText(doc);
                    SlipSheet ss = SlipSheet.Builder
                        .Start(doc.Key, text)
                        .SetFont(slipsheetFont)
                        .SetResolution(resolution)
                        .SetHorizontalTextPlacement(horizontalPlacement)
                        .SetVerticalTextPlacement(verticalPlacement)
                        .Build();
                    slipsheetLookup.Add(doc.Key, ss);
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

            if (slipsheetLookup.ContainsKey(docid))
            {
                SlipSheet ss = slipsheetLookup[docid];
                img = ss.GetImage();
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
            return slipsheetLookup.ContainsKey(docid);
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
                String path = (!String.IsNullOrWhiteSpace(slipsheetFolderName)) 
                    ? String.Format("\\{0}\\{1}.TIF", slipsheetFolderName, ssImageKey) 
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
            string ssPath = (destinationDir != null) 
                ? Path.Combine(destinationDir.FullName, path) 
                : path;
            FileInfo ssFile = new FileInfo(ssPath);

            if (!Directory.Exists(ssFile.Directory.FullName))
            {
                Directory.CreateDirectory(ssFile.Directory.FullName);
            }

            slipsheet.SaveImage(ssPath);
        }

        /// <summary>
        /// Builds an instance of <see cref="SlipSheets"/>.
        /// </summary>
        public class Builder
        {
            private SlipSheets instance;

            private Builder()
            {
                instance = new SlipSheets();
            }

            /// <summary>
            /// Starts the process of building a <see cref="SlipSheets"/> instance.
            /// </summary>
            /// <param name="trigger">The <see cref="SlipSheet"/> trigger.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public static Builder Start(Switch trigger)
            {
                Builder builder = new Builder();                
                builder.instance.trigger = trigger;
                return builder;
            }

            /// <summary>
            /// Sets the slipsheet destination folder.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetDestinationFolder(DirectoryInfo value)
            {
                instance.destinationDir = value;
                return this;
            }

            /// <summary>
            /// Sets the indicator for use of field labels.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetUseFieldLabels(bool value)
            {
                instance.useFieldLabels = value;
                return this;
            }

            /// <summary>
            /// Sets the indicator for binding slipsheets to documents.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetBindSlipsheets(bool value)
            {
                instance.bindSlipsheets = value;
                return this;
            }

            /// <summary>
            /// Sets the horizontal text placement option.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/>.</returns>
            public Builder SetHorizontalTextPlacement(SlipSheet.HorizontalPlacementOption value)
            {
                instance.horizontalPlacement = value;
                return this;
            }

            /// <summary>
            /// Sets the vertical text placement option.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/></returns>
            public Builder SetVerticalTextPlacement(SlipSheet.VerticalPlacementOption value)
            {
                instance.verticalPlacement = value;
                return this;
            }

            /// <summary>
            /// Sets the name of the slipsheet folder.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/></returns>
            public Builder SetFolderName(string value)
            {
                instance.slipsheetFolderName = value;
                return this;
            }

            /// <summary>
            /// Sets the slipsheet font.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/></returns>
            public Builder SetFont(Font value)
            {
                instance.slipsheetFont = value;
                return this;
            }

            /// <summary>
            /// Sets the slipsheet resolution.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/></returns>
            public Builder SetResolution(int value)
            {
                instance.resolution = value;
                return this;
            }

            /// <summary>
            /// Sets the alias map that maps metadata field names to an alias.
            /// </summary>
            /// <param name="value">The value to set.</param>
            /// <returns>Returns a <see cref="Builder"/></returns>
            public Builder SetAliasMap(Dictionary<string, string> value)
            {
                instance.aliasMap = value;
                return this;
            }

            /// <summary>
            /// Builds a <see cref="SlipSheets"/> instance.
            /// </summary>
            /// <returns>Returns a <see cref="SlipSheets"/> instance.</returns>
            public SlipSheets Build()
            {
                SlipSheets instance = this.instance;
                this.instance = null;
                return instance;
            }
        }
    }
}
