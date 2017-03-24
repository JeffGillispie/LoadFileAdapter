using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class DatRepresentativeSettings
    {
        private string column;
        private Representative.FileType fileType;

        public string ColumnName { get { return this.column; } }
        public Representative.FileType Type { get { return this.fileType; } }

        public DatRepresentativeSettings(string column, Representative.FileType type)
        {
            this.column = column;
            this.fileType = type;
        }
    }
}
