
namespace LoadFileAdapter.Exporters
{
    public interface IExportXrefSettings : IExportSettings
    {
        //todo: comments
        XrefTrigger GetBoxBreakTrigger();
        XrefTrigger GetGroupStartTrigger();
        XrefTrigger GetCodeStartTrigger();
        string GetCustomerData();
        string GetNamedFolder();
        string GetNamedFile();
        XrefSlipSheetSettings GetSlipsheets();
    }
}
