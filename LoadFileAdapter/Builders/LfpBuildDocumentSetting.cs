using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class LfpBuildDocumentSetting : ImageBuildDocumentSetting
    {
        private string[] nativeRecord;

        public string[] NativeRecord { get { return nativeRecord; } }

        public LfpBuildDocumentSetting(
            List<string[]> pageRecords, string[] nativeRecord, StructuredRepresentativeSetting textRepSetting, string pathPrefix) :
            base(pageRecords, textRepSetting, pathPrefix)
        {
            this.nativeRecord = nativeRecord;
        }
    }
}
