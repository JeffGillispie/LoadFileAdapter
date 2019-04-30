using LoadFileAdapter.Exporters;

namespace LoadFileAdapter.Instructions
{
    public class XrefExport : Export
    {
        public string CustomerDataField;
        public string NamedFolderField;
        public string NamedFileField;
        public Trigger BoxTrigger;
        public Trigger GroupStartTrigger;
        public Trigger CodeStartTrigger;
        public SlipsheetsInfo Slipsheets;

        public XrefExport() : base(null, null)
        {
            // do nothing here
        }
        
        public override IExporter BuildExporter()
        {
            return XrefExporter.Builder
                .Start(File, Encoding)
                .SetBoxTrigger(BoxTrigger.ToSwitch())
                .SetCodeStartTrigger(CodeStartTrigger.ToSwitch())
                .SetGroupStartTrigger(GroupStartTrigger.ToSwitch())
                .SetCustomerDataField(CustomerDataField)
                .SetNamedFolderField(NamedFolderField)
                .SetNamedFileField(NamedFileField)
                .SetSlipsheets(Slipsheets.ToSlipSheets())
                .Build();
        }
    }
}
