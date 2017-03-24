using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Base settings for building a document collection.
    /// </summary>
    public abstract class BuildDocCollectionSettings
    {
        private List<string[]> records;
        private string pathPrefix;

        /// <summary>
        /// A list of records used to build a document collection.
        /// </summary>
        public List<string[]> Records { get { return records; } }

        /// <summary>
        /// The value to preprend to any representative paths.
        /// </summary>
        public string PathPrefix { get { return pathPrefix; } }

        /// <summary>
        /// Initializes a new instance of <see cref="BuildDocCollectionSettings"/>
        /// </summary>
        /// <param name="records">A list of records used to build a document collection.</param>
        /// <param name="pathPrefix">The value that should be prepended to any representative path values.</param>
        public BuildDocCollectionSettings(List<string[]> records, string pathPrefix)
        {
            this.records = records;
            this.pathPrefix = pathPrefix;
        }
    }
}
