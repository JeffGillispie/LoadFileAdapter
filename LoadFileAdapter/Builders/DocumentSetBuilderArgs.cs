using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class DocumentSetBuilderArgs
    {
        private List<string[]> records = null;
        private string pathPrefix = null;
        private StructuredRepresentativeSetting textRepSetting = null;
        private List<SemiStructuredRepresentativeSetting> repColInfo = null;
        private bool hasHeader = true;
        private string keyColName = null;
        private string parentColName = null;
        private string childColName = null;
        private string childColDelim = null;
        
        public List<string[]> Records { get { return this.records; } }
        public string PathPrefix { get { return this.pathPrefix; } }
        public StructuredRepresentativeSetting TextRepresentativeSetting { get { return this.textRepSetting; } }
        public List<SemiStructuredRepresentativeSetting> RepresentativeColumnInfo { get { return this.repColInfo; } }
        public bool HasHeader { get { return this.hasHeader; } }
        public string KeyColumnName { get { return this.keyColName; } }
        public string ParentColumnName { get { return this.parentColName; } }
        public string ChildColumnName { get { return this.childColName; } }
        public string ChildColumnDelimiter { get { return this.childColDelim; } }

        private DocumentSetBuilderArgs(List<string[]> records, string pathPrefix, StructuredRepresentativeSetting textRepSetting, 
            List<SemiStructuredRepresentativeSetting> repColInfo, bool hasHeader, string keyColName, string parentColName, string childColName, string childColDelim)
        {
            this.records = records;
            this.pathPrefix = pathPrefix;
            this.textRepSetting = textRepSetting;
            this.repColInfo = repColInfo;
            this.hasHeader = hasHeader;
            this.keyColName = keyColName;
            this.parentColName = parentColName;
            this.childColName = childColName;
            this.childColDelim = childColDelim;
        }

        public static DocumentSetBuilderArgs GetImageSetArgs(List<string[]> records, string pathPrefix, StructuredRepresentativeSetting textRepSetting)
        {
            return new DocumentSetBuilderArgs(records, pathPrefix, textRepSetting, null, true, null, null, null, null);
        }

        public static DocumentSetBuilderArgs GetTextDelimitedArgs(List<string[]> records, bool hasHeader, string keyColName, string parentColName, string childColName, string childColDelim, 
            List<SemiStructuredRepresentativeSetting> repColInfo, string pathPrefix)
        {
            return new DocumentSetBuilderArgs(records, pathPrefix, null, repColInfo, hasHeader, keyColName, parentColName, childColName, childColDelim);
        }
    }
}
