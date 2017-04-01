
namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Export settings for an image load file (i.e. LFP or OPT).
    /// </summary>
    public interface IExportImageSettings : IExportSettings
    {
        /// <summary>
        /// Gets the export volume name.
        /// </summary>
        /// <returns>Returns the name of the export volume.</returns>
        string GetVolumeName();
    }
}
