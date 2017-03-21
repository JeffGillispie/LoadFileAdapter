using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public abstract class BuildDocCollectionSettings
    {
        private List<string[]> records;
        private string pathPrefix;

        public List<string[]> Records { get { return records; } }
        public string PathPrefix { get { return pathPrefix; } }

        public BuildDocCollectionSettings(List<string[]> records, string pathPrefix)
        {
            this.records = records;
            this.pathPrefix = pathPrefix;
        }
    }
}
