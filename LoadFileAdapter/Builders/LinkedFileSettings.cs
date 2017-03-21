using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileAdapter.Builders
{
    public class LinkedFileSettings
    {
        private string column;
        private LinkedFile.FileType fileType;

        public string ColumnName { get { return this.column; } }
        public LinkedFile.FileType Type { get { return this.fileType; } }

        public LinkedFileSettings(string column, LinkedFile.FileType type)
        {
            this.column = column;
            this.fileType = type;
        }
    }
}
