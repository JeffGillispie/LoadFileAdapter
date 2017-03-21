using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class ImageBuildDocCollectionSettings : BuildDocCollectionSettings
    {
        private StructuredRepresentativeSetting textRepSetting;

        public StructuredRepresentativeSetting TextSetting { get { return textRepSetting; } }

        public ImageBuildDocCollectionSettings(List<string[]> records, string pathPrefix, StructuredRepresentativeSetting textRepSetting) :
            base(records, pathPrefix)
        {
            this.textRepSetting = textRepSetting;
        } 
    }
}
