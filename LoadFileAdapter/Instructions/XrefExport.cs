using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    public class XrefExport : Export
    {
        public string CustomerDataField;
        public string NamedFolderField;
        public string NamedFileField;
        public Trigger BoxBreakTrigger;
        public Trigger GroupStartTrigger;
        public Trigger CodeStartTrigger;
        public SlipsheetsInfo SlipsheetSettings;

        public XrefExport() : base(null, null)
        {
            // do nothing here
        }                
    }
}
