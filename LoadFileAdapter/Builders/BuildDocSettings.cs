
namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// The base settings used to build a document.
    /// </summary>
    public abstract class BuildDocSettings
    {
        private string pathPrefix;

        /// <summary>
        /// The value used to prepend representative paths.
        /// </summary>
        public string PathPrefix { get { return pathPrefix; } }

        /// <summary>
        /// Initializes a new instance of <see cref="BuildDocSettings"/>
        /// </summary>
        /// <param name="pathPrefix">The value used to prepend representative paths.</param>
        public BuildDocSettings(string pathPrefix)
        {
            this.pathPrefix = pathPrefix;
        }
    }
}
