﻿using System.IO;
using System.Linq;
using System.Text;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Parsers;

namespace LoadFileAdapter.Instructions
{    
    public class DatImport : Import
    {     
        public DelimitersBuilder Delimiters = new DelimitersBuilder(LoadFileAdapter.Parsers.Delimiters.CONCORDANCE);
        public bool HasHeader = true;
        public string KeyColumnName = null;
        public string ParentColumnName = null;
        public string ChildColumnName = null;
        public string ChildColumnDelimiter = null;
        public RepresentativeSettings[] LinkedFiles = null;

        public DatImport() : base(null, null)
        {
            // do nothing here
        }

        public DatImport(FileInfo file, Encoding encoding, Delimiters delimiters, bool hasHeader, 
            string keyColName, string parentColName, string childColName, string childColDelim,
            DatRepresentativeSettings[] linkedFiles) :
            base(file, encoding)
        {
            this.Delimiters = new DelimitersBuilder(delimiters);
            this.HasHeader = hasHeader;
            this.KeyColumnName = keyColName;
            this.ParentColumnName = parentColName;
            this.ChildColumnName = childColName;
            this.ChildColumnDelimiter = childColDelim;
            this.LinkedFiles = (linkedFiles != null) ? linkedFiles.Select(f => new RepresentativeSettings(f)).ToArray() : null;
        }               
    }
}
