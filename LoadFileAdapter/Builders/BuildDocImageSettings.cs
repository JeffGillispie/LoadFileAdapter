using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Represents the settings used to build a document from an image load file.
    /// </summary>
    public class BuildDocImageSettings : BuildDocSettings
    {
        private List<string[]> pageRecords;
        private TextRepresentativeSettings textSettings;

        /// <summary>
        /// A list of page record field values from an image load file.
        /// </summary>
        public List<string[]> PageRecords { get { return pageRecords; } }

        /// <summary>
        /// The settings used to create text representatives from an image load file.
        /// </summary>
        public TextRepresentativeSettings TextSettings { get { return textSettings; } }

        /// <summary>
        /// Initializes a new instance of <see cref="BuildDocImageSettings"/>
        /// </summary>
        /// <param name="pageRecords">A list of page records from an image load file.</param>
        /// <param name="textSettings">The settings used to build text representatives.</param>
        /// <param name="pathPrefix">The value used to prepend representative paths.</param>
        public BuildDocImageSettings(List<string[]> pageRecords, 
            TextRepresentativeSettings textSettings, string pathPrefix) :
            base(pathPrefix)
        {
            this.pageRecords = pageRecords;
            this.textSettings = textSettings;
        }
    }
}
