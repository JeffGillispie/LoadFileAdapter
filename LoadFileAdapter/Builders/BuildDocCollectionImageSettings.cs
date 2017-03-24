using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class BuildDocCollectionImageSettings : BuildDocCollectionSettings
    {
        private TextRepresentativeSettings textRepSetting;

        public TextRepresentativeSettings TextSetting { get { return textRepSetting; } }

        public BuildDocCollectionImageSettings(List<string[]> records, string pathPrefix, TextRepresentativeSettings textRepSetting) :
            base(records, pathPrefix)
        {
            this.textRepSetting = textRepSetting;
        } 
    }
}
