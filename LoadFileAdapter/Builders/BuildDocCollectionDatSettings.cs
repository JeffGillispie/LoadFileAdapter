using System.Collections.Generic;

namespace LoadFileAdapter.Builders
{
    /// <summary>
    /// Represents the settings used to build a document collection from a DAT file.
    /// </summary>
    public class BuildDocCollectionDatSettings : BuildDocCollectionSettings
    {
        private bool hasHeader;
        private string keyColName;
        private string parentColName;
        private string childColName;
        private string childColDelim;
        private List<DatRepresentativeSettings> repColInfo;

        /// <summary>
        /// Indicates if the first record is a header.
        /// </summary>
        public bool HasHeader { get { return hasHeader; } }

        /// <summary>
        /// The field name of the key field (i.e. DOCID).
        /// </summary>
        public string KeyColumnName { get { return keyColName; } }

        /// <summary>
        /// The field name of the parent field (i.e. ParentID).
        /// </summary>
        public string ParentColumnName { get { return parentColName; } }

        /// <summary>
        /// The field name of the child list field (i.e. AttachIDs).
        /// </summary>
        public string ChildColumnName { get { return childColName; } }

        /// <summary>
        /// The separator used in the child list field (i.e. AttachIDs).
        /// </summary>
        public string ChildColumnDelimiter { get { return childColDelim; } }

        /// <summary>
        /// A list of representative field settings.
        /// </summary>
        public List<DatRepresentativeSettings> RepresentativeColumnInfo { get { return repColInfo; } }

        /// <summary>
        /// Initializes a new instance of <see cref="BuildDocCollectionDatSettings"/>
        /// </summary>
        /// <param name="records">A list of records that contain document metadata.</param>
        /// <param name="pathPrefix">The path prefix that should be prepended to representative path values.</param>
        /// <param name="hasHeader">Indicates if the first record in records is a header.</param>
        /// <param name="keyColName">The field name of the key field.</param>
        /// <param name="parentColName">The field name of the parent field.</param>
        /// <param name="childColName">The field name of the child list field.</param>
        /// <param name="childColDelim">The separator used in the child list field.</param>
        /// <param name="repColInfo">A list of representative field settings.</param>
        public BuildDocCollectionDatSettings(List<string[]> records, string pathPrefix, 
            bool hasHeader, string keyColName, string parentColName, string childColName, string childColDelim,
            List<DatRepresentativeSettings> repColInfo) :
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
