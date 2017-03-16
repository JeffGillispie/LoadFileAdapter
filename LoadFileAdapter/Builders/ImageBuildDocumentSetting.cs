using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class ImageBuildDocumentSetting : BuildDocumentSetting
    {
        private List<string[]> pageRecords;
        private StructuredRepresentativeSetting textRepSetting;

        public List<string[]> PageRecords { get { return pageRecords; } }
        public StructuredRepresentativeSetting TextSetting { get { return textRepSetting; } }

        public ImageBuildDocumentSetting(List<string[]> pageRecords, StructuredRepresentativeSetting textRepSetting, string pathPrefix) :
            base(pathPrefix)
        {
            this.pageRecords = pageRecords;
            this.textRepSetting = textRepSetting;
        }
    }
}
