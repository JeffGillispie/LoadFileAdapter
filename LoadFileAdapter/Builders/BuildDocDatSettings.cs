using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    public class BuildDocDatSettings : BuildDocSettings
    {
        private string[] record;
        private string[] header;
        private string keyColName;
        private List<LinkedFileSettings> repColInfo;

        public string[] Record { get { return record; } }
        public string[] Header { get { return header; } }
        public string KeyColumnName { get { return keyColName; } }
        public List<LinkedFileSettings> RepresentativeColumnInfo { get { return repColInfo; } }

        public BuildDocDatSettings(string[] record, string[] header, string keyColName, 
            List<LinkedFileSettings> repColInfo, string pathPrefix) :
            base(pathPrefix)
        {
            this.record = record;
            this.header = header;
            this.keyColName = keyColName;
            this.repColInfo = repColInfo;            
        }
    }
}
