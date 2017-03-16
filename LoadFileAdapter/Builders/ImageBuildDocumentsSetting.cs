using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class ImageBuildDocumentsSetting : BuildDocumentsSetting
    {
        private StructuredRepresentativeSetting textRepSetting;

        public StructuredRepresentativeSetting TextSetting { get { return textRepSetting; } }

        public ImageBuildDocumentsSetting(List<string[]> records, string pathPrefix, StructuredRepresentativeSetting textRepSetting) :
            base(records, pathPrefix)
        {
            this.textRepSetting = textRepSetting;
        } 
    }
}
