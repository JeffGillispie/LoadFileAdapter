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
                (base.File != null) ? base.File : null, 
                (base.Encoding != null) ? base.Encoding : null, 
                (this.BoxBreakTrigger != null) ? this.BoxBreakTrigger.GetXrefTrigger() : null,
                (this.GroupStartTrigger != null) ? this.GroupStartTrigger.GetXrefTrigger() : null,
                (this.CodeStartTrigger != null) ? this.CodeStartTrigger.GetXrefTrigger() : null,
                this.CustomerDataField,
                this.NamedFolderField,
                this.NamedFileField,
                (this.SlipsheetSettings != null) ? this.SlipsheetSettings.GetSlipsheetSettings() : null);
        }
    }
}
