using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class BuildDocLfpSettings : BuildDocImageSettings
    {
        private string[] nativeRecord;

        public string[] NativeRecord { get { return nativeRecord; } }

        public BuildDocLfpSettings(
            List<string[]> pageRecords, string[] nativeRecord, TextFileSettings textRepSetting, string pathPrefix) :
            base(pageRecords, textRepSetting, pathPrefix)
        {
            this.nativeRecord = nativeRecord;
        }
    }
}
