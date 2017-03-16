using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class TabularBuildDocumentSetting : BuildDocumentSetting
    {
        private string[] record;
        private string[] header;
        private string keyColName;
        private List<SemiStructuredRepresentativeSetting> repColInfo;

        public string[] Record { get { return record; } }
        public string[] Header { get { return header; } }
        public string KeyColumnName { get { return keyColName; } }
        public List<SemiStructuredRepresentativeSetting> RepresentativeColumnInfo { get { return repColInfo; } }

        public TabularBuildDocumentSetting(string[] record, string[] header, string keyColName, 
            List<SemiStructuredRepresentativeSetting> repColInfo, string pathPrefix) :
            base(pathPrefix)
        {
            this.record = record;
            this.header = header;
            this.keyColName = keyColName;
            this.repColInfo = repColInfo;            
        }
    }
}
