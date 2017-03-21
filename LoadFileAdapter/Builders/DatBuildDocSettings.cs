using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class DatBuildDocSettings : BuildDocSettings
    {
        private string[] record;
        private string[] header;
        private string keyColName;
        private List<LinkFileSettings> repColInfo;

        public string[] Record { get { return record; } }
        public string[] Header { get { return header; } }
        public string KeyColumnName { get { return keyColName; } }
        public List<LinkFileSettings> RepresentativeColumnInfo { get { return repColInfo; } }

        public DatBuildDocSettings(string[] record, string[] header, string keyColName, 
            List<LinkFileSettings> repColInfo, string pathPrefix) :
            base(pathPrefix)
        {
            this.record = record;
            this.header = header;
            this.keyColName = keyColName;
            this.repColInfo = repColInfo;            
        }
    }
}
