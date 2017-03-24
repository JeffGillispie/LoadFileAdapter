using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Represents the settings used to build a document from an LFP file.
    /// </summary>
    public class BuildDocLfpSettings : BuildDocImageSettings
    {
        private string[] nativeRecord;

        /// <summary>
        /// The field values of a record from an LFP file.
        /// </summary>
        public string[] NativeRecord { get { return nativeRecord; } }

        /// <summary>
        /// Inisializes a new instance of <see cref="BuildDocLfpSettings"/>
        /// </summary>
        /// <param name="pageRecords">A list of page records for a single document.</param>
        /// <param name="nativeRecord">The field values of a native record from an LFP file.</param>
        /// <param name="textRepSettings">The text representative settings.</param>
        /// <param name="pathPrefix">The value used to prepend representative path values.</param>
        public BuildDocLfpSettings(
            List<string[]> pageRecords, string[] nativeRecord, 
            TextRepresentativeSettings textRepSettings, string pathPrefix) :
            base(pageRecords, textRepSettings, pathPrefix)
        {
            this.nativeRecord = nativeRecord;
        }
    }
}
