using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public abstract class BuildDocumentsSetting
    {
        private List<string[]> records;
        private string pathPrefix;

        public List<string[]> Records { get { return records; } }
        public string PathPrefix { get { return pathPrefix; } }

        public BuildDocumentsSetting(List<string[]> records, string pathPrefix)
        {
            this.records = records;
            this.pathPrefix = pathPrefix;
        }
    }
}
