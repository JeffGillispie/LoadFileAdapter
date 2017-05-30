
namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// The XREF export settings interface.
    /// </summary>
    public interface IExportXrefSettings : IExportSettings
    {
        /// <summary>
        /// The trigger used to determine when a box should change.
        /// </summary>
        /// <returns>Returns a <see cref="XrefTrigger"/>.</returns>
        XrefTrigger GetBoxBreakTrigger();

        /// <summary>
        /// The trigger used to determine when the group start value should change.
        /// </summary>
        /// <returns>Returns a <see cref="XrefTrigger"/>.</returns>
        XrefTrigger GetGroupStartTrigger();

        /// <summary>
        /// The trigger used to determine when the code start value should change.
        /// </summary>
        /// <returns>Returns a <see cref="XrefTrigger"/>.</returns>
        XrefTrigger GetCodeStartTrigger();

        /// <summary>
        /// Gets the name of the metadata field to use for customer data.
        /// </summary>
        /// <returns>Returns the customer data metadata field name.</returns>
        string GetCustomerData();

        /// <summary>
        /// Gets the name of the metadata field to use for named folders.
        /// </summary>
        /// <returns>Returns the named folder metadata field name.</returns>
        string GetNamedFolder();

        /// <summary>
        /// Gets the name of the metadata field to use for named files.
        /// </summary>
        /// <returns>Returns the named file metadata field name.</returns>
        string GetNamedFile();

        /// <summary>
        /// Gets the slipsheet settings used to export an XREF.
        /// </summary>
        /// <returns>Returns slipsheet settings.</returns>
        XrefSlipSheetSettings GetSlipsheets();
    }
}
