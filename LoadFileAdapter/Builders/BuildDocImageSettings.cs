using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class BuildDocImageSettings : BuildDocSettings
    {
        private List<string[]> pageRecords;
        private StructuredRepresentativeSetting textRepSetting;

        public List<string[]> PageRecords { get { return pageRecords; } }
        public StructuredRepresentativeSetting TextSetting { get { return textRepSetting; } }

        public BuildDocImageSettings(List<string[]> pageRecords, StructuredRepresentativeSetting textRepSetting, string pathPrefix) :
            base(pathPrefix)
        {
            this.pageRecords = pageRecords;
            this.textRepSetting = textRepSetting;
        }
    }
}
