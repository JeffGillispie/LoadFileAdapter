using System.Drawing;
using System.Linq;
using System.Xml.Serialization;
using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    /// <summary>
    /// Defines how slipsheets will be created.
    /// </summary>
    public class SlipsheetsInfo
    {
        /// <summary>
        /// Determines if field labels will be printed on the slipsheet.
        /// </summary>
        public bool UseFieldLabels;

        /// <summary>
        /// Determines if the slipsheet will become the first page of the document.
        /// </summary>
        public bool BindSlipsheets;

        /// <summary>
        /// The horizontal placement of the slipsheet text.
        /// </summary>
        public XrefSlipSheetSettings.HorizontalPlacementOption HorizontalPlacement = XrefSlipSheetSettings.HorizontalPlacementOption.Left;

        /// <summary>
        /// The vertical placement of the slipsheet text.
        /// </summary>
        public XrefSlipSheetSettings.VerticalPlacementOption VerticalPlacement = XrefSlipSheetSettings.VerticalPlacementOption.Top;

        /// <summary>
        /// The name of the slipsheet folder or empty if slipsheets are saved to the images folder.
        /// </summary>
        public string FolderName;

        /// <summary>
        /// The resolution of the slipsheet.
        /// </summary>
        public int Resolution;

        /// <summary>
        /// The font family for the slipsheet text.
        /// </summary>
        public string FontFamilyName;

        /// <summary>
        /// The font size for the slipsheet text.
        /// </summary>
        public float FontSize;

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

        /// <summary>
        /// Initializes a new instance of <see cref="SlipsheetsInfo"/>.
        /// </summary>
        public SlipsheetsInfo()
        { 
            // do nothing here
        }

        /// <summary>
        /// Gets an XREF slipsheet settings object.
        /// </summary>
        /// <returns>Returns a <see cref="XrefSlipSheetSettings"/>.</returns>
        public XrefSlipSheetSettings GetSlipsheetSettings()
        {
            return new XrefSlipSheetSettings(
                this.UseFieldLabels,
                this.BindSlipsheets,
                !string.IsNullOrEmpty(this.FolderName),
                this.HorizontalPlacement,
                this.VerticalPlacement,
                this.FolderName,
                GetFont(),
                this.Resolution,
                this.Fields.ToDictionary(f => f.FieldName, f => f.Alias), 
                this.Trigger.GetXrefTrigger());
        }

        /// <summary>
        /// Gets the font for the slipsheet text.
        /// </summary>
        /// <returns>Returns a <see cref="Font"/>.</returns>
        public Font GetFont()
        {
            return new Font(this.FontFamilyName, this.FontSize * (this.Resolution / 100), this.FontStyle);
        }
    }
}
