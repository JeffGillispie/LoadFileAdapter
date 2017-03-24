using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoadFileAdapter;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Instructions
{
    public class RepresentativeSettings
    {
        public string ColumnName = null;
        public Representative.FileType FileType = Representative.FileType.Native;

        public RepresentativeSettings()
        {

        }

        public RepresentativeSettings(DatRepresentativeSettings setting)
        {
            this.ColumnName = setting.ColumnName;
            this.FileType = setting.Type;
        }

        public DatRepresentativeSettings GetSetting()
        {
            return new DatRepresentativeSettings(this.ColumnName, this.FileType);
        }
    }
}
