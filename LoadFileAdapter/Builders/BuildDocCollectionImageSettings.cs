using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class BuildDocCollectionImageSettings : BuildDocCollectionSettings
    {
        private TextFileSettings textRepSetting;

        public TextFileSettings TextSetting { get { return textRepSetting; } }

        public BuildDocCollectionImageSettings(List<string[]> records, string pathPrefix, TextFileSettings textRepSetting) :
            base(records, pathPrefix)
        {
            this.textRepSetting = textRepSetting;
        } 
    }
}
