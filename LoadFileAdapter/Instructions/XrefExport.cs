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
            return ExportXrefFileSettings.Builder.Start(docs, base.File, base.Encoding)
                .SetBoxTrigger((this.BoxBreakTrigger != null) ? this.BoxBreakTrigger.GetXrefTrigger() : null)
                .SetGroupStartTrigger((this.GroupStartTrigger != null) ? this.GroupStartTrigger.GetXrefTrigger() : null)
                .SetCodeStartTrigger((this.CodeStartTrigger != null) ? this.CodeStartTrigger.GetXrefTrigger() : null)
                .SetSlipsheets((this.SlipsheetSettings != null) ? this.SlipsheetSettings.GetSlipsheetSettings() : null)
                .SetCustomerData(this.CustomerDataField)
                .SetNamedFolder(this.NamedFolderField)
                .SetNamedFile(this.NamedFileField)
                .Build();            
        }
    }
}
