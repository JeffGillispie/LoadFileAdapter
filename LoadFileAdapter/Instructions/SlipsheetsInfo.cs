using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;
using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    public class SlipsheetsInfo
    {
        public bool UseFieldLabels;
        public bool BindSlipsheets;        
        public XrefSlipSheetSettings.HorizontalPlacementOption HorizontalPlacement = XrefSlipSheetSettings.HorizontalPlacementOption.Left;
        public XrefSlipSheetSettings.VerticalPlacementOption VerticalPlacement = XrefSlipSheetSettings.VerticalPlacementOption.Top;
        public string FolderName;
        public Font Font;
        public int Resolution;

        [XmlArray("Fields")]
        [XmlArrayItem(typeof(SlipsheetField), ElementName = "Field")]
        public SlipsheetField[] Fields;

        public Trigger Trigger;

        public SlipsheetsInfo()
        {
            // do nothing here
        }

        public XrefSlipSheetSettings GetSlipsheetSettings()
        {
            return new XrefSlipSheetSettings(
                this.UseFieldLabels,
                this.BindSlipsheets,
                !string.IsNullOrEmpty(this.FolderName),
                this.HorizontalPlacement,
                this.VerticalPlacement,
                this.FolderName,
                this.Font,
                this.Resolution,
                this.Fields.ToDictionary(f => f.FieldName, f => f.Alias), 
                this.Trigger.GetXrefTrigger());
        }
    }
}
