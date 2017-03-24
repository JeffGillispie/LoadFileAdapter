using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Represents the settings used to build a document collection from an image load file.
    /// </summary>
    public class BuildDocCollectionImageSettings : BuildDocCollectionSettings
    {
        private TextRepresentativeSettings textRepSettings;

        /// <summary>
        /// The text representative settings.
        /// </summary>
        public TextRepresentativeSettings TextSettings { get { return textRepSettings; } }

        /// <summary>
        /// Inisializes a new instance of <see cref="BuildDocCollectionImageSettings"/>
        /// </summary>
        /// <param name="records">A list of records from an image load file.</param>
        /// <param name="pathPrefix">The value that should be prepended to representative path values.</param>
        /// <param name="textRepSettings">The text representative settings.</param>
        public BuildDocCollectionImageSettings(
            List<string[]> records, string pathPrefix, TextRepresentativeSettings textRepSettings) :
            base(records, pathPrefix)
        {
            this.textRepSettings = textRepSettings;
        } 
    }
}
