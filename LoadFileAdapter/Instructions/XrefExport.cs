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

        public ExportXrefFileSettings GetFileSettings(DocumentCollection docs)
        {
            return new ExportXrefFileSettings(docs, 
                base.File, 
                base.Encoding, 
                this.BoxBreakTrigger.GetXrefTrigger(),
                this.GroupStartTrigger.GetXrefTrigger(),
                this.CodeStartTrigger.GetXrefTrigger(),
                this.CustomerDataField,
                this.NamedFolderField,
                this.NamedFileField,
                this.SlipsheetSettings.GetSlipsheetSettings());
        }
    }
}
