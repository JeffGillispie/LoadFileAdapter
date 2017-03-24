using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class BuildDocImageSettings : BuildDocSettings
    {
        private List<string[]> pageRecords;
        private TextRepresentativeSettings textSetting;

        public List<string[]> PageRecords { get { return pageRecords; } }
        public TextRepresentativeSettings TextSetting { get { return textSetting; } }

        public BuildDocImageSettings(List<string[]> pageRecords, TextRepresentativeSettings textSetting, string pathPrefix) :
            base(pathPrefix)
        {
            this.pageRecords = pageRecords;
            this.textSetting = textSetting;
        }
    }
}
