using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class BuildDocCollectionDatSettings : BuildDocCollectionSettings
    {
        private bool hasHeader;
        private string keyColName;
        private string parentColName;
        private string childColName;
        private string childColDelim;
        private List<LinkedFileSettings> repColInfo;

        public bool HasHeader { get { return hasHeader; } }
        public string KeyColumnName { get { return keyColName; } }
        public string ParentColumnName { get { return parentColName; } }
        public string ChildColumnName { get { return childColName; } }
        public string ChildColumnDelimiter { get { return childColDelim; } }
        public List<LinkedFileSettings> RepresentativeColumnInfo { get { return repColInfo; } }

        public BuildDocCollectionDatSettings(List<string[]> records, string pathPrefix, 
            bool hasHeader, string keyColName, string parentColName, string childColName, string childColDelim,
            List<LinkedFileSettings> repColInfo) :
            base(records, pathPrefix)
        {
            this.hasHeader = hasHeader;
            this.keyColName = keyColName;
            this.parentColName = parentColName;
            this.childColName = childColName;
            this.childColDelim = childColDelim;
            this.repColInfo = repColInfo;
        }
    }
}
