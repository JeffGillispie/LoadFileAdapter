﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Defines how slipsheets will be created.
    /// </summary>
    public class SlipsheetsInfo : IEquatable<SlipsheetsInfo>
    {
        /// <summary>
        /// Determines if field labels will be printed on the slipsheet.
        /// </summary>
        public bool UseFieldLabels = true;

        /// <summary>
        /// Determines if the slipsheet will become the first page of the document.
        /// </summary>
        public bool BindSlipsheets = false;

        /// <summary>
        /// The horizontal placement of the slipsheet text.
        /// </summary>
        public SlipSheet.HorizontalPlacementOption HorizontalPlacement = SlipSheet.HorizontalPlacementOption.Left;

        /// <summary>
        /// The vertical placement of the slipsheet text.
        /// </summary>
        public SlipSheet.VerticalPlacementOption VerticalPlacement = SlipSheet.VerticalPlacementOption.Top;

        /// <summary>
        /// The name of the slipsheet folder or empty if slipsheets are saved to the images folder.
        /// </summary>
        public string FolderName;

        /// <summary>
        /// The resolution of the slipsheet.
        /// </summary>
        public int Resolution = 300;

        /// <summary>
        /// The font family for the slipsheet text.
        /// </summary>
        public string FontFamilyName;

        /// <summary>
        /// The font size for the slipsheet text.
        /// </summary>
        public float FontSize = 12;

        /// <summary>
        /// The font style for the slipsheet text.
        /// </summary>
        public FontStyle FontStyle;

        /// <summary>
        /// The fields that will be printed on the slipsheet.
        /// </summary>
        [XmlArray("Fields")]
        [XmlArrayItem(typeof(SlipsheetField), ElementName = "Field")]
        public SlipsheetField[] Fields;

        /// <summary>
        /// The trigger that produces a slipsheet for a document.
        /// </summary>
        public Trigger Trigger;

        [XmlIgnore]
        private DirectoryInfo destinationFolder;

        /// <summary>
        /// Initializes a new instance of <see cref="SlipsheetsInfo"/>.
        /// </summary>
        public SlipsheetsInfo()
        { 
            // do nothing here
        }

        [XmlIgnore]
        public DirectoryInfo DestinationFolder
        {
            get
            {
                return destinationFolder;
            }

            set
            {
                destinationFolder = value;
            }
        }

        public string DestinationFolderPath
        {
            get
            {
                return destinationFolder.FullName;
            }

            set
            {
                destinationFolder = new DirectoryInfo(value);
            }
        }

        /// <summary>
        /// Gets an XREF slipsheet settings object.
        /// </summary>
        /// <returns>Returns a <see cref="XrefSlipSheetSettings"/>.</returns>
        public SlipSheets ToSlipSheets()
        {
            return SlipSheets.Builder
                .Start(Trigger.ToSwitch())
                .SetUseFieldLabels(UseFieldLabels)
                .SetBindSlipsheets(BindSlipsheets)
                .SetHorizontalTextPlacement(HorizontalPlacement)
                .SetVerticalTextPlacement(VerticalPlacement)
                .SetFolderName(FolderName)
                .SetFont(GetFont())
                .SetResolution(Resolution)
                .SetAliasMap(Fields.ToDictionary(f => f.FieldName, f=> f.Alias))
                .SetDestinationFolder(destinationFolder)
                .Build();            
        }

        /// <summary>
        /// Gets the font for the slipsheet text.
        /// </summary>
        /// <returns>Returns a <see cref="Font"/>.</returns>
        public Font GetFont()
        {
            return new Font(this.FontFamilyName, this.FontSize * (this.Resolution / 100), this.FontStyle);
        }

        public bool Equals(SlipsheetsInfo slipsheetsInfo)
        {
            if (slipsheetsInfo == null) return false;

            return this.UseFieldLabels == slipsheetsInfo.UseFieldLabels &&
                this.BindSlipsheets == slipsheetsInfo.BindSlipsheets &&
                (this.FolderName == slipsheetsInfo.FolderName || this.FolderName.Equals(slipsheetsInfo.FolderName)) &&
                this.FontFamilyName.Equals(slipsheetsInfo.FontFamilyName) &&
                this.FontSize == slipsheetsInfo.FontSize &&
                this.FontStyle == slipsheetsInfo.FontStyle &&
                this.Resolution == slipsheetsInfo.Resolution &&
                this.HorizontalPlacement == slipsheetsInfo.HorizontalPlacement &&
                this.VerticalPlacement == slipsheetsInfo.VerticalPlacement &&
                this.Trigger.Equals(slipsheetsInfo.Trigger) &&
                this.Fields.SequenceEqual(slipsheetsInfo.Fields);
        }
    }
}
