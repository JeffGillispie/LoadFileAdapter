using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoadFileAdapter;
using LoadFileAdapter.Builders;

namespace LoadFileAdapter.Instructions
{
    public class LinkedFile
    {
        public string ColumnName = null;
        public Representative.Type Type = Representative.Type.Native;

        public LinkedFile()
        {

        }

        public LinkedFile(LinkFileSettings setting)
        {
            this.ColumnName = setting.RepresentativeColumn;
            this.Type = setting.RepresentativeType;
        }

        public LinkFileSettings GetSetting()
        {
            return new LinkFileSettings(this.ColumnName, this.Type);
        }
    }
}
