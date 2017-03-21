using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class LfpBuildDocSettings : ImageBuildDocSettings
    {
        private string[] nativeRecord;

        public string[] NativeRecord { get { return nativeRecord; } }

        public LfpBuildDocSettings(
            List<string[]> pageRecords, string[] nativeRecord, StructuredRepresentativeSetting textRepSetting, string pathPrefix) :
            base(pageRecords, textRepSetting, pathPrefix)
        {
            this.nativeRecord = nativeRecord;
        }
    }
}
