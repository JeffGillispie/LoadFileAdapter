using System.Collections.Generic;
using System.Drawing;

namespace LoadFileAdapter.Exporters
{
    public class XrefSlipSheetSettings
    {
        //todo: comments
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

        public bool IsUseFieldLabels { get { return useFieldLabels; } }
        public bool IsBindSlipsheets { get { return bindSlipsheets; } }
        public bool IsPlaceInFolder { get { return placeInFolder; } }
        public HorizontalPlacementOption HorizontalPlacement { get { return horizontalPlacement; } }
        public VerticalPlacementOption VerticalPlacement { get { return verticalPlacement; } }
        public string SlipsheetFolder { get { return slipsheetFolder; } }
        public Font SlipsheetFont { get { return slipsheetFont; } }
        public int Resolution { get { return resolution; } }
        public Dictionary<string, string> FieldList { get { return fieldList; } }
        public XrefTrigger Trigger { get { return trigger; } }

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

        public enum HorizontalPlacementOption
        {
            Center, Left
        }

        public enum VerticalPlacementOption
        {
            Center, Top
        }
    }
}
