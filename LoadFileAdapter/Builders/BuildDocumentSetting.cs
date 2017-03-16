
namespace LoadFileAdapter.Builders
{
    public abstract class BuildDocumentSetting
    {
        private string pathPrefix;

        public string PathPrefix { get { return pathPrefix; } }

        public BuildDocumentSetting(string pathPrefix)
        {
            this.pathPrefix = pathPrefix;
        }
    }
}
