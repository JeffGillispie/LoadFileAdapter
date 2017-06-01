using System.Collections.Generic;
using System.Drawing;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The slipsheet settings to use for an XREF export.
    /// </summary>
    public class XrefSlipSheetSettings
    {
        private bool useFieldLabels;
        private bool bindSlipsheets;
        private bool placeInFolder;
        private HorizontalPlacementOption horizontalPlacement;
        private VerticalPlacementOption verticalPlacement;
        private string slipsheetFolder;
        private Font slipsheetFont;
        private int resolution;
        private Dictionary<string, string> fieldList;
        private XrefTrigger trigger;

        /// <summary>
        /// Determines if labes should be added to the slipsheet text.
        /// </summary>
        public bool IsUseFieldLabels { get { return useFieldLabels; } }

        /// <summary>
        /// Determines if slipsheets should become the first page of their document.
        /// </summary>
        public bool IsBindSlipsheets { get { return bindSlipsheets; } }

        /// <summary>
        /// Determines if slipsheets should be saved to <see cref="SlipsheetFolder"/>.
        /// </summary>
        public bool IsPlaceInFolder { get { return placeInFolder; } }

        /// <summary>
        /// The horizontal placement option for slipsheet text.
        /// </summary>
        public HorizontalPlacementOption HorizontalPlacement { get { return horizontalPlacement; } }

        /// <summary>
        /// the vertical placement option for slipsheet text.
        /// </summary>
        public VerticalPlacementOption VerticalPlacement { get { return verticalPlacement; } }

        /// <summary>
        /// The optional name of a slipsheet folder.
        /// </summary>
        public string SlipsheetFolder { get { return slipsheetFolder; } }

        /// <summary>
        /// The font to use for slipsheet text.
        /// </summary>
        public Font SlipsheetFont { get { return slipsheetFont; } }

        /// <summary>
        /// The resolution to use for creating slipsheets.
        /// </summary>
        public int Resolution { get { return resolution; } }

        /// <summary>
        /// A map of actual metadata field names to an alias.
        /// </summary>
        public Dictionary<string, string> FieldList { get { return fieldList; } }

        /// <summary>
        /// The trigger used to determine if a slipsheet should be created.
        /// </summary>
        public XrefTrigger Trigger { get { return trigger; } }

        /// <summary>
        /// Initializes a new instance of <see cref="XrefSlipSheetSettings"/>.
        /// </summary>
        /// <param name="useFieldLabels">Indicates if labels should be added to slipsheet text.</param>
        /// <param name="bindSlipsheets">Indicates if slipsheets should be bound as the first page to its document.</param>
        /// <param name="placeInFolder">Indicates if slipsheet should be saved to the slipsheet folder.</param>
        /// <param name="horizontalPlacement">Determines the horizontal placement of slipsheet text.</param>
        /// <param name="verticalPlacement">Determines the vertical placement of slipsheet text.</param>
        /// <param name="slipsheetFolder">The name of the folder to save the slipsheets to.</param>
        /// <param name="slipsheetFont">The font to use for the slipsheet text.</param>
        /// <param name="resolution">The resolution setting for slipsheets.</param>
        /// <param name="fieldList">A map of actual metadata field names to an alias.</param>
        /// <param name="trigger">The trigger used to determine if a slipsheet should be created.</param>
        public XrefSlipSheetSettings(bool useFieldLabels, bool bindSlipsheets, bool placeInFolder, 
            HorizontalPlacementOption horizontalPlacement, VerticalPlacementOption verticalPlacement,
            string slipsheetFolder, Font slipsheetFont, int resolution, 
            Dictionary<string, string> fieldList, XrefTrigger trigger)
        {
            this.useFieldLabels = useFieldLabels;
            this.bindSlipsheets = bindSlipsheets;
            this.placeInFolder = placeInFolder;
            this.horizontalPlacement = horizontalPlacement;
            this.verticalPlacement = verticalPlacement;
            this.slipsheetFolder = slipsheetFolder;
            this.slipsheetFont = slipsheetFont;
            this.resolution = resolution;
            this.fieldList = fieldList;
            this.trigger = trigger;
        }

        /// <summary>
        /// The horizontal placement option for slipsheet text.
        /// </summary>
        public enum HorizontalPlacementOption
        {
            Center, Left
        }

        /// <summary>
        /// The vertical placement option for slipsheet text.
        /// </summary>
        public enum VerticalPlacementOption
        {
            Center, Top
        }
    }
}
