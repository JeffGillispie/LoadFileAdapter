
namespace LoadFileAdapter.Builders
{
    public abstract class BuildDocSettings
    {
        private string pathPrefix;

        public string PathPrefix { get { return pathPrefix; } }

        public BuildDocSettings(string pathPrefix)
        {
            this.pathPrefix = pathPrefix;
        }
    }
}
