using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class BuildDocImageSettings : BuildDocSettings
    {
        private List<string[]> pageRecords;
        private TextFileSettings textSetting;

        public List<string[]> PageRecords { get { return pageRecords; } }
        public TextFileSettings TextSetting { get { return textSetting; } }

        public BuildDocImageSettings(List<string[]> pageRecords, TextFileSettings textSetting, string pathPrefix) :
            base(pathPrefix)
        {
            this.pageRecords = pageRecords;
            this.textSetting = textSetting;
        }
    }
}
