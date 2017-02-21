using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class DocumentBuilderArgs
    {
        private List<string[]> pageRecords = null;
        private string pathPrefix = null;
        private string[] nativeRecord = null;
        private StructuredRepresentativeSetting textRepSetting = null;
        private List<SemiStructuredRepresentativeSetting> repColInfo = null;
        private string[] docRecord = null;
        private string[] header = null;
        private string keyColName = null;        

        public List<string[]> PageRecords { get { return this.pageRecords; } }
        public string PathPrefex { get { return this.pathPrefix; } }
        public string[] NativeRecord { get { return this.nativeRecord; } }
        public StructuredRepresentativeSetting TextSetting { get { return this.textRepSetting; } }
        public List<SemiStructuredRepresentativeSetting> RepresentativeColumnInformation { get { return this.repColInfo; } }
        public string[] DocumentRecord { get { return this.docRecord; } }
        public string[] Header { get { return this.header; } }
        public string KeyColumnName { get { return this.keyColName; } }        

        private DocumentBuilderArgs(List<string[]> pageRecords, string[] nativeRecord, string pathPrefix, StructuredRepresentativeSetting textRepSetting,
            string[] docRecord, string[] header, string keyColName, List<SemiStructuredRepresentativeSetting> repColInfo)
        {
            this.pageRecords = pageRecords;
            this.nativeRecord = nativeRecord;
            this.pathPrefix = pathPrefix;
            this.textRepSetting = textRepSetting;
            this.docRecord = docRecord;
            this.header = header;
            this.keyColName = keyColName;
            this.repColInfo = repColInfo;
        }

        public static DocumentBuilderArgs GetLfpArgs(List<string[]> pageRecords, string[] nativeRecord, string pathPrefix, StructuredRepresentativeSetting textRepSetting)
        {
            return new DocumentBuilderArgs(pageRecords, nativeRecord, pathPrefix, textRepSetting, null, null, null, null);
        }

        public static DocumentBuilderArgs GetOptArgs(List<string[]> pageRecords, string pathPrefix, StructuredRepresentativeSetting textRepSetting)
        {
            return new DocumentBuilderArgs(pageRecords, null, pathPrefix, textRepSetting, null, null, null, null);
        }

        public static DocumentBuilderArgs GetTextDelimitedArgs(string[] docRecord, string[] header, string keyColName, List<SemiStructuredRepresentativeSetting> repColInfo, string pathPrefix)
        {
            return new DocumentBuilderArgs(null, null, pathPrefix, null, docRecord, header, keyColName, repColInfo);
        }
    }
}
