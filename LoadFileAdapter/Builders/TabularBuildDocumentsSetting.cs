using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class TabularBuildDocumentsSetting : BuildDocumentsSetting
    {
        private bool hasHeader;
        private string keyColName;
        private string parentColName;
        private string childColName;
        private string childColDelim;
        private List<SemiStructuredRepresentativeSetting> repColInfo;

        public bool HasHeader { get { return hasHeader; } }
        public string KeyColumnName { get { return keyColName; } }
        public string ParentColumnName { get { return parentColName; } }
        public string ChildColumnName { get { return childColName; } }
        public string ChildColumnDelimiter { get { return childColDelim; } }
        public List<SemiStructuredRepresentativeSetting> RepresentativeColumnInfo { get { return repColInfo; } }

        public TabularBuildDocumentsSetting(List<string[]> records, string pathPrefix, 
            bool hasHeader, string keyColName, string parentColName, string childColName, string childColDelim,
            List<SemiStructuredRepresentativeSetting> repColInfo) :
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
