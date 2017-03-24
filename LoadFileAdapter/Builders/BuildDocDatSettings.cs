using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// The settings used to build a document from a DAT file.
    /// </summary>
    public class BuildDocDatSettings : BuildDocSettings
    {
        private string[] record;
        private string[] header;
        private string keyColName;
        private List<DatRepresentativeSettings> repColInfo;

        /// <summary>
        /// The field values of a DAT field record.
        /// </summary>
        public string[] Record { get { return record; } }

        /// <summary>
        /// The DAT file field list.
        /// </summary>
        public string[] Header { get { return header; } }

        /// <summary>
        /// The field name of the key field.
        /// </summary>
        public string KeyColumnName { get { return keyColName; } }

        /// <summary>
        /// A list of representative settings.
        /// </summary>
        public List<DatRepresentativeSettings> RepresentativeColumnInfo { get { return repColInfo; } }

        /// <summary>
        /// Inisitalizes a new instance of <see cref="BuildDocDatSettings"/>.
        /// </summary>
        /// <param name="record">A record used to build a document from a DAT file.</param>
        /// <param name="header">The header or field list from a DAT file.</param>
        /// <param name="keyColName">The field name of the key field.</param>
        /// <param name="repColInfo">A list of representative settings.</param>
        /// <param name="pathPrefix">The path to prepend to representative path values.</param>
        public BuildDocDatSettings(string[] record, string[] header, string keyColName, 
            List<DatRepresentativeSettings> repColInfo, string pathPrefix) :
            base(pathPrefix)
        {
            this.record = record;
            this.header = header;
            this.keyColName = keyColName;
            this.repColInfo = repColInfo;            
        }
    }
}
